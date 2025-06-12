namespace A_Very_Simple_HIS.Models
{
    public class Gender
    {
        public int Id { get; set; }

        public string Name { get; set; }  // Example: "Male", "Female", "Other"

        public ICollection<Patient> Patients { get; set; }
    }
}