using System.Collections.Generic;

namespace MailingApi.Services
{
    public interface IEmailSenderService
    {
        void Send(string sender, IEnumerable<string> recipients, string subject, string body);
    }
}
