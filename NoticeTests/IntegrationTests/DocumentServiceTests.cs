using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.DocumentService;
using Notices.NoticeData;
using Notices.NoticeService;
using Notices.NoticeStorage;
using Xunit;

namespace Notices.NoticeTests.IntegrationTests
{
    public class DocumentServiceTests
    {
        private string _templateDirectory;
        private string _documentArchiveDirectory;
        private ProcessingInformation _principalInformation;
        private NoticeStorageService _noticeStorageService;

        public DocumentServiceTests ()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            _templateDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests");
            _documentArchiveDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests\ArchiveFolder");
            _principalInformation = new ProcessingInformation
            {
                DocumentTemplate = "PaidSickLeave-MandatoryNotice-English.pdf",
                FormParameters = new Dictionary<string, string>
                { { "Start of Calendar Year", "2019-01-01" },
                { "End of Calendar Year", "2019-12-31" }
                }
            };
            var noticeStorageOptions = Options.Create<NoticeStorageOptions> (
                new NoticeStorageOptions
                {
                    ConnectionString = "UseDevelopmentStorage=true",
                        ApplicationName = "notice"
                });

            _noticeStorageService = new NoticeStorageService (noticeStorageOptions, null);
        }

        [Fact]
        public async Task CreateNoticeDocumentCreatesSimpleDocument ()
        {
            IDocumentService cut = new NoticeDocumentService (null, new FileSystem (), _noticeStorageService, GetDocumentOptions ());
            var actual = await cut.CreateNoticeDocument (
                _principalInformation,
                Mandate.TestNotifications);

            Assert.True (actual.WasSuccessful);
        }

        private IOptions<DocumentServiceOptions> GetDocumentOptions ()
        {
            return Options.Create<DocumentServiceOptions> (new DocumentServiceOptions
            {
                ArchiveDirectory = _documentArchiveDirectory,
                    TemplateDirectory = _templateDirectory
            });
        }
    }
}