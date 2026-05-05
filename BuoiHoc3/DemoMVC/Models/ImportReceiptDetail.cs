using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class ImportReceiptDetail
    {
        public int ImportReceiptDetailId { get; set; }

        public int ImportReceiptId { get; set; }
        public ImportReceipt? ImportReceipt { get; set; }

        [Display(Name = "Thiết bị")]
        public int DeviceId { get; set; }
        public Device? Device { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Don gia nhap phai >= 0")]
        [Display(Name = "Don gia nhap")]
        public decimal ImportPrice { get; set; }

        [NotMapped]
        [Display(Name = "Thanh tien")]
        public decimal Total => Quantity * ImportPrice;
    }
}
