using WhiteLagoon.Application.Contract;

namespace WhiteLagoon.Infrastructure.Emails;

public class EmailService : IEmailService
{
    
    
    public Task<bool> SendEmailAsync(string email, string subject, string message)
    {
        throw new NotImplementedException();
    }
}