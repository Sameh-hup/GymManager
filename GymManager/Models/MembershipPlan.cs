using System.ComponentModel.DataAnnotations;
namespace GymManager.Models;

public class MembershipPlan
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Range(1,365)]
    public int DurationInDays { get; set; }
    [Range(0,1000000)]  
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
