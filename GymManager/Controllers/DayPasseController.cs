
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    public class DayPasseController : Controller
    {
        private readonly GymDbContext _context;

        public DayPasseController(GymDbContext context)
        {
            _context = context;
        }

        // GET: DayPasse
        public async Task<IActionResult> Index()
        {
            var dayPasses = await _context.DayPasses
                .OrderByDescending(d => d.VisitDateTime)
                .ToListAsync();

            return View(dayPasses);
        }

        // GET: DayPasse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var dayPass = await _context.DayPasses
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dayPass == null)
                return NotFound();

            return View(dayPass);
        }

        // GET: DayPasse/Create
        public IActionResult Create()
        {
            return View(new DayPass
            {
                VisitDateTime = DateTime.Now,
                PaymentMethod = "Cash"
            });
        }

        // POST: DayPasse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("VisitorName,VisitorPhone,VisitDateTime,Fee,PaymentMethod,Notes")]
            DayPass dayPass)
        {
            ValidatePaymentMethod(dayPass.PaymentMethod);

            if (!ModelState.IsValid)
                return View(dayPass);

            _context.DayPasses.Add(dayPass);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Day pass created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DayPasse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var dayPass = await _context.DayPasses.FindAsync(id);

            if (dayPass == null)
                return NotFound();

            return View(dayPass);
        }

        // POST: DayPasse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,VisitorName,VisitorPhone,VisitDateTime,Fee,PaymentMethod,Notes")]
            DayPass dayPass)
        {
            if (id != dayPass.Id)
                return NotFound();

            ValidatePaymentMethod(dayPass.PaymentMethod);

            if (!ModelState.IsValid)
                return View(dayPass);

            var existingDayPass = await _context.DayPasses.FindAsync(id);

            if (existingDayPass == null)
                return NotFound();

            // Update only the fields that can be edited.
            existingDayPass.VisitorName = dayPass.VisitorName;
            existingDayPass.VisitorPhone = dayPass.VisitorPhone;
            existingDayPass.VisitDateTime = dayPass.VisitDateTime;
            existingDayPass.Fee = dayPass.Fee;
            existingDayPass.PaymentMethod = dayPass.PaymentMethod;
            existingDayPass.Notes = dayPass.Notes;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Day pass updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: DayPasse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var dayPass = await _context.DayPasses
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dayPass == null)
                return NotFound();

            return View(dayPass);
        }

        // POST: DayPasse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dayPass = await _context.DayPasses.FindAsync(id);

            if (dayPass == null)
                return NotFound();

            _context.DayPasses.Remove(dayPass);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Day pass deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void ValidatePaymentMethod(string? paymentMethod)
        {
            var allowedMethods = new[] { "Cash", "Card", "Wallet" };

            if (string.IsNullOrWhiteSpace(paymentMethod) ||
                !allowedMethods.Contains(paymentMethod))
            {
                ModelState.AddModelError(
                    nameof(DayPass.PaymentMethod),
                    "Please select a valid payment method.");
            }
        }
    }
}