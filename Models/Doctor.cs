using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Specialty { get; set; }

        public ICollection<Visit> Visits { get; set; }
    }
}