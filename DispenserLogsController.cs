using Dashboard.Data;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    public class DispenserLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DispenserLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // STEP 1: Verify staff
        [HttpGet]
        public IActionResult VerifyStaff(string dispenserId)
        {
            ViewBag.DispenserID = dispenserId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyStaff(string staffCode, string dispenserId)
        {
            if (string.IsNullOrEmpty(dispenserId))
                return NotFound("Dispenser ID is required.");

            var allowedCodes = new List<string> { "MJ", "ST-001", "AC-02", "CANINDO" };

            if (allowedCodes.Contains(staffCode.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("VerifiedStaffCode", staffCode);
                return RedirectToAction("LogForm", new { dispenserId });
            }

            TempData["Error"] = "Invalid staff code. Please try again.";
            ViewBag.DispenserID = dispenserId;
            return View("VerifyStaff");
        }

        // STEP 2: Show log form
        [HttpGet]
        public IActionResult LogForm(string dispenserId)
        {
            if (string.IsNullOrEmpty(dispenserId))
                return NotFound("Dispenser ID is required.");

            var dispenser = _context.Dispensers.FirstOrDefault(d => d.DispenserID == dispenserId);
            if (dispenser == null)
                return NotFound("Dispenser not found.");

            var code = HttpContext.Session.GetString("VerifiedStaffCode");
            if (string.IsNullOrEmpty(code))
                return RedirectToAction("VerifyStaff", new { dispenserId });

            ViewBag.Dispenser = dispenser;
            return View(new DispenserLog
            {
                DispenserID = dispenserId,
                StaffName = code
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitLog(DispenserLog log)
        {
            if (log.ActionTaken == "Replace" && string.IsNullOrEmpty(log.Volume))
            {
                ModelState.AddModelError("Volume", "Please select a volume when replacing.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Dispenser = _context.Dispensers.FirstOrDefault(d => d.DispenserID == log.DispenserID);
                return View("LogForm", log);
            }

            log.DateTime = DateTime.Now;

            _context.DispenserLogs.Add(log);
            _context.SaveChanges();

            TempData["Success"] = "Log entry submitted successfully!";
            TempData["DispenserID"] = log.DispenserID;

            HttpContext.Session.Remove("VerifiedStaffCode");

            return RedirectToAction("LogSuccess");
        }

        // STEP 3: Success Page
        public IActionResult LogSuccess()
        {
            ViewBag.DispenserID = TempData["DispenserID"];
            ViewBag.Message = TempData["Success"];
            return View();
        }
    }
}
