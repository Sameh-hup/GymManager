
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class PaymentsController : Controller
{
    private readonly GymDbContext _context;

    public PaymentsController(GymDbContext context)
    {
        _context = context;
    }

    // GET: PAYMENTS
    public async Task<IActionResult> Index()
    {
        var payments = await _context.Payments
            .Include(p => p.Membership)
                .ThenInclude(m => m.Member)
            .Include(p => p.Membership)
                .ThenInclude(m => m.MembershipPlan)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return View(payments);
    }

    // GET: PAYMENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payments
            .Include(p => p.Membership)
                .ThenInclude(m => m.Member)
            .Include(p => p.Membership)
                .ThenInclude(m => m.MembershipPlan)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // GET: Payments/Create
    public async Task<IActionResult> Create()
    {
        await LoadMembershipsDropdownAsync();

        var payment = new Payment
        {
            PaymentDate = DateTime.Now,
            PaymentMethod = "Cash"
        };

        return View(payment);
    }

    // POST: PAYMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MembershipId,Amount,PaymentDate,PaymentMethod,Notes")]
    Payment payment)
    {
        ModelState.Remove(nameof(Payment.Membership));

        var membership = await _context.Memberships
            .FirstOrDefaultAsync(m => m.Id == payment.MembershipId);

        if (membership == null)
        {
            ModelState.AddModelError(
                nameof(Payment.MembershipId),
                "The selected membership does not exist.");
        }
        else
        {
            var totalPaid = await _context.Payments
                .Where(p => p.MembershipId == payment.MembershipId)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            var remainingAmount =
                membership.PriceAtPurchase - totalPaid;

            if (payment.Amount > remainingAmount)
            {
                ModelState.AddModelError(
                    nameof(Payment.Amount),
                    $"The remaining amount is {remainingAmount:N2}.");
            }
        }

        if (ModelState.IsValid)
        {
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        await LoadMembershipsDropdownAsync(payment.MembershipId);

        return View(payment);
    }

    // GET: PAYMENTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payments
            .Include(p => p.Membership)
                .ThenInclude(m => m.Member)
            .Include(p => p.Membership)
                .ThenInclude(m => m.MembershipPlan)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // POST: PAYMENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,Amount,PaymentDate,PaymentMethod,Notes")]
    Payment payment)
    {
        if (id != payment.Id)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(Payment.MembershipId));
        ModelState.Remove(nameof(Payment.Membership));

        var existingPayment = await _context.Payments
            .Include(p => p.Membership)
                .ThenInclude(m => m.Member)
            .Include(p => p.Membership)
                .ThenInclude(m => m.MembershipPlan)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingPayment == null)
        {
            return NotFound();
        }

        var membership = existingPayment.Membership;

        var otherPaymentsTotal = await _context.Payments
            .Where(p =>
                p.MembershipId == existingPayment.MembershipId &&
                p.Id != existingPayment.Id)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

        var maximumAllowedAmount =
            membership.PriceAtPurchase - otherPaymentsTotal;

        if (payment.Amount > maximumAllowedAmount)
        {
            ModelState.AddModelError(
                nameof(Payment.Amount),
                $"Maximum allowed payment is {maximumAllowedAmount:N2}.");
        }

        if (ModelState.IsValid)
        {
            existingPayment.Amount = payment.Amount;
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.Notes = payment.Notes;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        existingPayment.Amount = payment.Amount;
        existingPayment.PaymentDate = payment.PaymentDate;
        existingPayment.PaymentMethod = payment.PaymentMethod;
        existingPayment.Notes = payment.Notes;

        return View(existingPayment);
    }

    // GET: PAYMENTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payments
            .Include(p => p.Membership)
                .ThenInclude(m => m.Member)
            .Include(p => p.Membership)
                .ThenInclude(m => m.MembershipPlan)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // POST: PAYMENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    private async Task LoadMembershipsDropdownAsync(
    int? selectedMembershipId = null)
    {
        var memberships = await _context.Memberships
            .Include(m => m.Member)
            .Include(m => m.MembershipPlan)
            .ToListAsync();

        var paymentsByMembership = await _context.Payments
            .GroupBy(p => p.MembershipId)
            .Select(g => new
            {
                MembershipId = g.Key,
                TotalPaid = g.Sum(p => p.Amount)
            })
            .ToDictionaryAsync(
                x => x.MembershipId,
                x => x.TotalPaid);

        var items = memberships
            .Select(m =>
            {
                paymentsByMembership.TryGetValue(
                    m.Id, out var totalPaid);

                var remaining =
                    m.PriceAtPurchase - totalPaid;

                return new
                {
                    Membership = m,
                    TotalPaid = totalPaid,
                    Remaining = remaining
                };
            })
            .Where(x => x.Remaining > 0)
            .Select(x => new SelectListItem
            {
                Value = x.Membership.Id.ToString(),
                Text =
                    $"{x.Membership.Member.FullName} - " +
                    $"{x.Membership.MembershipPlan.Name} - " +
                    $"Remaining: {x.Remaining:N2}",
                Selected = x.Membership.Id == selectedMembershipId
            })
            .ToList();

        ViewBag.Memberships = items;
    }
    private bool PaymentExists(int? id)
    {
        return _context.Payments.Any(e => e.Id == id);
    }
}
