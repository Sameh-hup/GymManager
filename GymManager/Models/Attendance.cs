using System.ComponentModel.DataAnnotations;

namespace GymManager.Models;

public class Attendance
{
    public int Id { get; set; }

    [Required]
    public int MemberId { get; set; }

    public DateTime CheckInDateTime { get; set; } = DateTime.Now;

    [StringLength(300)]
    public string? Notes { get; set; }

    // Relationship
    public Member Member { get; set; } = null!;
}