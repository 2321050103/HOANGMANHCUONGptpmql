using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class ExportReceiptDetail
    {
        public int ExportReceiptDetailId { get; set; }

        public int ExportReceiptId { get; set; }
        public ExportReceipt? ExportReceipt { get; set; }

        [Display(Name = "Thiết bị")]
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Don gia xuat phai >=0")]
        [Display(Name = "Don gia xuat")]
        public decimal ExportPrice { get; set; }

        [NotMapped]
        [Display(Name = "Thanh tien")]
        public decimal Total => Quantity * ExportPrice;
    }
}
