using HospitalManagementSystem.Context;
using HospitalManagementSystem.Filters;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                DoctorCount = await _db.Doctors.CountAsync(),
                PatientCount = await _db.Patients.CountAsync(),
                AppointmentCount = await _db.Appointments.CountAsync(),
                LabOrderCount = await _db.LabOrders.CountAsync()
            };

            return View(model);
        }
    }
}
