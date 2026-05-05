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
