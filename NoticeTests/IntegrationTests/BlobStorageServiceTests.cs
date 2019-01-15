using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.BlobStorageService;
using Notices.NoticeData;
using Xunit;

namespace NoticeTests.IntegrationTests
{
    public class BlobStorageServiceTests
    {
        private IOptions<BlobStorageOptions> _options;
        private string _filename;

        public BlobStorageServiceTests()
        {
            _options = Options.Create<BlobStorageOptions>( 
                new BlobStorageOptions{
                    ConnectionString = "UseDevelopmentStorage=true",
                    ApplicationName = "notice"
                });

                var templateDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests");
                _filename = Path.Combine(templateDirectory, "PaidSickLeave-MandatoryNotice-English.pdf");
        }

        [Fact(DisplayName="Can store a file")]
        public async Task CanStoreARecord()
        {
            var cut = new BlobStorage(_options, null);
            await cut.UploadFile(Mandate.TestNotifications, BlobUsage.NOTICE, _filename);
        }

    }
}