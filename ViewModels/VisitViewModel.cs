using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace A_Very_Simple_HIS.ViewModels
{
    public class VisitViewModel
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime VisitDate { get; set; }
        public string Notes { get; set; }
    }
}
