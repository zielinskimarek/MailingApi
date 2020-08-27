using System;
using System.Collections.Generic;
using MailingApi.DataAccess.Dtos;
using MailingApi.DataAccess.Mappers;
using MailingApi.Domain.Models;
using Xunit;

namespace MailingApi.Tests.DataAccess.Mappers
{
    public class EmailsMapperTests
    {
        private const string Separator = ",";

        [Fact]
        public void When_Map_WithEmailDataInputIsCalled_Then_PendingEmailIsReturned()
        {
            // Given
            var emailDataInput = new EmailDataInput
            {
                Sender = "sender@test.com",
                Recipients = new List<string>
                {
                    "recipient1@test.com",
                    "recipient2@test.com"
                },
                Subject = "Test subject",
                Body = "Test body"
            };
            var emailsMapper = new EmailsMapper();

            // When
            var email = emailsMapper.Map(emailDataInput);

            // Then
            Assert.Equal(emailDataInput.Sender, email.Sender);
            Assert.Equal(string.Join(Separator, emailDataInput.Recipients), email.Recipients);
            Assert.Equal(emailDataInput.Subject, email.Subject);
            Assert.Equal(emailDataInput.Body, email.Body);
            Assert.Equal(EmailStatus.Pending, email.EmailStatus);
        }

        [Fact]
        public void When_Map_WithEmailDataInputIsCalled_AndSendersEmailIsInvalid_Then_ExceptionIsThrown()
        {
            // Given
            var emailDataInput = new EmailDataInput
            {
                Sender = "sender",
                Recipients = new List<string>
                {
                    "recipient1@test.com",
                    "recipient2@test.com"
                },
                Subject = "Test subject",
                Body = "Test body"
            };
            var emailsMapper = new EmailsMapper();

            // When & Then
            var exception = Assert.Throws<ArgumentException>(() => emailsMapper.Map(emailDataInput));
            Assert.Contains("'sender' is not a valid email address", exception.Message);
        }

        [Fact]
        public void When_Map_WithEmailDataInputIsCalled_AndOneOfRecipientsEmailIsInvalid_Then_ArgumentExceptionIsThrown()
        {
            // Given
            var emailDataInput = new EmailDataInput
            {
                Sender = "sender@test.com",
                Recipients = new List<string>
                {
                    "recipient1@test.com",
                    "recipient"
                },
                Subject = "Test subject",
                Body = "Test body"
            };
            var emailsMapper = new EmailsMapper();

            // When & Then
            var exception = Assert.Throws<ArgumentException>(() => emailsMapper.Map(emailDataInput));
            Assert.Contains("'recipient' is not a valid email address", exception.Message);
        }
    }
}
