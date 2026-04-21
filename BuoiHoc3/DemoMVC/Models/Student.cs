using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20, ErrorMessage = "Mã sinh viên tối đa 20 ký tự")]
        [Display(Name = "Mã sinh viên")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự")]
        [Display(Name = "Họ tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tuổi không được để trống")]
        [Range(18, 60, ErrorMessage = "Tuổi phải từ 18 đến 60")]
        [Display(Name = "Tuổi")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khoa")]
        [Display(Name = "Khoa")]
        public int FacultyId { get; set; }

        [ForeignKey("FacultyId")]
        public virtual Faculty? Faculty { get; set; }
    }
}
