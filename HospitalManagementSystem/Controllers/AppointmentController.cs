using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AppointmentController(ApplicationDbContext db) => _db = db;

       
        private int? GetCurrentUserId()
        {
            var s = HttpContext.Session.GetString("UserId");
            if (int.TryParse(s, out var id)) return id;
            return null;
        }

    
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

       
        private bool IsDoctor()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            return role.Equals("Doctor", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> Index()
        {
            var uid = GetCurrentUserId();
            IQueryable<Appointment> query = _db.Appointments.Include(a => a.Patient)
                                                            .Include(a => a.Doctor);

            if (IsAdmin())
            {
               
                var allAppointments = await query.ToListAsync();
                return View(allAppointments);
            }
            else if (IsDoctor())
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));

                var appointments = await _db.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => a.Doctor.OwnerUserId == userId)
                    .ToListAsync();

                return View(appointments);
            }

            else
            {
               
                var patientAppointments = await query.Where(a => a.OwnerUserId == uid)
                                                    .ToListAsync();
                return View(patientAppointments);
            }

            return View(new List<Appointment>());
        }



       
        public async Task<IActionResult> Create()
        {
           
            if (IsDoctor())
            {
               
                TempData["Info"] = "Doctors cannot create appointments.";
                return RedirectToAction(nameof(Index));
            }

            var role = HttpContext.Session.GetString("UserRole") ?? "";
            var userIdStr = HttpContext.Session.GetString("UserId") ?? "";
            int.TryParse(userIdStr, out var currentUserId);

            if (IsAdmin())
            {
                ViewBag.Patients = new SelectList(await _db.Patients.ToListAsync(), "PatientId", "PatientName");
            }
            else
            {
                var myPatients = await _db.Patients.Where(p => p.OwnerUserId == currentUserId).ToListAsync();
                ViewBag.Patients = new SelectList(myPatients, "PatientId", "PatientName");

                if (myPatients.Count == 1)
                    ViewBag.PreselectedPatientId = myPatients[0].PatientId;
            }

            ViewBag.Doctors = new SelectList(await _db.Doctors.ToListAsync(), "DoctorId", "Name");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
           
            if (IsDoctor())
            {
                TempData["Info"] = "Doctors cannot create appointments.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");

            var uid = GetCurrentUserId();
            if (uid != null)
            {
                model.OwnerUserId = uid.Value;
            }

            if (!ModelState.IsValid)
            {
                int.TryParse(HttpContext.Session.GetString("UserId"), out var currentUserId);

                if (IsAdmin())
                {
                    ViewBag.Patients = new SelectList(await _db.Patients.ToListAsync(), "PatientId", "PatientName", model.PatientId);
                }
                else
                {
                    var myPatients = await _db.Patients.Where(p => p.OwnerUserId == currentUserId).ToListAsync();
                    ViewBag.Patients = new SelectList(myPatients, "PatientId", "PatientName", model.PatientId);

                    if (myPatients.Count == 1)
                        ViewBag.PreselectedPatientId = myPatients[0].PatientId;
                }

                ViewBag.Doctors = new SelectList(await _db.Doctors.ToListAsync(), "DoctorId", "Name", model.DoctorId);
                return View(model);
            }

            _db.Appointments.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _db.Appointments.FindAsync(id);
            if (app == null) return NotFound();

            var uid = GetCurrentUserId();
            if (!IsAdmin() && app.OwnerUserId != uid)
                return RedirectToAction(nameof(Index));

            ViewBag.Patients = new SelectList(await _db.Patients.ToListAsync(), "PatientId", "PatientName", app.PatientId);
            ViewBag.Doctors = new SelectList(await _db.Doctors.ToListAsync(), "DoctorId", "Name", app.DoctorId);
            return View(app);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Appointment model)
        {
            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");

            var existing = await _db.Appointments.AsNoTracking()
                                                 .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);
            if (existing == null) return NotFound();

            var uid = GetCurrentUserId();
            if (!IsAdmin() && existing.OwnerUserId != uid)
                return RedirectToAction(nameof(Index));

            model.OwnerUserId = existing.OwnerUserId;

            if (!ModelState.IsValid)
            {
                ViewBag.Patients = new SelectList(await _db.Patients.ToListAsync(), "PatientId", "PatientName", model.PatientId);
                ViewBag.Doctors = new SelectList(await _db.Doctors.ToListAsync(), "DoctorId", "Name", model.DoctorId);
                return View(model);
            }

            _db.Appointments.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

      
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _db.Appointments.FindAsync(id);
            if (app == null) return RedirectToAction(nameof(Index));

            var uid = GetCurrentUserId();
            if (!IsAdmin() && app.OwnerUserId != uid)
                return RedirectToAction(nameof(Index));

            _db.Appointments.Remove(app);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
