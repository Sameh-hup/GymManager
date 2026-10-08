using System.ComponentModel.DataAnnotations;
namespace GymManager.Models;

public class Membership
{
    public int Id { get; set; }
    [Required]
    public int MemberId { get; set; }
    [Required]  
    public int MembershipPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [Range(0, 1000000)]
    public decimal PriceAtPurchase { get; set; }
    
    // Relationships
    public Member? Member { get; set; } = null!;
    public MembershipPlan? MembershipPlan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
