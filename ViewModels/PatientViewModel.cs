using A_Very_Simple_HIS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.ViewModels
{
    public class PatientViewModel
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

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public int GenderId { get; set; }

        [Display(Name = "Gender")]
        public string GenderName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Insurance")]
        public int InsuranceId { get; set; }

        [Display(Name = "Insurance")]
        public string InsuranceName { get; set; } = string.Empty; 

        public int VisitsCount { get; set; }
    }
}

