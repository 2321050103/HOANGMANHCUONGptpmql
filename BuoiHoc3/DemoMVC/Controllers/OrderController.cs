using DemoMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.ViewModels;
namespace DemoMVC.Controllers
{
    public class OrderController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        // GET: Product
        public async Task<IActionResult> Index()
        {
            var data = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            return View(data);
        }
        public IActionResult Create()
        {
            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Products = _context.Products.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrderVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _context.Customers.ToList();
                ViewBag.Products = _context.Products.ToList();
                return View(model);
            }

            var order = new Order
            {
                OrderDate = model.OrderDate,
                CustomerId = model.CustomerId,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in model.Details)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null) continue;

                // trừ tồn kho
                if (product.Stock < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm {product.ProductName} không đủ hàng");
                    return View(model);
                }

                product.Stock -= item.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return View(order);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order != null)
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails!);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}