
using GymManager.Data;
using GymManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly GymDbContext _context;

        public DashboardController(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var nextSevenDays = today.AddDays(7);

            // Analytics date range: today + previous 6 days
            var startDate = today.AddDays(-6);
            var endDateExclusive = today.AddDays(1);

            var model = new DashboardViewModel();

            // Total members
            model.TotalMembers = await _context.Members
                .CountAsync();

            // Memberships active today
            model.ActiveMemberships = await _context.Memberships
                .CountAsync(m =>
                    m.StartDate.Date <= today &&
                    m.EndDate.Date >= today);

            // Memberships that have expired
            model.ExpiredMemberships = await _context.Memberships
                .CountAsync(m => m.EndDate.Date < today);

            // Attendance records created today
            model.TodayAttendance = await _context.Attendances
                .CountAsync(a =>
                    a.CheckInDateTime >= today &&
                    a.CheckInDateTime < endDateExclusive);

            // Day passes registered today
            model.TodayDayPasses = await _context.DayPasses
                .CountAsync(d =>
                    d.VisitDateTime >= today &&
                    d.VisitDateTime < endDateExclusive);

            // Total recorded payments
            model.TotalPayments = await _context.Payments
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            // Latest 5 payments
            model.RecentPayments = await _context.Payments
                .Include(p => p.Membership)
                    .ThenInclude(m => m.Member)
                .Include(p => p.Membership)
                    .ThenInclude(m => m.MembershipPlan)
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToListAsync();

            // Latest 5 attendance records
            model.RecentAttendance = await _context.Attendances
                .Include(a => a.Member)
                .OrderByDescending(a => a.CheckInDateTime)
                .Take(5)
                .ToListAsync();

            // Memberships expiring within the next 7 days
            model.ExpiringMemberships = await _context.Memberships
                .Include(m => m.Member)
                .Include(m => m.MembershipPlan)
                .Where(m =>
                    m.StartDate.Date <= today &&
                    m.EndDate.Date >= today &&
                    m.EndDate.Date <= nextSevenDays)
                .OrderBy(m => m.EndDate)
                .ToListAsync();

            // Get actual payments during the last 7 days

            var recentPayments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate &&
                            p.PaymentDate < endDateExclusive)
                .Select(p => new { p.PaymentDate, p.Amount })
                .ToListAsync();

            var recentDayPasses = await _context.DayPasses
                .Where(d => d.VisitDateTime >= startDate &&
                            d.VisitDateTime < endDateExclusive)
                .Select(d => new { d.VisitDateTime, d.Fee })
                .ToListAsync();

            var recentAttendance = await _context.Attendances
                .Where(a => a.CheckInDateTime >= startDate &&
                            a.CheckInDateTime < endDateExclusive)
                .Select(a => new { a.CheckInDateTime })
                .ToListAsync();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);

                model.Last7DaysLabels.Add(date.ToString("dd MMM"));

                var membershipRevenue = recentPayments
                    .Where(p => p.PaymentDate.Date == date)
                    .Sum(p => p.Amount);

                var dayPassRevenue = recentDayPasses
                    .Where(d => d.VisitDateTime.Date == date)
                    .Sum(d => d.Fee);

                var totalDailyRevenue = membershipRevenue + dayPassRevenue;

                model.Last7DaysRevenue.Add(totalDailyRevenue);

                var dailyAttendance = recentAttendance
                    .Count(a => a.CheckInDateTime.Date == date);

                model.Last7DaysAttendance.Add(dailyAttendance);
            }
            return View(model);
        }
    }
}