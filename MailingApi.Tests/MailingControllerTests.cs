using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailingApi.Controllers;
using MailingApi.DataAccess.Dtos;
using MailingApi.DataAccess.Repositories;
using MailingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MailingApi.Tests
{
    public class MailingControllerTests
    {
        private readonly Mock<IEmailsRepository> _emailsRepositoryMock = new Mock<IEmailsRepository>();
        private readonly Mock<IEmailSenderService> _emailSenderServiceMock = new Mock<IEmailSenderService>();

        private readonly EmailDataInput _emailDataInputStub = new EmailDataInput
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

        private readonly EmailDto _emailDtoStub = new EmailDto(
            Guid.NewGuid(), 
            "sender@test.com", 
            new List<string>{ "recipient@test.com" },
            "Test subject", 
            "Test body",
            "Pending");

        private readonly Guid _emailId = Guid.NewGuid();
        private const string SuccessResponseMessage = "Success";
        private const string ErrorResponseMessage = "Error";

        [Fact]
        public async Task When_CreateEmailIsCalled_Then_EmailsRepositoryCallsCreateEmailOnce()
        {
            // Given
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            await controller.CreateEmail(_emailDataInputStub);

            // Then
            _emailsRepositoryMock.Verify(mock => mock.CreateEmail(_emailDataInputStub), Times.Once);
        }

        [Fact]
        public async Task When_CreateEmailIsCalled_WithValidEmailInput_Then_ControllersCreateEmail_Returns_200()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.CreateEmail(_emailDataInputStub))
                .ReturnsAsync(new ActionResponse<bool>(true, SuccessResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.CreateEmail(_emailDataInputStub);

            // Then
            var statusCodeResult = result as StatusCodeResult;
            Assert.Equal(200, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_CreateEmailIsCalled_WithInvalidEmailInput_Then_ControllersCreateEmail_Returns_500()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.CreateEmail(null))
                .ReturnsAsync(new ActionResponse<bool>(false, ErrorResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.CreateEmail(null);

            // Then
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(500, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_GetEmailStatusIsCalled_Then_EmailsRepositoryCallsGetEmailStatusOnce()
        {
            // Given
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            await controller.GetEmailStatus(_emailId);

            // Then
            _emailsRepositoryMock.Verify(mock => mock.GetEmailStatus(_emailId), Times.Once);
        }

        [Fact]
        public async Task When_GetEmailStatusIsCalled_WithValidEmailId_Then_ControllersGetEmailStatus_Returns_200()
        {
            const string pendingStatus = "Pending";
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.GetEmailStatus(_emailId))
                .ReturnsAsync(new ActionResponse<string>(pendingStatus, SuccessResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.GetEmailStatus(_emailId);

            // Then
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(200, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_GetEmailStatusIsCalled_WithInvalidEmailId_Then_ControllersGetEmailStatus_Returns_404()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.GetEmailStatus(Guid.Empty))
                .ReturnsAsync(new ActionResponse<string>(string.Empty, ErrorResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.GetEmailStatus(_emailId);

            // Then
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(404, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_GetPendingEmailsIsCalled_Then_EmailsRepositoryCallsGetPendingEmailsOnce()
        {
            // Given
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            await controller.GetPendingEmails();

            // Then
            _emailsRepositoryMock.Verify(mock => mock.GetPendingEmails(), Times.Once);
        }

        [Fact]
        public async Task When_GetPendingEmailsIsCalled_AndThereAreEmailsInPendingState_Then_ControllersGetPendingEmails_Returns_200()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.GetPendingEmails())
                .ReturnsAsync(new ActionResponse<IEnumerable<EmailDto>>(new List<EmailDto>{_emailDtoStub}, SuccessResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.GetPendingEmails();

            // Then
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(200, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_GetPendingEmailsIsCalled_ButThereAreNoEmailsInPendingState_Then_ControllersGetPendingEmails_Returns_404()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.GetPendingEmails())
                .ReturnsAsync(new ActionResponse<IEnumerable<EmailDto>>(new List<EmailDto>(), ErrorResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            var result = await controller.GetPendingEmails();

            // Then
            var statusCodeResult = result as ObjectResult;
            Assert.Equal(404, statusCodeResult?.StatusCode);
        }

        [Fact]
        public async Task When_SendPendingEmailsIsCalled_AndThereAreTwoPendingEmails_Then_EmailSenderServiceCallsGetPendingEmailsTwice()
        {
            // Given
            _emailsRepositoryMock
                .Setup(mock => mock.GetPendingEmails())
                .ReturnsAsync(new ActionResponse<IEnumerable<EmailDto>>(new List<EmailDto> { _emailDtoStub, _emailDtoStub }, SuccessResponseMessage));
            var controller = new MailingController(_emailsRepositoryMock.Object, _emailSenderServiceMock.Object);

            // When
            await controller.SendPendingEmails();

            // Then
            _emailSenderServiceMock.Verify(mock => mock.Send(
                It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
