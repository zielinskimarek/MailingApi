using System;
using System.ComponentModel.DataAnnotations;

namespace MailingApi.Domain.Models
{
    public class Email
    {
        private Email() {}

        public Email(string sender, string recipients, string subject, string body, EmailStatus emailStatus)
        {
            Sender = sender;
            Recipients = recipients;
            Subject = subject;
            Body = body;
            EmailStatus = emailStatus;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public string Sender { get; private set; }

        [Required]
        public string Recipients { get; private set; }

        [Required]
        public string Subject { get; private set; }

        public string Body { get; private set; }

        public EmailStatus EmailStatus { get; private set; }

        public void MarkAsSent()
        {
            EmailStatus = EmailStatus.Sent;
        }
    }
}
