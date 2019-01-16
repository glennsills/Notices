using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using Notices.DCNotifier;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;
using Notices.NoticeServiceImplementation;
using Notices.NoticeStorage;
using Notices.TestNotifiers;
using Xunit;

namespace Notices.NoticeTests.IntegrationTests
{
    public class NoticeEmailTests
    {
        private readonly string _emailTemplate;
        private readonly IOptions<NoticeEmailOptions> _testOptions;
        private NoticeStorageService _noticeStorageService;

        public NoticeEmailTests ()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            _emailTemplate = "EmailTemplate.html";

            _testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailTemplateFolder = Path.Combine (dir, @"..\..\..\IntegrationTests"),
                        EmailArchiveFolder = Path.Combine (dir, @"..\..\..\\IntegrationTests\EmailArchiveFolder"),
                        EmailSenderAddress = "email.return@address.com",
                        EmailSenderName = "Email Return",
                        SmtpHost = "127.0.0.1",
                        SmtpPort = 0
                });

            var noticeStorageOptions = Options.Create<NoticeStorageOptions>( 
            new NoticeStorageOptions{
                ConnectionString = "UseDevelopmentStorage=true",
                ApplicationName = "notice"
            });

            _noticeStorageService = new NoticeStorageService(noticeStorageOptions, null);

        }

        [Fact]
        public async Task CanSendEmail ()
        {
            var recipients = new List<string> { "george@jungle.com" };
            var parameters = new Dictionary<string, string>
                { { "Link1HREF", "Link1HREF" },
                    { "Link1Title", "Link1Title" },
                    { "Subject", "%%Reminder%%Subject with %%Link1Title%%" }
                };

            var cut = new NoticeEmail (_testOptions, null,_noticeStorageService, new FileSystem ());

            var archiveFile =  await cut.SendNoticeEmail (_emailTemplate, recipients, parameters );
            
            Assert.NotNull(archiveFile);

        }
    }
}