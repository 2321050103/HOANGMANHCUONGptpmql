using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class ImportReceipt
    {
        public int ImportReceiptId { get; set; }

        [Required]
        [Display(Name = "Ngày nhập")]
        public DateTime ImportDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Ghi  chú")]
        public string? Note { get; set; }

        public ICollection<ImportReceiptDetail> Details { get; set; } = new List<ImportReceiptDetail>();
    }
}
