using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Numerics;

namespace A_Very_Simple_HIS.Models
{
    public class Visit
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        [ValidateNever]
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        [ValidateNever]
        public Doctor Doctor { get; set; }

        public DateTime VisitDate { get; set; }

        public string VisitType { get; set; } // "Inpatient" or "Outpatient"

        public string Notes { get; set; }
    }
}