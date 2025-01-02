using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> obj = _unitOfWork.Category.GetAll().ToList();
            return View(obj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(model);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var findCategory = _unitOfWork.Category.Get(u => u.CategoryId == id);
            if (findCategory == null)
            {
                return NotFound();
            }

            return View(findCategory);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Category? findCategory = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (findCategory == null)
            {
                return NotFound();
            }

            return View(findCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(model);
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var findCategory = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (findCategory == null)
            {
                return NotFound();
            }
            return View(findCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category model, int id)
        {
            var findCategory = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (findCategory == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(findCategory);
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
