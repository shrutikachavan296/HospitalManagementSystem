using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db) => _db = db;

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hash);
        }

      
        public async Task<IActionResult> Register()
        {
            ViewBag.Roles = await _db.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _db.Roles.ToListAsync();
                return View(model);
            }

            var exists = await _db.Users.AnyAsync(u => u.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already registered.");
                ViewBag.Roles = await _db.Roles.ToListAsync();
                return View(model);
            }

           
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                DOB = model.DOB,
                PasswordHash = HashPassword(model.Password),
                RoleId = model.RoleId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            
            var role = await _db.Roles.FindAsync(model.RoleId);
            if (role != null)
            {
                if (role.RoleName.Equals("Doctor", System.StringComparison.OrdinalIgnoreCase))
                {
                    var doctor = new Doctor
                    {
                        Name = model.FullName,
                        Phone = model.Phone,
                        Specialization = "General",
                        OwnerUserId = user.UserId
                        
                    };
                    _db.Doctors.Add(doctor);
                    await _db.SaveChangesAsync();
                }
                else if (role.RoleName.Equals("Patient", System.StringComparison.OrdinalIgnoreCase))
                {
                    
                    var defaultDoctor = await _db.Doctors.FirstOrDefaultAsync();
                    var patient = new Patient
                    {
                        PatientName = model.FullName,
                        Phone = model.Phone,
                        Address = model.Address,
                        DOB = model.DOB,
                        OwnerUserId = user.UserId,
                        DoctorId = defaultDoctor != null ? defaultDoctor.DoctorId : 0
                    };
                    _db.Patients.Add(patient);
                    await _db.SaveChangesAsync();
                }
            }

            return RedirectToAction("Login");
        }

    
        public IActionResult Login() => View();

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var hashed = HashPassword(model.Password).ToUpper();
            var user = await _db.Users.Include(u => u.Role)
                                      .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash.ToUpper() == hashed);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

          
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role?.RoleName ?? "");

           
            if (user.Role?.RoleName == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

     
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
