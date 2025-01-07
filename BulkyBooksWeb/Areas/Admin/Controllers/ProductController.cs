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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            //IEnumerable<SelectListItem> CategoryList = (IEnumerable<SelectListItem>)_unitOfWork.Category.GetAll().ToList();\

            return View(obj);
        }

        public IActionResult Upsert(int? id)
        {      

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString()
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u =>u.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                 string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"image\product");
  
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath,filename),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }
                
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);  
                }
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {

                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString()
                });

            }
            return View(productVM);
        }

        public IActionResult Create()
        {
            //IEnumerable<SelectListItem> CategoryList = 

            //ViewBag.CategoryList = CategoryList;
            //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(model.Product);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {

                model.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CategoryId.ToString()
                });
                    
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var findProduct = _unitOfWork.Product.Get(u => u.Id == id);
            if (findProduct == null)
            {
                return NotFound();
            }

            return View(findProduct);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Product? findProduct = _unitOfWork.Product.Get(u => u.Id == id);

            if (findProduct == null)
            {
                return NotFound();
            }

            return View(findProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(model);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var findProduct = _unitOfWork.Product.Get(u => u.Id == id);

            if (findProduct == null)
            {
                return NotFound();
            }
            return View(findProduct);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product model, int id)
        {
            var findProduct = _unitOfWork.Product.Get(u => u.Id == id);

            if (findProduct == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(findProduct);
            _unitOfWork.save();
            return RedirectToAction("Index");
        }

        #region Api calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = obj});
        }

        public IActionResult Deletes(int id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);    
             if(productToBeDeleted == null)
            {
                 return Json(new {success = false, message = "Error while deleting"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
