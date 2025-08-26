namespace A_Very_Simple_HIS.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
