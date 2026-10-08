
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManager.Models;
using GymManager.Data;

public class MembershipPlansController : Controller
{
    private readonly GymDbContext _context;

    public MembershipPlansController(GymDbContext context)
    {
        _context = context;
    }

    // GET: MEMBERSHIPPLANS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.MembershipPlans.ToListAsync());
    }

    // GET: MEMBERSHIPPLANS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipplan = await _context.MembershipPlans
            .FirstOrDefaultAsync(m => m.Id == id);
        if (membershipplan == null)
        {
            return NotFound();
        }

        return View(membershipplan);
    }

    // GET: MEMBERSHIPPLANS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEMBERSHIPPLANS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,DurationInDays,Price,Description,IsActive")] MembershipPlan membershipplan)
    {
        if (ModelState.IsValid)
        {
            _context.Add(membershipplan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(membershipplan);
    }

    // GET: MEMBERSHIPPLANS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipplan = await _context.MembershipPlans.FindAsync(id);
        if (membershipplan == null)
        {
            return NotFound();
        }
        return View(membershipplan);
    }

    // POST: MEMBERSHIPPLANS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,DurationInDays,Price,Description,IsActive")] MembershipPlan membershipplan)
    {
        if (id != membershipplan.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(membershipplan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembershipPlanExists(membershipplan.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(membershipplan);
    }

    // GET: MEMBERSHIPPLANS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershipplan = await _context.MembershipPlans
            .FirstOrDefaultAsync(m => m.Id == id);
        if (membershipplan == null)
        {
            return NotFound();
        }

        return View(membershipplan);
    }

    // POST: MEMBERSHIPPLANS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var membershipplan = await _context.MembershipPlans.FindAsync(id);
        if (membershipplan != null)
        {
            _context.MembershipPlans.Remove(membershipplan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MembershipPlanExists(int? id)
    {
        return _context.MembershipPlans.Any(e => e.Id == id);
    }
}
