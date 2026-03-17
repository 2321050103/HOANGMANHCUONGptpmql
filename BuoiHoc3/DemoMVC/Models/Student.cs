using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tuổi không được để trống")]
        [Range(18, 60, ErrorMessage = "Tuổi phải từ 18 đến 60")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
    }
}