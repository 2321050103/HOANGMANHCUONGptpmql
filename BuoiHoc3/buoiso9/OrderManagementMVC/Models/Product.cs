using System.ComponentModel.DataAnnotations;

namespace OrderManagementMVC.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm tối đa 100 ký tự.")]
    [Display(Name = "Tên sản phẩm")]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
    [Display(Name = "Giá bán")]
    public double Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm.")]
    [Display(Name = "Tồn kho")]
    public int Stock { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = [];
}
