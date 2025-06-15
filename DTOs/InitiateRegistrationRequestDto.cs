// DTOs/InitiateRegistrationRequestDto.cs
using System.ComponentModel.DataAnnotations;

public class InitiateRegistrationRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    public string? FullName { get; set; }
    public int? CompanyId { get; set; }
    public string? Role { get; set; } // e.g., 'A', 'U', 'SA'
}
