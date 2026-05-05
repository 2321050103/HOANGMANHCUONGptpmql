using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class DeviceCategory
    {
        public int DeviceCategoryId { get; set; }

        [Required(ErrorMessage = "Tên loại thiết bị không được để trống")]
        [StringLength(100)]
        [Display(Name = "Loại thiết bị")]
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
