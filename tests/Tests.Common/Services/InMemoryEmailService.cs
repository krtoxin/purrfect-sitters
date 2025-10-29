using Application.Common.Interfaces;

namespace Tests.Common.Services;

public class InMemoryEmailService : IEmailSendingService
{
    private readonly List<EmailMessage> _sentEmails = new();

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _sentEmails.Add(new EmailMessage(to, subject, body));
        return Task.CompletedTask;
    }

    public IReadOnlyList<EmailMessage> SentEmails => _sentEmails.AsReadOnly();

    public void ClearSentEmails() => _sentEmails.Clear();

    public record EmailMessage(string To, string Subject, string Body);
}