using A_Very_Simple_HIS.Data;
using A_Very_Simple_HIS.Models;
using A_Very_Simple_HIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

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
            PopulateSelectLists();
            var vm = new VisitViewModel()
            {
                VisitDate = DateTime.Now
            };
            return View(vm);
        }

        [Authorize("Visits.Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return View(vm);
            }

            var visit = new Visit
            {
                PatientId = vm.PatientId,
                DoctorId = vm.DoctorId,
                VisitDate = vm.VisitDate,
                Notes = vm.Notes,
            };

            _context.Add(visit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize("Visits.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits.Include(v => v.Patient).Include(v => v.Doctor).FirstOrDefaultAsync();
            if (visit == null)
            {
                return NotFound();
            }
            var vm = new VisitViewModel()
            {
                Id = visit.Id,
                VisitDate = visit.VisitDate,
                PatientName = visit.Patient?.FirstName + " " + visit.Patient?.LastName,
                DoctorName = visit.Doctor?.FirstName + " " + visit.Doctor?.LastName,
                Notes = visit.Notes,
            };
            PopulateSelectLists();
            return View(vm);
        }

        [Authorize("Visits.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return View(vm);
            }

            var visit = await _context.Visits.FindAsync(id);
            if (visit == null) return NotFound();

            visit.PatientId = vm.PatientId;
            visit.DoctorId = vm.DoctorId;
            visit.VisitDate = vm.VisitDate;
            visit.Notes = vm.Notes;

            try
            {
                _context.Update(visit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisitExists(visit.Id)) return NotFound();
                else throw;
            }

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


        [HttpPost]
        [Authorize("Patients.Edit")]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _context.Visits.FirstOrDefaultAsync(v => v.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _context.Visits.Remove(objFromDb);
            await _context.SaveChangesAsync();
            return Json(new { success = true, data = "Delete Successful" });
        }

        #endregion

        //Helpers
        private void PopulateSelectLists(int? selectedPatientId = null, int? selectedDoctorId = null)
        {
            var doctors = _context.Doctors
            .AsNoTracking()
            .Select(d => new
            {
                d.Id,
                FullName = d.FirstName + " " + d.LastName, 
            })
            .ToList();

                var patients = _context.Patients
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.Id,
                        FullName = p.FirstName + " " + p.LastName
                    })
                    .ToList();

                ViewData["DoctorId"] = new SelectList(doctors, "Id", "FullName", selectedDoctorId);
                ViewData["PatientId"] = new SelectList(patients, "Id", "FullName", selectedPatientId);
        }

    }
}
