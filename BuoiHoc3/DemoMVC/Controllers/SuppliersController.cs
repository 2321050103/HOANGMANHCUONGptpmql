using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? editId)
        {
            ViewBag.Supplier = editId == null
                ? new Supplier()
                : await _context.Suppliers.FindAsync(editId) ?? new Supplier();

            return View(await _context.Suppliers.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            var data = await _context.Suppliers
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    supplierName = s.SupplierName,
                    address = s.Address,
                    phone = s.Phone
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> AjaxSave([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (supplier.SupplierId == 0)
            {
                _context.Suppliers.Add(supplier);
            }
            else
            {
                var existingSupplier = await _context.Suppliers.FindAsync(supplier.SupplierId);
                if (existingSupplier == null)
                {
                    return NotFound();
                }

                existingSupplier.SupplierName = supplier.SupplierName;
                existingSupplier.Address = supplier.Address;
                existingSupplier.Phone = supplier.Phone;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AjaxDelete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Supplier = supplier;
                return View("Index", await _context.Suppliers.ToListAsync());
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
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
