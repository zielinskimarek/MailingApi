using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailingApi.DataAccess.Dtos;
using MailingApi.Domain.Models;

namespace MailingApi.DataAccess.Mappers
{
    public class EmailsMapper : IEmailsMapper
    {
        private const string Separator = ",";

        public Email Map(EmailDataInput emailDataInput)
        {
            var sender = IsEmailAddressValid(emailDataInput.Sender) ? emailDataInput.Sender : throw new ArgumentException($"'{emailDataInput.Sender}' is not a valid email address");
            var joinedRecipients = JoinRecipients(emailDataInput.Recipients);
            return new Email(sender, joinedRecipients, emailDataInput.Subject, emailDataInput.Body, EmailStatus.Pending);
        }

        public IEnumerable<EmailDto> Map(IEnumerable<Email> emails)
        {
            return emails.Select(email => new EmailDto(email.Id, email.Sender, email.Recipients.Split(Separator.ToCharArray()), email.Subject, email.Body, email.EmailStatus.ToString()));
        }

        private static bool IsEmailAddressValid(string emailAddress)
        {
            try
            {
                var mailAddress = new MailAddress(emailAddress);
                return mailAddress.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }

        private static string JoinRecipients(IEnumerable<string> recipients)
        {
            var joinedRecipients = string.Empty;
            foreach (var recipient in recipients)
            {
                if (IsEmailAddressValid(recipient))
                {
                    joinedRecipients = string.Join(Separator, joinedRecipients, recipient);
                }
                else
                {
                    throw new ArgumentException($"'{recipient}' is not a valid email address");
                }
            }

            return joinedRecipients.Substring(1);
        }
    }
}
