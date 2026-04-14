using System.ComponentModel.DataAnnotations;

namespace OrderManagementMVC.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên khách hàng tối đa 100 ký tự.")]
    [Display(Name = "Tên khách hàng")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(150, ErrorMessage = "Email tối đa 150 ký tự.")]
    public string Email { get; set; } = string.Empty;

    public List<Order> Orders { get; set; } = [];
}
