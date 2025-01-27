using Azure.Core;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //[BindProperty]
        //public OrderVM OrderVM { get; set; }

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Index(string? status)
        //{
        //    OrderVM = new()
        //    {
        //        OrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList(),
        //        OrderDetail = new List<OrderDetail>()
        //    };

        //    switch (status)
        //    {
        //        case "pending":
        //            OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
        //            break;
        //        case "inprocess":
        //            OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusInProcess);
        //            break;
        //        case "completed":
        //            OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusShipped);
        //            break;
        //        case "approved":
        //            OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusApproved);
        //            break;
        //        default:
        //            break;
        //    }

        //    return View(OrderVM);
        //}

        public IActionResult Details(int orderId)
        {

            OrderVM orderVM = new()
            {

                OrderHeader = new List<OrderHeader> { _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser") },
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.Id == orderId, includeProperties: "Product")

            };

            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {

            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeader.Id);

            orderHeaderFromDb.Name = OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderHeader.PhoneNumber;
            orderHeaderFromDb.Street = OrderHeader.Street;
            orderHeaderFromDb.City = OrderHeader.City;
            orderHeaderFromDb.State = OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.save();

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });

            return View();
        }

        #region Api calls

        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            // IEnumerable<OrderHeader> obj = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            OrderVM OrderVM = new()
            {
                OrderHeader = new List<OrderHeader>(),
                //OrderDetail = new List<OrderDetail>()
            };

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderVM = new()
                {
                    OrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList()
                };
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                OrderVM = new()
                {
                    OrderHeader = _unitOfWork.OrderHeader.GetAll(u =>u.ApplicationUserId == userId, includeProperties: "ApplicationUser")
                };
            }

            switch (status)
            {
                case "pending":
                    OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    OrderVM.OrderHeader = OrderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = OrderVM.OrderHeader });
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderHeader.Id,SD.StatusInProcess);
            return View();
        }

        //public IActionResult Deletes(int id)
        //{
        //    var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productToBeDeleted == null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });
        //    }

        //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    _unitOfWork.Product.Remove(productToBeDeleted);
        //    _unitOfWork.save();

        //    return Json(new { success = true, message = "Delete successfull" });
        //}

        #endregion
    }
}
