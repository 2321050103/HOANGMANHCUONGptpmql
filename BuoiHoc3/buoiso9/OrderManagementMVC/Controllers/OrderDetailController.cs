using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderManagementMVC.Data;
using OrderManagementMVC.Models;

namespace OrderManagementMVC.Controllers;

public class OrderDetailController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderDetailController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var orderDetails = await _context.OrderDetails
            .Include(od => od.Order)
            .ThenInclude(o => o!.Customer)
            .Include(od => od.Product)
            .ToListAsync();

        return View(orderDetails);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderDetail = await _context.OrderDetails
            .Include(od => od.Order)
            .ThenInclude(o => o!.Customer)
            .Include(od => od.Product)
            .FirstOrDefaultAsync(od => od.Id == id);

        if (orderDetail == null)
        {
            return NotFound();
        }

        return View(orderDetail);
    }

    public IActionResult Create()
    {
        PopulateDropDowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderDetail orderDetail)
    {
        if (!ModelState.IsValid)
        {
            PopulateDropDowns(orderDetail.OrderId, orderDetail.ProductId);
            return View(orderDetail);
        }

        _context.OrderDetails.Add(orderDetail);
        await _context.SaveChangesAsync();
        await UpdateOrderTotal(orderDetail.OrderId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail == null)
        {
            return NotFound();
        }

        PopulateDropDowns(orderDetail.OrderId, orderDetail.ProductId);
        return View(orderDetail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderDetail orderDetail)
    {
        if (id != orderDetail.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            PopulateDropDowns(orderDetail.OrderId, orderDetail.ProductId);
            return View(orderDetail);
        }

        var existingOrderDetail = await _context.OrderDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(od => od.Id == id);

        _context.Update(orderDetail);
        await _context.SaveChangesAsync();

        if (existingOrderDetail != null && existingOrderDetail.OrderId != orderDetail.OrderId)
        {
            await UpdateOrderTotal(existingOrderDetail.OrderId);
        }

        await UpdateOrderTotal(orderDetail.OrderId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderDetail = await _context.OrderDetails
            .Include(od => od.Order)
            .ThenInclude(o => o!.Customer)
            .Include(od => od.Product)
            .FirstOrDefaultAsync(od => od.Id == id);

        if (orderDetail == null)
        {
            return NotFound();
        }

        return View(orderDetail);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orderDetail = await _context.OrderDetails.FindAsync(id);
        if (orderDetail != null)
        {
            var orderId = orderDetail.OrderId;
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            await UpdateOrderTotal(orderId);
        }

        return RedirectToAction(nameof(Index));
    }

    private void PopulateDropDowns(object? selectedOrder = null, object? selectedProduct = null)
    {
        ViewBag.OrderId = new SelectList(
            _context.Orders.Include(o => o.Customer).OrderByDescending(o => o.Id)
                .Select(o => new { o.Id, DisplayText = $"Đơn #{o.Id} - {o.Customer!.Name}" }),
            "Id",
            "DisplayText",
            selectedOrder);

        ViewBag.ProductId = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name", selectedProduct);
    }

    private async Task UpdateOrderTotal(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return;
        }

        order.TotalAmount = await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .SumAsync(od => (double?)(od.Quantity * od.Price)) ?? 0;

        await _context.SaveChangesAsync();
    }
}
