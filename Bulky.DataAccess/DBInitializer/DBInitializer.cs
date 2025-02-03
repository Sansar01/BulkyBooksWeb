using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Bulky.Utilty;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DBInitializer
{
    public class DBInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DBInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            //migration if they are not applied

            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                //crete roles if they are not created

                if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Name = "Sansar Tiwari",
                        Email = "admin@gmail.com",
                        PhoneNumber = "1234567890",
                        StreetAddress = "jawahar",
                        State = "M.P",
                        PostalCode = "485001",
                        City = "Satna"
                    }, "Admin123").GetAwaiter().GetResult();

                    ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                    _userManager.AddToRoleAsync(applicationUser, SD.Role_Admin).GetAwaiter().GetResult();
                }

                return;
            }

            //if roles are not created, then we will created admin user as well

            
        }
    }
}
