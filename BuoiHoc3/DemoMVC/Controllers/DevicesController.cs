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

        [HttpGet]
        public async Task<IActionResult> GetDevices(string? keyword)
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

            var data = await query
                .Select(d => new
                {
                    deviceId = d.DeviceId,
                    deviceCode = d.DeviceCode,
                    deviceName = d.DeviceName,
                    deviceCategoryId = d.DeviceCategoryId,
                    categoryName = d.DeviceCategory == null ? string.Empty : d.DeviceCategory.CategoryName,
                    supplierId = d.SupplierId,
                    supplierName = d.Supplier == null ? string.Empty : d.Supplier.SupplierName,
                    quantity = d.Quantity,
                    price = d.Price
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetOptions()
        {
            var categories = await _context.DeviceCategories
                .Select(c => new
                {
                    deviceCategoryId = c.DeviceCategoryId,
                    categoryName = c.CategoryName
                })
                .ToListAsync();

            var suppliers = await _context.Suppliers
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    supplierName = s.SupplierName
                })
                .ToListAsync();

            return Json(new { categories, suppliers });
        }

        [HttpPost]
        public async Task<IActionResult> AjaxSave([FromBody] Device device)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (device.DeviceId == 0)
            {
                _context.Devices.Add(device);
            }
            else
            {
                var existingDevice = await _context.Devices.FindAsync(device.DeviceId);
                if (existingDevice == null)
                {
                    return NotFound();
                }

                existingDevice.DeviceCode = device.DeviceCode;
                existingDevice.DeviceName = device.DeviceName;
                existingDevice.Quantity = device.Quantity;
                existingDevice.Price = device.Price;
                existingDevice.DeviceCategoryId = device.DeviceCategoryId;
                existingDevice.SupplierId = device.SupplierId;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AjaxDelete(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
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
