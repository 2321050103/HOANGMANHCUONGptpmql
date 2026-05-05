using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.Models.ViewModels
{
    public class InventoryVM
    {
        public string? Keyword { get; set; }
        public Supplier Supplier { get; set; } = new();
        public DeviceCategory Category { get; set; } = new();
        public Device Device { get; set; } = new();
        public ReceiptVM ImportForm { get; set; } = ReceiptVM.Create();
        public ReceiptVM ExportForm { get; set; } = ReceiptVM.Create();
        public List<Supplier> Suppliers { get; set; } = new();
        public List<DeviceCategory> Categories { get; set; } = new();
        public List<DeviceItemVM> Devices { get; set; } = new();
        public List<ReceiptItemVM> ImportReceipts { get; set; } = new();
        public List<ReceiptItemVM> ExportReceipts { get; set; } = new();
        public SelectList? SupplierOptions { get; set; }
        public SelectList? CategoryOptions { get; set; }
        public SelectList? DeviceOptions { get; set; }
    }

    public class DeviceItemVM
    {
        public int DeviceId { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ReceiptVM
    {
        public int ReceiptId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string? Note { get; set; }
        public List<ReceiptLineVM> Lines { get; set; } = new();

        public static ReceiptVM Create()
        {
            return new ReceiptVM
            {
                Lines = new List<ReceiptLineVM>
                {
                    new(),
                    new(),
                    new()
                }
            };
        }
    }

    public class ReceiptLineVM
    {
        public int DeviceId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ReceiptItemVM
    {
        public int ReceiptId { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public decimal Total { get; set; }
    }
}
