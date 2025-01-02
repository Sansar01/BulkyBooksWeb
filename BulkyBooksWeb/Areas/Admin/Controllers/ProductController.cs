using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> obj = _unitOfWork.Product.GetAll().ToList();
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
    }
}
