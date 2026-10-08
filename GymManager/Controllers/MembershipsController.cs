
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class MembershipsController : Controller
{
    private readonly GymDbContext _context;

    public MembershipsController(GymDbContext context)
    {
        _context = context;
    }

    // GET: MEMBERSHIPS
    public async Task<IActionResult> Index()
    {
        return View(await _context.Memberships
            .Include(m => m.Member)
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .OrderByDescending(m => m.StartDate)
            .ToListAsync());
    }

    // GET: MEMBERSHIPS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membership = await _context.Memberships
            .Include(m => m.Member)
            .Include(m => m.MembershipPlan)
            .Include(m => m.Payments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
        {
            return NotFound();
        }

        return View(membership);
    }

    // GET: Memberships/Create
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();

        var membership = new Membership
        {
            StartDate = DateTime.Today
        };

        return View(membership);
    }

    // POST: MEMBERSHIPS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MemberId,MembershipPlanId,StartDate")] Membership membership)
    {
        ModelState.Remove(nameof(Membership.EndDate));
        ModelState.Remove(nameof(Membership.PriceAtPurchase));

        var memberExists = await _context.Members
            .AnyAsync(m => m.Id == membership.MemberId);

        var plan = await _context.MembershipPlans
            .FirstOrDefaultAsync(p =>
                p.Id == membership.MembershipPlanId && p.IsActive);

        if (!memberExists)
        {
            ModelState.AddModelError(
                nameof(Membership.MemberId),
                "The selected member does not exist.");
        }

        if (plan == null)
        {
            ModelState.AddModelError(
                nameof(Membership.MembershipPlanId),
                "The selected plan does not exist or is inactive.");
        }

        if (ModelState.IsValid)
        {
            membership.EndDate =
                membership.StartDate.AddDays(plan!.DurationInDays);

            membership.PriceAtPurchase = plan.Price;

            _context.Memberships.Add(membership);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        await LoadDropdownsAsync();

        return View(membership);
    }

    // GET: MEMBERSHIPS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membership = await _context.Memberships
            .Include(m => m.Member)
            .Include(m => m.MembershipPlan)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
        {
            return NotFound();
        }

        return View(membership);
    }

    // POST: MEMBERSHIPS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,StartDate")] Membership membership)
    {
        if (id != membership.Id)
        {
            return NotFound();
        }

        var existingMembership = await _context.Memberships
            .Include(m => m.MembershipPlan)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (existingMembership == null)
        {
            return NotFound();
        }

        ModelState.Remove(nameof(Membership.MemberId));
        ModelState.Remove(nameof(Membership.MembershipPlanId));
        ModelState.Remove(nameof(Membership.EndDate));
        ModelState.Remove(nameof(Membership.PriceAtPurchase));

        if (!ModelState.IsValid)
        {
            var viewModel = await _context.Memberships
                .Include(m => m.Member)
                .Include(m => m.MembershipPlan)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (viewModel == null)
            {
                return NotFound();
            }

            viewModel.StartDate = membership.StartDate;

            return View(viewModel);
        }

        existingMembership.StartDate = membership.StartDate;

        existingMembership.EndDate =
            membership.StartDate.AddDays(
                existingMembership.MembershipPlan.DurationInDays);

        // Keep the original purchase price unchanged.

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: MEMBERSHIPS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membership = await _context.Memberships
            .Include(m => m.Member)
            .Include(m => m.MembershipPlan)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
        {
            return NotFound();
        }

        return View(membership);
    }

    // POST: MEMBERSHIPS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membership = await _context.Memberships
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
        {
            return NotFound();
        }

        var hasPayments = await _context.Payments
            .AnyAsync(p => p.MembershipId == id);

        if (hasPayments)
        {
            TempData["Error"] =
                "Cannot delete this membership because it has payment records.";

            return RedirectToAction(nameof(Index));
        }

        _context.Memberships.Remove(membership);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    private async Task LoadDropdownsAsync()
    {
        ViewBag.Members = new SelectList(
            await _context.Members
                .OrderBy(m => m.FullName)
                .ToListAsync(),
            "Id",
            "FullName");

        ViewBag.MembershipPlans = new SelectList(
            await _context.MembershipPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync(),
            "Id",
            "Name");
    }
    private bool MembershipExists(int? id)
    {
        return _context.Memberships.Any(e => e.Id == id);
    }
}
