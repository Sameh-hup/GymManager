
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly GymDbContext _context;

        public AttendanceController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            await LoadMembersDropdownAsync();

            var attendances = await _context.Attendances
                .Include(a => a.Member)
                .OrderByDescending(a => a.CheckInDateTime)
                .ToListAsync();

            return View(attendances);
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Member)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // GET: Attendance/Create
        public async Task<IActionResult> Create()
        {
            await LoadMembersDropdownAsync();

            var attendance = new Attendance
            {
                CheckInDateTime = DateTime.Now
            };

            return View(attendance);
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MemberId,CheckInDateTime,Notes")]
            Attendance attendance)
        {
            ModelState.Remove(nameof(Attendance.Member));

            var memberExists = await _context.Members
                .AnyAsync(m => m.Id == attendance.MemberId);

            if (!memberExists)
            {
                ModelState.AddModelError(
                    nameof(Attendance.MemberId),
                    "العضو المحدد غير موجود.");
            }
            else
            {
                var checkInDate = attendance.CheckInDateTime.Date;

                var hasActiveMembership = await _context.Memberships
                    .AnyAsync(m =>
                        m.MemberId == attendance.MemberId &&
                        m.StartDate.Date <= checkInDate &&
                        m.EndDate.Date >= checkInDate);

                if (!hasActiveMembership)
                {
                    ModelState.AddModelError(
                        nameof(Attendance.MemberId),
                        "العضو ليس لديه اشتراك سارٍ في تاريخ الحضور المحدد.");
                }

                var alreadyCheckedIn = await _context.Attendances
                    .AnyAsync(a =>
                        a.MemberId == attendance.MemberId &&
                        a.CheckInDateTime.Date == checkInDate);

                if (alreadyCheckedIn)
                {
                    ModelState.AddModelError(
                        nameof(Attendance.MemberId),
                        "تم تسجيل حضور هذا العضو بالفعل في نفس اليوم.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await LoadMembersDropdownAsync(attendance.MemberId);

            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Member)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            await LoadMembersDropdownAsync(attendance.MemberId);

            return View(attendance);
        }

        // POST: Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,MemberId,CheckInDateTime,Notes")]
            Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Attendance.Member));

            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAttendance == null)
            {
                return NotFound();
            }

            var memberExists = await _context.Members
                .AnyAsync(m => m.Id == attendance.MemberId);

            if (!memberExists)
            {
                ModelState.AddModelError(
                    nameof(Attendance.MemberId),
                    "العضو المحدد غير موجود.");
            }
            else
            {
                var checkInDate = attendance.CheckInDateTime.Date;

                var hasActiveMembership = await _context.Memberships
                    .AnyAsync(m =>
                        m.MemberId == attendance.MemberId &&
                        m.StartDate.Date <= checkInDate &&
                        m.EndDate.Date >= checkInDate);

                if (!hasActiveMembership)
                {
                    ModelState.AddModelError(
                        nameof(Attendance.MemberId),
                        "العضو ليس لديه اشتراك سارٍ في تاريخ الحضور المحدد.");
                }

                var duplicateExists = await _context.Attendances
                    .AnyAsync(a =>
                        a.Id != id &&
                        a.MemberId == attendance.MemberId &&
                        a.CheckInDateTime.Date == checkInDate);

                if (duplicateExists)
                {
                    ModelState.AddModelError(
                        nameof(Attendance.MemberId),
                        "يوجد تسجيل حضور آخر لهذا العضو في نفس اليوم.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadMembersDropdownAsync(attendance.MemberId);

                return View(attendance);
            }

            existingAttendance.MemberId = attendance.MemberId;
            existingAttendance.CheckInDateTime = attendance.CheckInDateTime;
            existingAttendance.Notes = attendance.Notes;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Member)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendances.Remove(attendance);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Load members for dropdown
        private async Task LoadMembersDropdownAsync(
            int? selectedMemberId = null)
        {
            var members = await _context.Members
                .OrderBy(m => m.FullName)
                .ToListAsync();

            ViewBag.Members = new SelectList(
                members,
                "Id",
                "FullName",
                selectedMemberId);
        }
    }
}