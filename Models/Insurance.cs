using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace A_Very_Simple_HIS.Models
{
    public class Insurance
    {
        public int Id { get; set; }

        public string ProviderName { get; set; }

        public string PlanName { get; set; }
        [ValidateNever]
        public ICollection<Patient> Patients { get; set; }
    }
}