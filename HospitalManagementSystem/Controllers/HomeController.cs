using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel()
            {
                TotalPatients = await _db.Patients.CountAsync(),
                TotalDoctors = await _db.Doctors.CountAsync(),
                TotalAppointments = await _db.Appointments.CountAsync(),
                PendingLabOrders = await _db.LabOrders.Where(x => x.Status == LabStatus.Pending).CountAsync(),
                RecentAppointments = await _db.Appointments
                                            .Include(p => p.Patient)
                                            .Include(d => d.Doctor)
                                            .OrderByDescending(x => x.AppointmentDate)
                                            .Take(5)
                                            .ToListAsync()
            };

            return View(model);
        }
    }
}

