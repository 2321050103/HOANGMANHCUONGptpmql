using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên nhà cung cấp")]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Số Điện Thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
