// Controllers/AuthController.cs
using EmailRegistroAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IEmailService _emailService;

    public AuthController(ApplicationDbContext context, IMemoryCache cache, IEmailService emailService)
    {
        _context = context;
        _cache = cache;
        _emailService = emailService;
    }

    // A private class to hold registration data temporarily in the cache
    private class PendingRegistration
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string VerificationCode { get; set; }
        public string? FullName { get; set; }
        public int? CompanyId { get; set; }
        public string Role { get; set; }
    }

    [HttpPost("initiate-registration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InitiateRegistration([FromBody] InitiateRegistrationRequestDto request)
    {
        // 1. Check if user already exists
        if (await _context.Usuario.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("This email address is already registered.");
        }

        // 2. Generate verification code and hash password
        var verificationCode = new Random().Next(100000, 999999).ToString();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Store pending registration data in cache for 10 minutes
        var pendingUser = new PendingRegistration
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            VerificationCode = verificationCode,
            FullName = request.FullName,
            CompanyId = request.CompanyId,
            Role = request.Role ?? "U" // Default to 'U' if not provided
        };

        _cache.Set(request.Email, pendingUser, TimeSpan.FromMinutes(10));

        // 4. Send email
        try
        {
            await _emailService.SendVerificationEmailAsync(request.Email, verificationCode);
        }
        catch (Exception ex)
        {
            // Log the exception (using a proper logging framework is recommended)
            return StatusCode(500, "An error occurred while sending the verification email.");
        }

        return Ok(new { message = "A verification code has been sent to your email address." });
    }

    [HttpPost("confirm-registration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationRequestDto request)
    {
        // 1. Retrieve pending registration from cache
        if (!_cache.TryGetValue(request.Email, out PendingRegistration pendingUser))
        {
            return BadRequest("Invalid or expired registration request. Please try again.");
        }

        // 2. Validate the verification code
        if (pendingUser.VerificationCode != request.VerificationCode)
        {
            return BadRequest("The verification code is incorrect.");
        }

        // 3. Create the final user object
        var newUser = new User
        {
            Email = pendingUser.Email,
            PasswordHash = pendingUser.PasswordHash,
            FullName = pendingUser.FullName,
            CompanyId = pendingUser.CompanyId,
            Role = pendingUser.Role,
            IsActive = true,
            CreationDate = DateTime.UtcNow
        };

        // 4. Save to database
        _context.Usuario.Add(newUser);
        await _context.SaveChangesAsync();

        // 5. Clean up the cache
        _cache.Remove(request.Email);

        return Ok(new { message = "Registration completed successfully!" });
    }
}