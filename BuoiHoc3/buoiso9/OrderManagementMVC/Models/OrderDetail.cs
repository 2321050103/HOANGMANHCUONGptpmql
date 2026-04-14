using System.ComponentModel.DataAnnotations;

namespace OrderManagementMVC.Models;

public class OrderDetail
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0.")]
    [Display(Name = "Đơn giá")]
    public double Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đơn hàng.")]
    [Display(Name = "Đơn hàng")]
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn sản phẩm.")]
    [Display(Name = "Sản phẩm")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
