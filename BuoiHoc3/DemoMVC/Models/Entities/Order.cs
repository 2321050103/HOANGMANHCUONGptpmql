using System.ComponentModel.DataAnnotations;
namespace DemoMVC.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Ngày đặt hàng không được để trống")]
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "ID khách hàng không được để trống")]
        public int CustomerId { get; set; }
        // Navigation
        public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}