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
using System.Threading.Tasks;

namespace A_Very_Simple_HIS.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize("Patients.View")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize("Patients.View")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Gender)
                .Include(p => p.Insurance)
                .Include(p => p.Visits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            var vm = MapToViewModel(patient);
            return View(vm);
        }

        [Authorize("Patients.Create")]
        public IActionResult Create()
        {
            PopulateSelectLists();
            var vm = new PatientViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-20) 
            };
            return View(vm);
        }

        [Authorize("Patients.Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists();
                return View(vm);
            }

            var patient = new Patient
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DateOfBirth = vm.DateOfBirth,
                GenderId = vm.GenderId,
                InsuranceId = vm.InsuranceId
            };

            _context.Add(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize("Patients.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
            .Include(p => p.Gender)
            .Include(p => p.Insurance)
            .Include(p => p.Visits)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();

            var vm = MapToViewModel(patient);
            PopulateSelectLists(patient.GenderId, patient.InsuranceId);
            return View(vm);
        }

        [Authorize("Patients.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateSelectLists(vm.GenderId, vm.InsuranceId);
                return View(vm);
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            patient.FirstName = vm.FirstName;
            patient.LastName = vm.LastName;
            patient.DateOfBirth = vm.DateOfBirth;
            patient.GenderId = vm.GenderId;
            patient.InsuranceId = vm.InsuranceId;

            try
            {
                _context.Update(patient);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(patient.Id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize("Patients.Edit")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.Gender)
                .Include(p => p.Insurance)
                .Include(p => p.Visits)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null) return NotFound();

            var vm = MapToViewModel(patient);
            return View(vm);
        }

        [Authorize("Patients.Edit")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        #region API Calls
 
        [HttpGet]
        [Authorize("Patients.View")]
        public IActionResult GetAll()
        {
            var allPatients = _context.Patients
                .Include(p => p.Gender)
                .Include(p => p.Insurance)
                .Select(p => new
                {
                    p.Id,
                    firstName = p.FirstName,
                    lastName = p.LastName,
                    dateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                    gender = p.Gender != null ? p.Gender.Name : "",
                    insurance = p.Insurance != null ? p.Insurance.ProviderName : ""
                })
                .ToList();

            return Json(new { data = allPatients });
        }
        #endregion

        private static PatientViewModel MapToViewModel(Patient p)
        {
            return new PatientViewModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                GenderName = p.Gender?.Name,
                InsuranceName = p.Insurance?.ProviderName,
                VisitsCount = p.Visits.Count
            };
        }

        private void PopulateSelectLists(int? selectedGenderId = null, int? selectedInsuranceId = null)
        {
            ViewData["GenderId"] = new SelectList(_context.Genders.AsNoTracking().ToList(), "Id", "Name", selectedGenderId);
            ViewData["InsuranceId"] = new SelectList(_context.Insurances.AsNoTracking().ToList(), "Id", "ProviderName", selectedInsuranceId);
        }

    }
}
