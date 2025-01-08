using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Company> obj = _unitOfWork.Company.GetAll().ToList();
            //IEnumerable<SelectListItem> CompanyList = (IEnumerable<SelectListItem>)_unitOfWork.Company.GetAll().ToList();\

            return View(obj);
        }

        //public IActionResult Upsert(int? id)
        //{      

        //    if(id == null || id == 0)
        //    {
              
        //        return NotFound();
        //    }
        //    else
        //    {
                
        //        Company company = _unitOfWork.Company.Get(u =>u.Id == id);
        //        return View(company);
        //    }
        //}

        //[HttpPost]
        //public IActionResult Upsert(Company company)
        //{

        //    if (ModelState.IsValid)
        //    {
                
        //        if (company.Id == 0)
        //        {
        //            _unitOfWork.Company.Add(company);
        //        }
        //        else
        //        {
        //            _unitOfWork.Company.Update(company);  
        //        }
        //        _unitOfWork.Company.Add(company);
        //        _unitOfWork.save();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {

        //        return View(company);

        //    }
           
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(model);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {

                   return View(model); 
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var findCompany = _unitOfWork.Company.Get(u => u.Id == id);
            if (findCompany == null)
            {
                return NotFound();
            }

            return View(findCompany);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Company? findCompany = _unitOfWork.Company.Get(u => u.Id == id);

            if (findCompany == null)
            {
                return NotFound();
            }

            return View(findCompany);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Company model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(model);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var findCompany = _unitOfWork.Company.Get(u => u.Id == id);

            if (findCompany == null)
            {
                return NotFound();
            }
            return View(findCompany);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Company model)
        {
            var findCompany = _unitOfWork.Company.Get(u => u.Id == model.Id);

            if (findCompany == null)
            {
                return NotFound();
            }

            _unitOfWork.Company.Remove(findCompany);
            _unitOfWork.save();
            return RedirectToAction("Index");
        }

        #region Api calls

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Company").ToList();
        //    return Json(new {data = obj});
        //}

        //public IActionResult Deletes(int id)
        //{
        //    var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);    
        //     if(productToBeDeleted == null)
        //    {
        //         return Json(new {success = false, message = "Error while deleting"});
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
