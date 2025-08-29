using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace A_Very_Simple_HIS.ViewModels
{
    public class UserViewModel
    {

        public string Id { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
        public string SelectedRole{ get; set; }
    }
}
