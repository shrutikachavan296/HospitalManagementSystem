
using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DoctorController(ApplicationDbContext db)
        {
            _db = db;
        }

    
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(role) && role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

       
        private bool IsDoctor()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            return role.Equals("Doctor", StringComparison.OrdinalIgnoreCase);
        }

       
        private int? GetCurrentUserId()
        {
            var s = HttpContext.Session.GetString("UserId");
            if (int.TryParse(s, out var id)) return id;
            return null;
        }

     
        public async Task<IActionResult> Index()
        {
            
            var allDoctors = await _db.Doctors
                                      .Include(d => d.Patients)
                                      .ToListAsync();


            if (IsDoctor())
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId != null)
                {
                    var myDoctor = allDoctors.FirstOrDefault(d => d.OwnerUserId == currentUserId.Value);
                    if (myDoctor != null)
                    {
                        ViewBag.CurrentDoctorId = myDoctor.DoctorId;
          
                        ViewBag.CurrentDoctorPatients = myDoctor.Patients ?? new List<Patient>();
                    }
                }
            }

         
            return View(allDoctors);
        }

       
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Index)); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Index));

            
            ModelState.Remove(nameof(Doctor.Patients));
            if (!ModelState.IsValid) return View(doctor);

            _db.Doctors.Add(doctor);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Index));

            var d = await _db.Doctors.FindAsync(id);
            if (d == null) return NotFound();
            return View(d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Index));

            
            ModelState.Remove(nameof(Doctor.Patients));
            if (!ModelState.IsValid) return View(doctor);

            _db.Doctors.Update(doctor);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

     
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction(nameof(Index));

            var d = await _db.Doctors.FindAsync(id);
            if (d != null)
            {
                _db.Doctors.Remove(d);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
