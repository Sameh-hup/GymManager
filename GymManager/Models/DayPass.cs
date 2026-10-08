using System.ComponentModel.DataAnnotations;

namespace GymManager.Models;

public class DayPass
{
    public int Id { get; set; }

    [StringLength(100)]
    public string? VisitorName { get; set; }

    [Phone]
    public string? VisitorPhone { get; set; }

    public DateTime VisitDateTime { get; set; } = DateTime.Now;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Fee { get; set; }

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    [StringLength(300)]
    public string? Notes { get; set; }
}