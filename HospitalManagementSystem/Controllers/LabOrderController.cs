using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem.Context;
using HospitalManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HospitalManagementSystem.Controllers
{
    public class LabOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LabOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        private bool CanModifyLabOrders()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Doctor", StringComparison.OrdinalIgnoreCase)
                || role.Equals("LabOrder", StringComparison.OrdinalIgnoreCase);
        }

        private void LoadPatients(int? selected = null)
        {
            ViewBag.PatientId = new SelectList(_context.Patients.ToList(), "PatientId", "PatientName", selected);
        }

       
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            var userIdStr = HttpContext.Session.GetString("UserId") ?? "";
            int.TryParse(userIdStr, out var currentUserId);

           
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ||
                role.Equals("LabOrder", StringComparison.OrdinalIgnoreCase))
            {
                var listAll = _context.LabOrders.Include(x => x.Patient).ToList();
                return View(listAll);
            }

            
            var list = _context.LabOrders
                .Include(x => x.Patient)
                .Where(x => x.Patient.OwnerUserId == currentUserId)
                .ToList();

            return View(list);
        }

       
        [HttpGet]
        public IActionResult Create()
        {
           
            if (!CanModifyLabOrders())
                return RedirectToAction(nameof(Index));

            LoadPatients();
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabOrder labOrder)
        {
            
            if (!CanModifyLabOrders())
                return RedirectToAction(nameof(Index));

            ModelState.Remove(nameof(LabOrder.Patient));   

            if (ModelState.IsValid)
            {
               
                _context.LabOrders.Add(labOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

           
            LoadPatients(labOrder.PatientId);
            return View(labOrder);
        }

       
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.LabOrders.Find(id);
            if (order == null) return NotFound();

          
            if (!CanModifyLabOrders())
                return RedirectToAction(nameof(Index));

            LoadPatients(order.PatientId);
            return View(order);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(LabOrder labOrder)
        {
        
            if (!CanModifyLabOrders())
                return RedirectToAction(nameof(Index));

            ModelState.Remove(nameof(LabOrder.Patient));
            if (ModelState.IsValid)
            {
                _context.LabOrders.Update(labOrder);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadPatients(labOrder.PatientId);
            return View(labOrder);
        }

      
        [HttpGet] 
        public IActionResult Delete(int id)
        {
          
            if (!CanModifyLabOrders())
                return RedirectToAction(nameof(Index));

            var order = _context.LabOrders.Find(id);
            if (order != null)
            {
                _context.LabOrders.Remove(order);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
