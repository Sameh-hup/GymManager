using Microsoft.EntityFrameworkCore;
using GymManager.Models;
namespace GymManager.Data;

public class GymDbContext:DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membership>().Property(m => m.PriceAtPurchase).HasPrecision(18, 4);
        modelBuilder.Entity<MembershipPlan>().Property(m => m.Price).HasPrecision(18, 4);
        modelBuilder.Entity<Payment>().Property(m => m.Amount).HasPrecision(18, 4);
        modelBuilder.Entity<DayPass>().Property(m => m.Fee).HasPrecision(18, 4);
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Member> Members { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MembershipPlan> MembershipPlans { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<DayPass> DayPasses { get; set; }
}
