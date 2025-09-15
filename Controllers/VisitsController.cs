using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using A_Very_Simple_HIS.Data;
using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Authorization;
using A_Very_Simple_HIS.ViewModels;

namespace A_Very_Simple_HIS.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize("Visits.View")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize("Visits.View")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm =  _context.Visits
                .Where(v => v.Id == id)
                .Select(v => new VisitViewModel()
                {
                    Id = v.Id,
                    PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                    DoctorName = v.Doctor.FirstName + " " + v.Doctor.LastName,
                    VisitDate = v.VisitDate
                })
                .ToList();
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [Authorize("Visits.Create")]
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "FullName");
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FirstName");
            return View();
        }

        [Authorize("Visits.Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientId,DoctorId,VisitDate,VisitType,Notes")] Visit visit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", visit.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", visit.PatientId);
            return View(visit);
        }

        [Authorize("Visits.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits.FindAsync(id);
            if (visit == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", visit.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", visit.PatientId);
            return View(visit);
        }

        [Authorize("Visits.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,VisitDate,VisitType,Notes")] Visit visit)
        {
            if (id != visit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitExists(visit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", visit.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", visit.PatientId);
            return View(visit);
        }

        [Authorize("Visits.Edit")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // POST: Visits/Delete/5
        [Authorize("Visits.Edit")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitExists(int id)
        {
            return _context.Visits.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        [Authorize("Doctors.View")]
        public IActionResult GetAll()
        {
            var allVisits = _context.Visits
                .Select(v => new
                {
                    v.Id,
                    patientName = v.Patient.FirstName + " " + v.Patient.LastName,
                    doctorName = v.Doctor.FirstName + " " + v.Doctor.LastName,
                    visitDate = v.VisitDate
                })
                .ToList();

            return Json(new { data = allVisits });
        }

        #endregion

    }
}
