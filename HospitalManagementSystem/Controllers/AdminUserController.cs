using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
   
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminUsersController(ApplicationDbContext db) => _db = db;

  
        public async Task<IActionResult> Index()
        {
          
            var users = await _db.Users
                                 .AsNoTracking()
                                 .ToListAsync();

           
            var roles = await _db.Roles.AsNoTracking().ToListAsync();
            ViewBag.Roles = roles.ToDictionary(r => r.RoleId, r => r.RoleName);
            return View(users);
        }

  
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                DOB = user.DOB
            };

            return View(model);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FindAsync(model.UserId);
            if (user == null) return NotFound();

        
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.DOB = model.DOB;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


public async Task<IActionResult> AssignRole(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var roles = await _db.Roles.AsNoTracking().ToListAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.RoleId = roleId;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

      
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
