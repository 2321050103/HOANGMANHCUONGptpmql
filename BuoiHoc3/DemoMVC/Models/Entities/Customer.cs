using System.ComponentModel.DataAnnotations;
namespace DemoMVC.Models.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không được để trống")]
        [StringLength(100)]
        public string CustomerName { get; set; } = default!;
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress]
        public string Email { get; set; } = default!;
        // Navigation property
        public ICollection<Order>? Orders { get; set; }
    }
}