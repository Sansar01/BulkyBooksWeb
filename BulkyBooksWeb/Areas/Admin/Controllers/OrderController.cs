using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilty;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string? status)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList(),
                OrderDetail = new List<OrderDetail>()
            };

            switch (status)
            {
                case "pending":
                    orderVM.OrderHeader = orderVM.OrderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    orderVM.OrderHeader = orderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderVM.OrderHeader = orderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderVM.OrderHeader = orderVM.OrderHeader.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return View(orderVM);
        }

        //#region Api calls

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    List<OrderHeader> obj = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        //    return Json(new { data = obj });
        //}

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

        //#endregion
    }
}
