using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.TestNotifiers;
using Xunit;

namespace NoticeTests.IntegrationTests
{
    public class OtherNotifierTests
    {
        private NoticeDocumentService _documentService;
        private NoticeEmail _emailService;

        public OtherNotifierTests ()
        {
            _documentService = new NoticeDocumentService (null, new FileSystem (), null);
            _emailService = new NoticeEmail (GetEmailOptions (), null, new FileSystem ());
        }

        [Fact(DisplayName="Notify Stores Document and Email")]
        public async Task NotifyStoresDocumentAndEmail ()
        {
            var cut = new TestNotifier(null,_emailService, _documentService);
            var actual = await cut.Notify("123456789", Mandate.TestNotifications, "Initial");
            Assert.True(actual.WasSuccessful);
        }

        private IOptions<NoticeEmailOptions> GetEmailOptions ()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;


            return Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailTemplateFolder = Path.Combine (dir, @"..\..\..\IntegrationTests"),
                        EmailArchiveFolder = Path.Combine (dir, @"..\..\..\\IntegrationTests\EmailArchiveFolder"),
                        EmailSenderAddress = "email.return@address.com",
                        EmailSenderName = "Email Return",
                        SmtpHost = "127.0.0.1",
                        SmtpPort = 0
                });
        }
    }
}