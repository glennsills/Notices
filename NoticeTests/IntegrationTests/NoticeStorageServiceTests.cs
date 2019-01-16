using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Notices.NoticeStorage;
using Notices.NoticeData;
using Xunit;

namespace NoticeTests.IntegrationTests
{
    public class NoticeStorageServiceTests
    {
        private IOptions<NoticeStorageOptions> _options;
        private string _filePath;

        public NoticeStorageServiceTests()
        {
            _options = Options.Create<NoticeStorageOptions>( 
                new NoticeStorageOptions{
                    ConnectionString = "UseDevelopmentStorage=true",
                    ApplicationName = "notice"
                });

                var templateDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\IntegrationTests");
                _filePath = Path.Combine(templateDirectory, "NoticeStorageServiceTestFile.txt");
        }

        [Fact(DisplayName="Can store a file")]
        public async Task CanStoreARecord()
        {
            var cut = new NoticeStorageService(_options, null);
            FileStream inputStream = new FileStream(_filePath, FileMode.Open);
            await cut.UploadFileFromStream( "NoticeStorageServiceTestFile.txt", inputStream);
            inputStream.Close();
        }

    }
}