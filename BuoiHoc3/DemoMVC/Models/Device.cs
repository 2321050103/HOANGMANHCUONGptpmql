using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Device
    {
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "Mã thiết bị không được để trống")]
        [StringLength(30)]
        [Display(Name = "Mã thiết bị")]
        public string DeviceCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên thiết bị không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên thiết bị")]
        public string DeviceName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải >= 0")]
        [Display(Name = "Số Lượng tồn")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Don gia phai >= 0")]
        [Display(Name = "Don gia")]
        public decimal Price { get; set; }

        [Display(Name = "Loai thiet bi")]
        public int DeviceCategoryId { get; set; }
        public DeviceCategory? DeviceCategory { get; set; }

        [Display(Name = "Nha cung cap")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
