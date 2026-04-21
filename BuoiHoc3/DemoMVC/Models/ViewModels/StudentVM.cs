namespace DemoMVC.Models.ViewModels
{
    public class StudentVM
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
    }
}
