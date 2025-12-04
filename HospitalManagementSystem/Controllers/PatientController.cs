using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _db;
        public PatientController(ApplicationDbContext db) { _db = db; }

 
        private int? GetCurrentUserId()
        {
            var s = HttpContext.Session.GetString("UserId");
            if (int.TryParse(s, out var id)) return id;
            return null;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        

        public async Task<IActionResult> Index()
        {
            if (IsAdmin())
            {
                var list = await _db.Patients
                                    .Include(p => p.Doctor)
                                    .ToListAsync();
                return View(list);
            }
            else
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

             
                var doctor = await _db.Doctors
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(d => d.OwnerUserId == userId);

                if (doctor != null)
                {
                    var list = await _db.Patients
                                        .Where(p => p.DoctorId == doctor.DoctorId)
                                        .Include(p => p.Doctor)
                                        .ToListAsync();
                    return View(list);
                }

            
                var patientList = await _db.Patients
                                           .Where(p => p.OwnerUserId == userId)
                                           .Include(p => p.Doctor)
                                           .ToListAsync();
                return View(patientList);
            }
        }


        public IActionResult Create()
        {
            ViewBag.DoctorList = new SelectList(_db.Doctors, "DoctorId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
           
            var currentUserId = GetCurrentUserId();
            if (currentUserId != null)
            {
                patient.OwnerUserId = currentUserId.Value;
            }

           
            ModelState.Remove(nameof(Patient.Doctor));

            if (!ModelState.IsValid)
            {
               
                var errors = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ModelErrors"] = errors;

                ViewBag.DoctorList = new SelectList(_db.Doctors, "DoctorId", "Name", patient.DoctorId);
                return View(patient);
            }

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

      
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || p.OwnerUserId != currentUserId))
            {
               
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DoctorList = new SelectList(_db.Doctors, "DoctorId", "Name", p.DoctorId);
            return View(p);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            ModelState.Remove(nameof(Patient.Doctor));

           
            var existing = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.PatientId == patient.PatientId);
            if (existing == null) return NotFound();

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || existing.OwnerUserId != currentUserId))
            {
            
                return RedirectToAction(nameof(Index));
            }

           
            patient.OwnerUserId = existing.OwnerUserId;

            if (!ModelState.IsValid)
            {
                ViewBag.DoctorList = new SelectList(_db.Doctors, "DoctorId", "Name", patient.DoctorId);
                return View(patient);
            }

            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

   
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return RedirectToAction(nameof(Index));

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || p.OwnerUserId != currentUserId))
            {
            
                return RedirectToAction(nameof(Index));
            }

            _db.Patients.Remove(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
