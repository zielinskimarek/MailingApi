using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailingApi.DataAccess.Dtos;
using MailingApi.DataAccess.Mappers;
using MailingApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MailingApi.DataAccess.Repositories
{
    public class EmailsRepository : BaseRepository, IEmailsRepository
    {
        private readonly Func<DomainContext> _domainContextFactory;
        private readonly IEmailsMapper _emailsMapper;

        public EmailsRepository(Func<DomainContext> domainContextFactory, IEmailsMapper emailsMapper)
        {
            _domainContextFactory = domainContextFactory;
            _emailsMapper = emailsMapper;
        }

        public async Task<ActionResponse<bool>> CreateEmail(EmailDataInput emailDataInput)
        {
            const string responseMessage = "Successfully created emails";
            return await ExecuteDbOperation(async () =>
            {
                var email = _emailsMapper.Map(emailDataInput);
                using (var context = _domainContextFactory())
                {
                    await context.Emails.AddAsync(email);
                    await context.SaveChangesAsync();
                }

                return new ActionResponse<bool>(true, responseMessage);
            });
        }

        public async Task<ActionResponse<string>> GetEmailStatus(Guid emailId)
        {
            return await ExecuteDbOperation(async () =>
            {
                using (var context = _domainContextFactory())
                {
                    var email = await context.Emails.FirstOrDefaultAsync(e => e.Id == emailId);
                    var emailStatus = email != null ? email.EmailStatus.ToString() : string.Empty;
                    var responseMessage = email != null ? "Successfully loaded email status" : $"Could not find email with id: {emailId}";
                    return new ActionResponse<string>(emailStatus, responseMessage);
                }
            });
        }

        public async Task<ActionResponse<IEnumerable<EmailDto>>> GetPendingEmails()
        {
            return await ExecuteDbOperation(async () =>
            {
                using (var context = _domainContextFactory())
                {
                    var emails = await context.Emails.Where(e => e.EmailStatus == EmailStatus.Pending).ToListAsync();
                    var pendingEmails = _emailsMapper.Map(emails).ToList();
                    var responseMessage = pendingEmails.Any() ? "Successfully loaded pending emails" : "No pending emails";
                    return new ActionResponse<IEnumerable<EmailDto>>(pendingEmails, responseMessage);
                }
            });
        }

        public async Task MarkEmailAsSent(Guid emailId)
        {
            await ExecuteDbOperation(async () =>
            {
                using (var context = _domainContextFactory())
                {
                    var email = await context.Emails.FirstOrDefaultAsync(e => e.Id == emailId);
                    email.MarkAsSent();

                    await context.SaveChangesAsync();
                }
            });
        }
    }
}
