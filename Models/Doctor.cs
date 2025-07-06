using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace A_Very_Simple_HIS.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Specialty { get; set; }

        [ValidateNever]
        public ICollection<Visit> Visits { get; set; }
    }
}