using A_Very_Simple_HIS.Attributes;
using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.ViewModels
{
    public class VisitViewModel
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        [FutureOrToday(ErrorMessage = "Visit date must be today or in the future.")]
        public DateTime VisitDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; }
    }
}
