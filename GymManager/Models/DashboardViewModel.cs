
using GymManager.Models;

namespace GymManager.Models
{
    public class DashboardViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int TodayAttendance { get; set; }
        public int TodayDayPasses { get; set; }

        public decimal TotalPayments { get; set; }

        public List<Payment> RecentPayments { get; set; } = new();
        public List<Attendance> RecentAttendance { get; set; } = new();
        public List<Membership> ExpiringMemberships { get; set; } = new();

        // Analytics: last 7 days
        public List<string> Last7DaysLabels { get; set; } = new();
        public List<decimal> Last7DaysRevenue { get; set; } = new();
        public List<int> Last7DaysAttendance { get; set; } = new();
    }
}