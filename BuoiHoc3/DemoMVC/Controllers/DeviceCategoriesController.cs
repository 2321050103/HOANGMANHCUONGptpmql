using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class DeviceCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, int? editId)
        {
            var query = _context.DeviceCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c => c.CategoryName.Contains(keyword));
            }

            ViewBag.Keyword = keyword;
            ViewBag.Category = editId == null
                ? new DeviceCategory()
                : await _context.DeviceCategories.FindAsync(editId) ?? new DeviceCategory();

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(string? keyword)
        {
            var query = _context.DeviceCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c => c.CategoryName.Contains(keyword));
            }

            var data = await query
                .Select(c => new
                {
                    deviceCategoryId = c.DeviceCategoryId,
                    categoryName = c.CategoryName
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> AjaxSave([FromBody] DeviceCategory category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (category.DeviceCategoryId == 0)
            {
                _context.DeviceCategories.Add(category);
            }
            else
            {
                var existingCategory = await _context.DeviceCategories.FindAsync(category.DeviceCategoryId);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.CategoryName = category.CategoryName;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AjaxDelete(int id)
        {
            var category = await _context.DeviceCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.DeviceCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DeviceCategory category)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Category = category;
                return View("Index", await _context.DeviceCategories.ToListAsync());
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
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.DeviceCategories.FindAsync(id);
            if (category != null)
            {
                _context.DeviceCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
