using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword)
        {
            return View(await BuildViewModel(keyword));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildViewModel());
            }

            if (supplier.SupplierId == 0)
            {
                _context.Suppliers.Add(supplier);
            }
            else
            {
                _context.Suppliers.Update(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCategory(DeviceCategory category)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildViewModel());
            }

            if (category.DeviceCategoryId == 0)
            {
                _context.DeviceCategories.Add(category);
            }
            else
            {
                _context.DeviceCategories.Update(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.DeviceCategories.FindAsync(id);
            if (category != null)
            {
                _context.DeviceCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDevice(Device device)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await BuildViewModel());
            }

            if (device.DeviceId == 0)
            {
                _context.Devices.Add(device);
            }
            else
            {
                _context.Devices.Update(device);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ReceiptVM importForm)
        {
            var lines = importForm.Lines
                .Where(l => l.DeviceId > 0 && l.Quantity > 0)
                .ToList();

            if (!lines.Any())
            {
                ModelState.AddModelError(string.Empty, "Nhap it nhat mot thiet bi.");
                return View("Index", await BuildViewModel());
            }

            var receipt = new ImportReceipt
            {
                ImportDate = importForm.Date,
                Note = importForm.Note
            };

            foreach (var line in lines)
            {
                var device = await _context.Devices.FindAsync(line.DeviceId);
                if (device == null)
                {
                    return NotFound();
                }

                device.Quantity += line.Quantity;
                receipt.Details.Add(new ImportReceiptDetail
                {
                    DeviceId = line.DeviceId,
                    Quantity = line.Quantity,
                    ImportPrice = line.Price
                });
            }

            _context.ImportReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export(ReceiptVM exportForm)
        {
            var lines = exportForm.Lines
                .Where(l => l.DeviceId > 0 && l.Quantity > 0)
                .ToList();

            if (!lines.Any())
            {
                ModelState.AddModelError(string.Empty, "Nhap it nhat mot thiet bi.");
                return View("Index", await BuildViewModel());
            }

            foreach (var line in lines)
            {
                var device = await _context.Devices.FindAsync(line.DeviceId);
                if (device == null)
                {
                    return NotFound();
                }

                if (device.Quantity < line.Quantity)
                {
                    ModelState.AddModelError(string.Empty, $"Thiet bi {device.DeviceName} khong du so luong ton.");
                    return View("Index", await BuildViewModel());
                }
            }

            var receipt = new ExportReceipt
            {
                ExportDate = exportForm.Date,
                Note = exportForm.Note
            };

            foreach (var line in lines)
            {
                var device = await _context.Devices.FindAsync(line.DeviceId);
                device!.Quantity -= line.Quantity;
                receipt.Details.Add(new ExportReceiptDetail
                {
                    DeviceId = line.DeviceId,
                    Quantity = line.Quantity,
                    ExportPrice = line.Price
                });
            }

            _context.ExportReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<InventoryVM> BuildViewModel(string? keyword = null)
        {
            var deviceQuery = _context.Devices
                .Include(d => d.DeviceCategory)
                .Include(d => d.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                deviceQuery = deviceQuery.Where(d =>
                    d.DeviceCode.Contains(keyword) ||
                    d.DeviceName.Contains(keyword) ||
                    d.DeviceCategory!.CategoryName.Contains(keyword));
            }

            var suppliers = await _context.Suppliers.ToListAsync();
            var categories = await _context.DeviceCategories.ToListAsync();
            var devices = await deviceQuery
                .Select(d => new DeviceItemVM
                {
                    DeviceId = d.DeviceId,
                    DeviceCode = d.DeviceCode,
                    DeviceName = d.DeviceName,
                    CategoryName = d.DeviceCategory == null ? string.Empty : d.DeviceCategory.CategoryName,
                    SupplierName = d.Supplier == null ? string.Empty : d.Supplier.SupplierName,
                    Quantity = d.Quantity,
                    Price = d.Price
                })
                .ToListAsync();

            return new InventoryVM
            {
                Keyword = keyword,
                Suppliers = suppliers,
                Categories = categories,
                Devices = devices,
                ImportReceipts = await _context.ImportReceipts
                    .Include(r => r.Details)
                    .Select(r => new ReceiptItemVM
                    {
                        ReceiptId = r.ImportReceiptId,
                        Date = r.ImportDate,
                        Note = r.Note,
                        Total = r.Details.Sum(d => d.Quantity * d.ImportPrice)
                    })
                    .ToListAsync(),
                ExportReceipts = await _context.ExportReceipts
                    .Include(r => r.Details)
                    .Select(r => new ReceiptItemVM
                    {
                        ReceiptId = r.ExportReceiptId,
                        Date = r.ExportDate,
                        Note = r.Note,
                        Total = r.Details.Sum(d => d.Quantity * d.ExportPrice)
                    })
                    .ToListAsync(),
                SupplierOptions = new SelectList(suppliers, "SupplierId", "SupplierName"),
                CategoryOptions = new SelectList(categories, "DeviceCategoryId", "CategoryName"),
                DeviceOptions = new SelectList(await _context.Devices.ToListAsync(), "DeviceId", "DeviceName")
            };
        }
    }
}
