using Dashboard.Data;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Linq;
using System.Net;

namespace Dashboard.Controllers
{
    public class DispenserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DispenserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var units = _context.Units
                .Select(u => new { u.UnitID, u.UnitName })
                .ToList();

            ViewBag.UnitMap = System.Text.Json.JsonSerializer.Serialize(units);
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dispenser dispenser)
        {
            if (_context.Dispensers.Any(d => d.DispenserID == dispenser.DispenserID))
            {
                TempData["Error"] = "Dispenser already exists.";
                ViewBag.UnitList = _context.Units
                    .Select(u => new SelectListItem
                    {
                        Value = u.UnitID.ToString(),
                        Text = u.UnitName
                    })
                    .ToList();

                return View(dispenser);
            }

            dispenser.IsActive = true;
            dispenser.DateAdded = DateTime.Now;
            dispenser.LastRefill ="Not Checked yet";

            _context.Dispensers.Add(dispenser);
            await _context.SaveChangesAsync();

            string ip = "localhost"; // fallback
            try
            {
                ip = Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList
                        .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                        .ToString() ?? "localhost";
            }
            catch { /* Log if needed */ }

            var port = Request.Host.Port ?? 5000;
            string url = $"https://{ip}:{port}/DispenserLogs/LogForm?dispenserId={dispenser.DispenserID}";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrCodeData);
            string qrBase64 = qrCode.GetGraphic(20);

            dispenser.QRCodeURL = url;
            dispenser.QRCodeImageBase64 = qrBase64;
            _context.Update(dispenser);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("QRCodeImage", qrBase64);
            HttpContext.Session.SetString("GeneratedDispenserID", dispenser.DispenserID);

            TempData["Success"] = "Dispenser created successfully!";
            return RedirectToAction("QRGenerated");
        }

        [HttpGet]
        public IActionResult QRGenerated()
        {
            var qrBase64 = HttpContext.Session.GetString("QRCodeImage");
            var dispenserId = HttpContext.Session.GetString("GeneratedDispenserID");

            if (string.IsNullOrEmpty(qrBase64) || string.IsNullOrEmpty(dispenserId))
            {
                TempData["Error"] = "QR Code generation failed or session expired.";
                return RedirectToAction("Create");
            }

            var dispenser = _context.Dispensers
                .Include(d => d.Unit)
                .FirstOrDefault(d => d.DispenserID == dispenserId);

            if (dispenser == null)
            {
                TempData["Error"] = "Dispenser not found in the database.";
                return RedirectToAction("Create");
            }

            ViewBag.Dispenser = dispenser;
            return View();
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var dispenser = _context.Dispensers
                .Include(d => d.Unit)
                .FirstOrDefault(d => d.DispenserID == id);

            if (dispenser == null)
                return NotFound();

            // Generate QR URL if missing
            if (string.IsNullOrEmpty(dispenser.QRCodeURL))
            {
                try
                {
                    var ip = Dns.GetHostEntry(Dns.GetHostName())
                                .AddressList
                                .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                                .ToString() ?? "localhost";

                    var port = Request.Host.Port ?? 5000;
                    string url = $"https://{ip}:{port}/DispenserLogs/LogForm?dispenserId={dispenser.DispenserID}";
                    dispenser.QRCodeURL = url;
                    _context.Update(dispenser);
                    _context.SaveChanges();
                }
                catch { /* fallback silently */ }
            }

            return PartialView("_QRModalPartial", dispenser);
        }
    }
}