using System;
using System.Linq;
using System.Threading.Tasks;
using MailingApi.DataAccess.Dtos;
using MailingApi.DataAccess.Repositories;
using MailingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailingController : ControllerBase
    {
        private readonly IEmailsRepository _emailsRepository;
        private readonly IEmailSenderService _emailSenderService;

        public MailingController(IEmailsRepository emailsRepository, IEmailSenderService emailSenderService)
        {
            _emailsRepository = emailsRepository;
            _emailSenderService = emailSenderService;
        }

        [HttpPost]
        [Route("create-email")]
        public async Task<IActionResult> CreateEmail([FromBody]EmailDataInput emailData)
        {
            var result = await _emailsRepository.CreateEmail(emailData);
            if (result != null && result.Response)
            {
                return Ok();
            }

            return StatusCode(500, result?.Message);
        }

        [HttpGet]
        [Route("get-email-status")]
        public async Task<IActionResult> GetEmailStatus(Guid emailId)
        {
            var result = await _emailsRepository.GetEmailStatus(emailId);
            if (string.IsNullOrWhiteSpace(result?.Response))
            {
                return StatusCode(404, result?.Message);
            }

            return Ok(result);

        }

        [HttpGet]
        [Route("get-pending-emails")]
        public async Task<IActionResult> GetPendingEmails()
        {
            var result = await _emailsRepository.GetPendingEmails();
            if (result?.Response == null || !result.Response.Any())
            {
                return StatusCode(404, result?.Message);
            }

            return Ok(result);

        }

        [HttpPost]
        [Route("send-pending-emails")]
        public async Task SendPendingEmails()
        {
            await SendEmails();
        }

        private async Task SendEmails()
        {
            var result = await _emailsRepository.GetPendingEmails();
            foreach (var email in result.Response)
            {
                _emailSenderService.Send(email.Sender, email.Recipients, email.Subject, email.Body);
                await _emailsRepository.MarkEmailAsSent(email.Id);
            }
        }
    }
}
