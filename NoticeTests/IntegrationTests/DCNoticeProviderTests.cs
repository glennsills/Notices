using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.DCNotifier;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeStorage;
using Notices.TestNotifiers;
using Xunit;

namespace Notices.NoticeTests.IntegrationTests
{
    public class DCNoticeProviderTests
    {
        private NoticeStorageService _noticeStorageService;
        private NoticeDocumentService _documentService;
        private NoticeEmail _emailService;

            // Connect to the Azure Cosmos DB Emulator running locally
            // DocumentClient client = new DocumentClient(
            //     new Uri("https://localhost:8081"),
            //     "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

        public DCNoticeProviderTests ()
        {

            // EndpointUrl;
            // _authorizationKey = options.Value.AuthorizationKey;
            // _databaseId = options.Value.DatabaseId;
            // _collectionId = options.Value.CollectionId;


            var noticeStorageOptions = Options.Create<NoticeStorageOptions> (
                new NoticeStorageOptions
                {
                    ConnectionString = "UseDevelopmentStorage=true",
                    ApplicationName = "notice",
                    EndpointUrl = "https://localhost:8081",
                    AuthorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    DatabaseId = "Notices",
                    CollectionId = "NoticeRecords"


                });

            _noticeStorageService = new NoticeStorageService (noticeStorageOptions, null);
            
            _documentService = new NoticeDocumentService (null, new FileSystem (),_noticeStorageService, GetDocumentOptions ());
            _emailService = new NoticeEmail (GetEmailOptions (), null, _noticeStorageService, new FileSystem ());
        }

        [Fact (DisplayName = "Notify Stores Document and Email")]
        public async Task NotifyStoresDocumentAndEmail ()
        {
            var cut = new DCWageNotifier (null, _emailService, _documentService, _noticeStorageService);
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