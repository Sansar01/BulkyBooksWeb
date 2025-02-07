using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilty;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;    

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM RoleVM = new RoleManagementVM()
            {

                ApplicationUser = _context.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _context.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _context.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = _context.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string RoleId = _context.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _context.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                ApplicationUser applicationUser = _context.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);

                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _context.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return View("Index");

        }

        //public IActionResult Index()
        //{
        //    IEnumerable<ApplicationUser>  applicationUsers = _context.ApplicationUsers.Include(u=>u.Company).ToList();

        //    var userRoles = _context.UserRoles.ToList();
        //    var roles = _context.Roles.ToList();    

        //    foreach(var user in applicationUsers)
        //    {
        //        var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;
        //        var Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

        //        user.Role = Role;

        //        user.LockoutEnd = user.LockoutEnd.HasValue ? DateTime.Now : (DateTime?)null;
        //    }


        //    return View(applicationUsers);

        //}

        //[HttpPost]
        //public IActionResult LockUnlock([FromBody]string id)
        //{
        //    var obj = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);

        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    if (obj.LockoutEnd != null && obj.LockoutEnd > DateTime.Now)
        //    {
        //        obj.LockoutEnd = DateTime.Now;
        //    }
        //    else
        //    {
        //        obj.LockoutEnd = DateTime.Now.AddYears(1000);
        //    }
        //    _context.SaveChanges();
        //    return View("Index");
        //}

        #region Api calls

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<ApplicationUser> applicationUsers = _context.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            foreach (var user in applicationUsers)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                user.Role = Role;

                user.LockoutEnd = user.LockoutEnd.HasValue ? DateTime.Now : (DateTime?)null;
            }

            return Json(new {data = applicationUsers});
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var obj = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if (obj == null)
            {
                return Json(new {success = false , message = "Error while Locking/Unlocking"});
            }
            if (obj.LockoutEnd != null && obj.LockoutEnd > DateTime.Now)
            {
                obj.LockoutEnd = DateTime.Now;
            }
            else
            {
                obj.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Operation Successfull" });
        }

        #endregion Api calls

    }
}
