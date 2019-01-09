using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.DocumentService;
using Notices.NoticeData;
using Notices.NoticeService;
using Xunit;

namespace Notices.NoticeTests.IntegrationTests
{
    public class DocumentServiceTests
    {
        private string _templateDirectory;
        private string _documentArchiveDirectory;
        private PrincipalInformation _principalInformation;

        public DocumentServiceTests()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            
            _templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests");
            _documentArchiveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests\ArchiveFolder");
            _principalInformation = new PrincipalInformation
            {
                DocumentTemplate="PaidSickLeave-MandatoryNotice-English.pdf",
            };

        }
        
        [Fact]
        public async Task CreateNoticeDocumentCreatesSimpleDocument()
        {
            IDocumentService cut = new NoticeDocumentService(null, new FileSystem(), GetDocumentOptions());
            var actual = await cut.CreateNoticeDocument(
                _principalInformation, 
                Mandate.TestNotifications);

            Assert.True(File.Exists(actual.DocumentFilePath));
        }

        private IOptions<DocumentServiceOptions> GetDocumentOptions()
        {
            return Options.Create<DocumentServiceOptions> (new DocumentServiceOptions
            {
                ArchiveDirectory = _documentArchiveDirectory,
                TemplateDirectory = _templateDirectory
            });
        }
    }
}