// DTOs/ConfirmRegistrationRequestDto.cs
using System.ComponentModel.DataAnnotations;

public class ConfirmRegistrationRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(6)]
    public string VerificationCode { get; set; }
}