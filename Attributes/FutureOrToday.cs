using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.Attributes
{
    public class FutureOrToday : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date.Date >= DateTime.Today;
            }

            return true; 
        }
    }
}
