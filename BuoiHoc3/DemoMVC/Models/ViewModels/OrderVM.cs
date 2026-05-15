using System.ComponentModel.DataAnnotations;
namespace DemoMVC.Models.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public List<OrderDetailVM> Details { get; set; } = new();
    }
}