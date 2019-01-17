using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeStorage;
using Notices.TestNotifiers;
using Xunit;

namespace Notices.NoticeTests.IntegrationTests
{
    public class TestNoticeProviderTests
    {
        private NoticeStorageService _noticeStorageService;
        private NoticeDocumentService _documentService;
        private NoticeEmail _emailService;

        public TestNoticeProviderTests ()
        {
            var noticeStorageOptions = Options.Create<NoticeStorageOptions> (
                new NoticeStorageOptions
                {
                    ConnectionString = "UseDevelopmentStorage=true",
                        ApplicationName = "notice"
                });

            _noticeStorageService = new NoticeStorageService (noticeStorageOptions, null);
            
            _documentService = new NoticeDocumentService (null, new FileSystem (),_noticeStorageService, GetDocumentOptions ());
            _emailService = new NoticeEmail (GetEmailOptions (), null, _noticeStorageService, new FileSystem ());
        }

        [Fact (DisplayName = "Notify Stores Document and Email")]
        public async Task NotifyStoresDocumentAndEmail ()
        {
            var cut = new TestNoticeProvider (null, _emailService, _documentService, _noticeStorageService);
            var actual = await cut.Notify ("123456789", Mandate.TestNotifications, "Initial");
            Assert.True (actual.WasSuccessful);
        }

        private IOptions<DocumentServiceOptions> GetDocumentOptions ()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            return Options.Create<DocumentServiceOptions> (new DocumentServiceOptions
            {
                ArchiveDirectory = Path.Combine (dir, @"..\..\..\IntegrationTests\DocumentArchive"),
                    TemplateDirectory = Path.Combine (dir, @"..\..\..\IntegrationTests")
            });
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