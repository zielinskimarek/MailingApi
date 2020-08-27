using System.Collections.Generic;
using MailingApi.DataAccess.Dtos;
using MailingApi.Domain.Models;

namespace MailingApi.DataAccess.Mappers
{
    public interface IEmailsMapper
    {
        Email Map(EmailDataInput emailDataInput);

        IEnumerable<EmailDto> Map(IEnumerable<Email> emails);
    }
}
