using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBooksWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationUser>  applicationUsers = _context.ApplicationUsers.Include(u=>u.Company).ToList();

            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();    

            foreach(var user in applicationUsers)
            {
                var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;
                var Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

                user.Role = Role;
            }

            return View(applicationUsers);
        
        }
    }
}
