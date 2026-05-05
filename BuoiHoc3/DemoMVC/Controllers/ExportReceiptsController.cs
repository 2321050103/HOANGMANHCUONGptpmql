using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class ExportReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportReceiptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? editId)
        {
            return View(await BuildViewModel(null, editId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptVM exportForm)
        {
            var lines = exportForm.Lines.Where(l => l.DeviceId > 0 && l.Quantity > 0).ToList();

            if (!lines.Any())
            {
                ModelState.AddModelError(string.Empty, "Nhap it nhat mot thiet bi.");
                return View("Index", await BuildViewModel(exportForm));
            }

            if (exportForm.ReceiptId != 0)
            {
                var oldReceipt = await _context.ExportReceipts
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.ExportReceiptId == exportForm.ReceiptId);

                if (oldReceipt != null)
                {
                    foreach (var oldLine in oldReceipt.Details)
                    {
                        var oldDevice = await _context.Devices.FindAsync(oldLine.DeviceId);
                        if (oldDevice != null)
                        {
                            oldDevice.Quantity += oldLine.Quantity;
                        }
                    }
                }
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
                    return View("Index", await BuildViewModel(exportForm));
                }
            }

            ExportReceipt receipt;
            if (exportForm.ReceiptId == 0)
            {
                receipt = new ExportReceipt();
                _context.ExportReceipts.Add(receipt);
            }
            else
            {
                receipt = await _context.ExportReceipts
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.ExportReceiptId == exportForm.ReceiptId) ?? new ExportReceipt();

                _context.ExportReceiptDetails.RemoveRange(receipt.Details);
            }

            receipt.ExportDate = exportForm.Date;
            receipt.Note = exportForm.Note;
            receipt.Details = new List<ExportReceiptDetail>();

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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var receipt = await _context.ExportReceipts
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.ExportReceiptId == id);

            if (receipt != null)
            {
                foreach (var line in receipt.Details)
                {
                    var device = await _context.Devices.FindAsync(line.DeviceId);
                    if (device != null)
                    {
                        device.Quantity += line.Quantity;
                    }
                }

                _context.ExportReceipts.Remove(receipt);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<InventoryVM> BuildViewModel(ReceiptVM? form = null, int? editId = null)
        {
            if (editId != null)
            {
                var receipt = await _context.ExportReceipts
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.ExportReceiptId == editId);

                if (receipt != null)
                {
                    form = new ReceiptVM
                    {
                        ReceiptId = receipt.ExportReceiptId,
                        Date = receipt.ExportDate,
                        Note = receipt.Note,
                        Lines = receipt.Details
                            .Select(d => new ReceiptLineVM
                            {
                                DeviceId = d.DeviceId,
                                Quantity = d.Quantity,
                                Price = d.ExportPrice
                            })
                            .ToList()
                    };

                    while (form.Lines.Count < 3)
                    {
                        form.Lines.Add(new ReceiptLineVM());
                    }
                }
            }

            return new InventoryVM
            {
                ExportForm = form ?? ReceiptVM.Create(),
                DeviceOptions = new SelectList(await _context.Devices.ToListAsync(), "DeviceId", "DeviceName"),
                ExportReceipts = await _context.ExportReceipts
                    .Include(r => r.Details)
                    .OrderByDescending(r => r.ExportDate)
                    .Select(r => new ReceiptItemVM
                    {
                        ReceiptId = r.ExportReceiptId,
                        Date = r.ExportDate,
                        Note = r.Note,
                        Total = r.Details.Sum(d => d.Quantity * d.ExportPrice)
                    })
                    .ToListAsync()
            };
        }
    }
}
