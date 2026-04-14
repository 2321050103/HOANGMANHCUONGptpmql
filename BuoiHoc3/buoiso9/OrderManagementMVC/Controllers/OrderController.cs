using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderManagementMVC.Data;
using OrderManagementMVC.Models;

namespace OrderManagementMVC.Controllers;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    public IActionResult Create()
    {
        PopulateCustomersDropDownList();
        return View(new Order { OrderDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        order.TotalAmount = 0;

        if (!ModelState.IsValid)
        {
            PopulateCustomersDropDownList(order.CustomerId);
            return View(order);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        PopulateCustomersDropDownList(order.CustomerId);
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        order.TotalAmount = await _context.OrderDetails
            .Where(od => od.OrderId == id)
            .SumAsync(od => (double?)(od.Quantity * od.Price)) ?? 0;

        if (!ModelState.IsValid)
        {
            PopulateCustomersDropDownList(order.CustomerId);
            return View(order);
        }

        _context.Update(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> OrdersByCustomer(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        ViewBag.CustomerName = customer.Name;

        var orders = await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    private void PopulateCustomersDropDownList(object? selectedCustomer = null)
    {
        ViewBag.CustomerId = new SelectList(_context.Customers.OrderBy(c => c.Name), "Id", "Name", selectedCustomer);
    }
}
