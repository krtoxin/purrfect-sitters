using Application.Common.Interfaces;
using System.Threading.Tasks;

namespace Tests.Common.Services;

public class DummyEmailSendingService : IEmailSendingService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}
