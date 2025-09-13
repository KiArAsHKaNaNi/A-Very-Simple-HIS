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
        public async Task<IActionResult> Create(DoctorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var doctor = new Doctor()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Specialty = vm.Specialty,
            };
            _context.Add(doctor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize("Doctors.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.Doctors.Where(x => x.Id == id).Select(d => new DoctorViewModel()
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                Specialty = d.Specialty,
            }).FirstOrDefaultAsync();

            return View(vm);
        }


        [Authorize("Doctors.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) 
            {
                return View(vm);
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            doctor.FirstName = vm.FirstName;
            doctor.LastName = vm.LastName;
            doctor.Specialty = vm.Specialty;

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

        [HttpPost]
        [Authorize("Doctors.Edit")]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _context.Doctors.Remove(objFromDb);
            await _context.SaveChangesAsync();
            return Json(new { success = true, data = "Delete Successful" });
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
