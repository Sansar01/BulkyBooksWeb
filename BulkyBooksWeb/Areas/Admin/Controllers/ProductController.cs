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
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages").ToList();
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
                productVM.Product = _unitOfWork.Product.Get(u =>u.Id == id,includeProperties:"ProductImages");
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,List<IFormFile>? files)
        {

            if (ModelState.IsValid)
            {

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-"+productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                            using (var fileStream = new FileStream(Path.Combine(finalPath, filename), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new() 
                        { 
                          ImageUrl = @"\" + productPath + @"\" + filename,
                          ProductId = productVM.Product.Id, 
                        };

                        if(productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                        //_unitOfWork.ProductImage.Add(productImage);

                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.save();

                    //if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    //{
                    //    //delete the old image
                    //    var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    //    if (System.IO.File.Exists(oldImagePath))
                    //    {
                    //        System.IO.File.Delete(oldImagePath);
                    //    }
                    //}

                    //using (var fileStream = new FileStream(Path.Combine(productPath,filename),FileMode.Create))
                    //{
                    //    file.CopyTo(fileStream);
                    //}
                    //productVM.Product.ImageUrl = @"\images\product\" + filename;
                }
                
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

        [HttpPut]
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

        [HttpDelete]
        public IActionResult Delete(Product model)
        {
            var findProduct = _unitOfWork.Product.Get(u => u.Id == model.Id);

            string productPath = @"images\products\product-" + model.Id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            if (findProduct == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(findProduct);
            _unitOfWork.save();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imagetobeDeleted = _unitOfWork.ProductImage.Get(u=>u.Id == imageId);
            var productId = imagetobeDeleted.ProductId;
            if(imagetobeDeleted != null)
            {
                if(!string.IsNullOrEmpty(imagetobeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imagetobeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Remove(imagetobeDeleted);
                _unitOfWork.save();
            }
            return RedirectToAction(nameof(Upsert),new {id = productId});
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

            //var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            //if (System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
