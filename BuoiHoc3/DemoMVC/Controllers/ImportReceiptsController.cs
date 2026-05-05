using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class ImportReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImportReceiptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? editId)
        {
            return View(await BuildViewModel(null, editId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptVM importForm)
        {
            var lines = importForm.Lines.Where(l => l.DeviceId > 0 && l.Quantity > 0).ToList();

            if (!lines.Any())
            {
                ModelState.AddModelError(string.Empty, "Nhap it nhat mot thiet bi.");
                return View("Index", await BuildViewModel(importForm));
            }

            ImportReceipt receipt;
            if (importForm.ReceiptId == 0)
            {
                receipt = new ImportReceipt();
                _context.ImportReceipts.Add(receipt);
            }
            else
            {
                receipt = await _context.ImportReceipts
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.ImportReceiptId == importForm.ReceiptId) ?? new ImportReceipt();

                foreach (var oldLine in receipt.Details)
                {
                    var oldDevice = await _context.Devices.FindAsync(oldLine.DeviceId);
                    if (oldDevice != null)
                    {
                        oldDevice.Quantity -= oldLine.Quantity;
                    }
                }

                _context.ImportReceiptDetails.RemoveRange(receipt.Details);
            }

            receipt.ImportDate = importForm.Date;
            receipt.Note = importForm.Note;
            receipt.Details = new List<ImportReceiptDetail>();

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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var receipt = await _context.ImportReceipts
                .Include(r => r.Details)
                .FirstOrDefaultAsync(r => r.ImportReceiptId == id);

            if (receipt != null)
            {
                foreach (var line in receipt.Details)
                {
                    var device = await _context.Devices.FindAsync(line.DeviceId);
                    if (device != null)
                    {
                        device.Quantity -= line.Quantity;
                    }
                }

                _context.ImportReceipts.Remove(receipt);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<InventoryVM> BuildViewModel(ReceiptVM? form = null, int? editId = null)
        {
            if (editId != null)
            {
                var receipt = await _context.ImportReceipts
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.ImportReceiptId == editId);

                if (receipt != null)
                {
                    form = new ReceiptVM
                    {
                        ReceiptId = receipt.ImportReceiptId,
                        Date = receipt.ImportDate,
                        Note = receipt.Note,
                        Lines = receipt.Details
                            .Select(d => new ReceiptLineVM
                            {
                                DeviceId = d.DeviceId,
                                Quantity = d.Quantity,
                                Price = d.ImportPrice
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
                ImportForm = form ?? ReceiptVM.Create(),
                DeviceOptions = new SelectList(await _context.Devices.ToListAsync(), "DeviceId", "DeviceName"),
                ImportReceipts = await _context.ImportReceipts
                    .Include(r => r.Details)
                    .OrderByDescending(r => r.ImportDate)
                    .Select(r => new ReceiptItemVM
                    {
                        ReceiptId = r.ImportReceiptId,
                        Date = r.ImportDate,
                        Note = r.Note,
                        Total = r.Details.Sum(d => d.Quantity * d.ImportPrice)
                    })
                    .ToListAsync()
            };
        }
    }
}
