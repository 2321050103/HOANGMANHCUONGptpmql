using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, int? editId)
        {
            var query = _context.Devices
                .Include(d => d.DeviceCategory)
                .Include(d => d.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(d =>
                    d.DeviceCode.Contains(keyword) ||
                    d.DeviceName.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            var device = editId == null
                ? new Device()
                : await _context.Devices.FindAsync(editId) ?? new Device();
            ViewBag.Device = device;
            await LoadOptions(device.DeviceCategoryId, device.SupplierId);

            var data = await query.Select(d => new DeviceItemVM
            {
                DeviceId = d.DeviceId,
                DeviceCode = d.DeviceCode,
                DeviceName = d.DeviceName,
                CategoryName = d.DeviceCategory == null ? string.Empty : d.DeviceCategory.CategoryName,
                SupplierName = d.Supplier == null ? string.Empty : d.Supplier.SupplierName,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToListAsync();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Device device)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Device = device;
                await LoadOptions(device.DeviceCategoryId, device.SupplierId);
                return View("Index", new List<DeviceItemVM>());
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
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadOptions(int? categoryId = null, int? supplierId = null)
        {
            ViewBag.CategoryOptions = new SelectList(await _context.DeviceCategories.ToListAsync(), "DeviceCategoryId", "CategoryName", categoryId);
            ViewBag.SupplierOptions = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "SupplierName", supplierId);
        }
    }
}
