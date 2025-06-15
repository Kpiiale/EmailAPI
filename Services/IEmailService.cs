namespace EmailRegistroAPI.Services
{
    // Services/IEmailService.cs
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string code);
    }
}

