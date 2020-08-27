using System;
using System.Collections.Generic;

namespace MailingApi.DataAccess.Dtos
{
    public class EmailDto
    {
        public EmailDto(Guid id, string sender, IEnumerable<string> recipients, string subject, string body, string emailStatus)
        {
            Id = id;
            Sender = sender;
            Recipients = recipients;
            Subject = subject;
            Body = body;
            EmailStatus = emailStatus;
        }

        public Guid Id { get; private set; }

        public string Sender { get; private set; }

        public IEnumerable<string> Recipients { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public string EmailStatus { get; private set; }
    }
}
