using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Options;
using MimeKit;
using Notices.DCNotifier;
using Notices.EmailService;
using Notices.NoticeService;
using Notices.NoticeServiceImplementation;
using Notices.TestNotifiers;
using Xunit;

namespace Notices.NoticeTests.UnitTests
{

    public class NoticeEmailTests
    {

        private MockFileSystem fileSystem;
        private string testHtml = @"<p>You have been notified</p><p> Click here to see your notice <a href='%%NoticeHREF%%'>%%LinkTitle%%</a></p>";
        public NoticeEmailTests ()
        {
            fileSystem = new MockFileSystem (new Dictionary<string, MockFileData>
            { { @"c:\approot\emailTemplates\testtemplate.html", new MockFileData (testHtml) },
            });
        }


 
        [Fact (DisplayName = "Missing Template Folder Option")]
        public void LoadTemplateThrowsExceptionWhenMessingTemplateFolderOption ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null,null, fileSystem);
                cut.LoadTemplate ("testfolder");
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
        }

        [Fact (DisplayName = "LoadTemplate Returns Correct Value")]
        public void LoadTemplateReturnsData ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailTemplateFolder = @"c:\approot\emailTemplates"
                });

            string expected = testHtml;

            var cut = new NoticeEmail (testOptions, null,null, fileSystem);
            var actual = cut.LoadTemplate ("testtemplate.html");
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "LoadTemplate Throws Exception on Bad Filenme")]
        public void LoadTemplateThrowsOnBadTemplateFilename ()
        {

            IOptions<NoticeEmailOptions> testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailTemplateFolder = @"c:\approot\emailTemplates"
                });

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.LoadTemplate ("BadFile.html");
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
        }

        [Fact (DisplayName = "ReplaceParameters Replaces Parameters")]
        public void ReplaceParametersReplacesEverything ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var template = "token1=%%Token1%%, token2=%%Token2%%, token3=%%Token3%%";
            var parameters = new Dictionary<string, string>
                { { "Token1", "token1" },
                    { "Token2", "token2" },
                    { "Token3", "token3" },
                };

            var expected = "token1=token1, token2=token2, token3=token3";
            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.ReplaceParameters (template, parameters);
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "LoadBody Returns Correct Body")]
        public void LoadBodyReturnsBody ()
        {
            IOptions<NoticeEmailOptions> testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailTemplateFolder = @"c:\approot\emailTemplates"
                });

            string expected = @"<p>You have been notified</p><p> Click here to see your notice <a href='NoticeHREF'>LinkTitle</a></p>";

            var parameters = new Dictionary<string, string>
                { { "NoticeHREF", "NoticeHREF" },
                    { "LinkTitle", "LinkTitle" }
                };

            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.LoadBody ("testtemplate.html", parameters);
            Assert.Equal (expected, actual, true, true);
        }

        [Fact (DisplayName = "GetFromMailboxAddress Throws When No EmailSenderName option")]
        public void GetFromMailboxAddressThrowsExceptionNoEmailSenderNameOption ()
        {
            IOptions<NoticeEmailOptions> testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailSenderAddress = "address@somewhere.com"
                });

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetFromMailboxAddress ();
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetFromMailboxAddress Throws When No EmailSenderAddress option")]
        public void GetFromMailboxAddressThrowsExceptionNoEmailSenderAddressOption ()
        {
            IOptions<NoticeEmailOptions> testOptions = Options.Create<NoticeEmailOptions> (
                new NoticeEmailOptions
                {
                    EmailSenderAddress = "someName"
                });

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetFromMailboxAddress ();
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetFromMailboxAddress Throws When No EmailSender options")]
        public void GetFromMailboxAddressThrowsExceptionNoEmailSenderOptions ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetFromMailboxAddress ();
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetFromMailboxAddress Returns Proper Value")]
        public void GetFromMailboxAddressReturnsValue ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions
            {
                EmailSenderAddress = "someone@somewhere.com",
                    EmailSenderName = "someone"
            });

            var expected = new MailboxAddress (testOptions.Value.EmailSenderName, testOptions.Value.EmailSenderAddress);

            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.GetFromMailboxAddress ();

            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetToMailboxAddresses Throws For Empty Recipient Address List")]
        public void GetToMailboxAddressesThrowsExceptionEmptyAddressList ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetToMailboxAddresses (new List<string> ());
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetToMailboxAddresses Throws For null Recipient Address List")]
        public void GetToMailboxAddressesThrowsExceptionForNullAddressList ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetToMailboxAddresses (null);
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetToMailboxAddresses Returns Proper Mailbox Addresses")]
        public void GetToMailboxAddressesReturnsProperMailboxAddresses ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var recipientList = new List<string>
            {
                "some1@somewhere.com",
                "some2@somewhereelse.com"
            };
            var expected = new List<MailboxAddress>
            {
                new MailboxAddress ("some1@somewhere.com"),
                new MailboxAddress ("some2@somewhereelse.com")

            };

            var cut = new NoticeEmail (testOptions, null, null,  fileSystem);
            var actual = cut.GetToMailboxAddresses (recipientList);

            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetSubject Throws Exception If There is No Subject Parameter")]
        public void GetSubjectThrowExceptionIfThereIsNotSubjectParameter ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var parameters = new Dictionary<string, string> ();

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetSubject (null);
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetSubject Throws Exception If Subject Parameter is Null")]
        public void GetSubjectThrowExceptionIfSubjectParameterIsNull ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var parameters = new Dictionary<string, string> () { { "Subject", null } };

            var expected = typeof (EmailServiceException);
            Type actual = null;
            try
            {
                var cut = new NoticeEmail (testOptions, null, null, fileSystem);
                cut.GetSubject (parameters);
            }
            catch (Exception ex)
            {
                actual = ex.GetType ();
            }
            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetSubject Returns Subject")]
        public void GetSubjectReturnsSubject ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var parameters = new Dictionary<string, string> () { { "Subject", "subject" } };

            var expected = parameters["Subject"];

            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.GetSubject (parameters);

            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetSubject Returns Subject With Replacments")]
        public void GetSubjectReturnsSubjectWithReplacments ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var parameters = new Dictionary<string, string> ()
                { { "Subject", "%%Reminder%%Subject with %%Replacement1%% and %%Replacement2%%" }, { "Reminder", "Reminder - " }, { "Replacement1", "replacement1" }, { "Replacement2", "replacement2" }
                };

            var expected = "Reminder - Subject with replacement1 and replacement2";

            var cut = new NoticeEmail (testOptions, null, null,  fileSystem);
            var actual = cut.GetSubject (parameters);

            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "GetSubject With Missing Replacement Parameter Still Looks OK")]
        public void GetSubjectReturnsSubjectThatLooksOKWhenAParameterIsMissing ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var parameters = new Dictionary<string, string> ()
                { { "Subject", "%%Reminder%%Subject with %%Replacement1%% and %%Replacement2%%" }, { "Replacement1", "replacement1" }, { "Replacement2", "replacement2" }
                };

            var expected = "Subject with replacement1 and replacement2";

            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.GetSubject (parameters);

            Assert.Equal (expected, actual);
        }

        [Fact (DisplayName = "Remove Missing Parameters Removes Them")]
        public void RemoveMissingParametersRemovesThem ()
        {
            var testOptions = Options.Create<NoticeEmailOptions> (new NoticeEmailOptions ());
            var templateString = "This is a template with a %%Parameters%%";
            var expected = "This is a template with a ";
            var cut = new NoticeEmail (testOptions, null, null, fileSystem);
            var actual = cut.RemoveMissingParameters (templateString);

            Assert.Equal (expected, actual);
        }
    }
}