using Microsoft.AspNetCore.Identity;

namespace A_Very_Simple_HIS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
