using System.ComponentModel.DataAnnotations;

namespace OrderManagementMVC.Models;

public class Order
{
    public int Id { get; set; }

    [Display(Name = "Ngày đặt hàng")]
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không được âm.")]
    [Display(Name = "Tổng tiền")]
    public double TotalAmount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khách hàng.")]
    [Display(Name = "Khách hàng")]
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = [];
}
