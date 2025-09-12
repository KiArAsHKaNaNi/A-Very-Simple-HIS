using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.ViewModels
{
    public class DoctorViewModel
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }


        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters are allowed.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string Specialty { get; set; }

        [ValidateNever]
        public ICollection<VisitViewModel> Visits { get; set; } = new List<VisitViewModel>();
    }
}
