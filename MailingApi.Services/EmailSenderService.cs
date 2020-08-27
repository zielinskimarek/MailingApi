using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace MailingApi.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpClient _smtpClient;

        public EmailSenderService(string hostName, int port, string username, string password, bool enableSsl)
        {
            _smtpClient = new SmtpClient(hostName)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };
        }

        public void Send(string sender, IEnumerable<string> recipients, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(sender),
                    Subject = subject,
                    Body = body,
                };

                foreach (var recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }

                _smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
