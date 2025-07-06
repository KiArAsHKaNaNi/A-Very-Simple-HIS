using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters are allowed.")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int GenderId { get; set; }
        public Gender Gender { get; set; }

        public int InsuranceId { get; set; }
        public Insurance Insurance { get; set; }

        public ICollection<Visit> Visits { get; set; }
    }
}
