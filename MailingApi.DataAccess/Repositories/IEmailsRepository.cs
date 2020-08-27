using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailingApi.DataAccess.Dtos;

namespace MailingApi.DataAccess.Repositories
{
    public interface IEmailsRepository
    {
        Task<ActionResponse<bool>> CreateEmail(EmailDataInput emailDataInput);

        Task<ActionResponse<string>> GetEmailStatus(Guid emailId);

        Task<ActionResponse<IEnumerable<EmailDto>>> GetPendingEmails();

        Task MarkEmailAsSent(Guid emailId);
    }
}
