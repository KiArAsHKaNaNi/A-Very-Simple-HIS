using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters are allowed.")]

        public string FullName { get; set; }


        public string Specialty { get; set; }

        [ValidateNever]
        public ICollection<Visit> Visits { get; set; }
    }
}