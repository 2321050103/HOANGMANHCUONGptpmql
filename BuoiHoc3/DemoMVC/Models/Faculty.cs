using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Faculty
    {
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "Tên khoa không được để trống")]
        public string FacultyName { get; set; } = string.Empty;

        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
