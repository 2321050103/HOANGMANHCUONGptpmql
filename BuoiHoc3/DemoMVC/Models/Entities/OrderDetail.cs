using System.ComponentModel.DataAnnotations;
namespace DemoMVC.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0")]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        // Navigation
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}