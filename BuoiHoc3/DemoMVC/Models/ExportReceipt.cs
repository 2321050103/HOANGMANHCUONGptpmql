using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class ExportReceipt
    {
        public int ExportReceiptId { get; set; }

        [Required]
        [Display(Name = "Ngày xuất")]
        public DateTime ExportDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        public ICollection<ExportReceiptDetail> Details { get; set; } = new List<ExportReceiptDetail>();
    }
}