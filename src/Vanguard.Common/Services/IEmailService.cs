namespace Vanguard.Common.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendTemplatedEmailAsync(string to, string templateName, object model);
    }
}
