using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
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

            };

        }
        
        [Fact]
        public async Task CreateNoticeDocumentCreatesSimpleDocument()
        {
            IDocumentService cut = new NoticeDocumentService(new FileSystem());
            var actual = await cut.CreateNoticeDocument(
                _principalInformation, 
                "PaidSickLeave-MandatoryNotice-English.pdf",
                _templateDirectory,
                _documentArchiveDirectory,
                Mandate.TestNotifications);

            Assert.True(File.Exists(actual.DocumentFilePath));
        }

        

    }
}