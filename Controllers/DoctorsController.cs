using A_Very_Simple_HIS.Data;
using A_Very_Simple_HIS.Models;
using A_Very_Simple_HIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace A_Very_Simple_HIS.Controllers
{
    [Authorize]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize("Doctors.View")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize("Doctors.View")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

             var vm = await _context.Doctors
            .Where(d => d.Id == id)
            .Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Visits = d.Visits
                    .OrderByDescending(v => v.VisitDate)
                    .Select(v => new VisitViewModel
                    {
                        Id = v.Id,
                        PatientId = v.PatientId,
                        PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                        VisitDate = v.VisitDate,
                        VisitType = v.VisitType,
                        Notes = v.Notes
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
            if (vm == null)
            {
                return NotFound();
            }
             return View(vm);
        }

        [Authorize("Doctors.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize("Doctors.Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        [Authorize("Doctors.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }


        [Authorize("Doctors.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
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
            return View(doctor);
        }

        [Authorize("Doctors.Edit")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        [Authorize("Doctors.Edit")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        #region API Calls

        [HttpGet]
        [Authorize("Doctors.View")]
        public IActionResult GetAll()
        {
            var allDoctors = _context.Doctors
                .Select(d => new
                {
                    d.Id,
                    firstName = d.FirstName,
                    lastName = d.LastName,
                    specialty = d.Specialty,
                    visits = d.Visits.Count
                })
                .ToList();

            return Json(new { data = allDoctors });
        }
        #endregion


        //Helpers
        private static DoctorViewModel MapToViewModel(Doctor d)
        {

            return new DoctorViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Visits = d.Visits
                .OrderByDescending(v => v.VisitDate)
                .Select(v => new VisitViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    PatientName = v.Patient.FirstName + " " + v.Patient.LastName,
                    VisitDate = v.VisitDate,
                    VisitType = v.VisitType,
                    Notes = v.Notes
                })
                .ToList()};

        }
    }
}
