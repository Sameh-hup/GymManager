using System.ComponentModel.DataAnnotations;
namespace GymManager.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int MembershipId { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    [StringLength(300)]
    public string? Notes { get; set; }

    // Relationship
    public Membership Membership { get; set; } = null!;
}
