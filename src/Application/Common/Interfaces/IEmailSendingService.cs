namespace Application.Common.Interfaces;

public interface IEmailSendingService
{
    Task SendEmailAsync(string to, string subject, string body);
}