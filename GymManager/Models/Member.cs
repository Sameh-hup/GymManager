using System.ComponentModel.DataAnnotations;
namespace GymManager.Models;

public class Member
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; } = DateTime.Today;

    public string ? Notes {  get; set; }
}
