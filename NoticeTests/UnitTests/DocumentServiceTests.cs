using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Options;
using Notices.DocumentService;
using Notices.NoticeData;
using Notices.NoticeService;
using Xunit;

namespace Notices.NoticeTests.UnitTests
{
    public class DocumentServiceTests
    {

        public DocumentServiceTests ()
        { }

        [Fact(DisplayName="Ensure Archive returns correct folder with Mandate name")]
        public void EnsureArchiveFolderReturnsPath ()
        {
            var cut = new NoticeDocumentService(null,SetupFileSystem(), GetDocumentOptions());
            var expected = @"c:\approot\archive\TestNotifications";
            var actual = cut.EnsureArchiveFolder(@"c:\approot\archive", Mandate.TestNotifications);
            Assert.Equal(expected,actual);
        }


        [Fact(DisplayName="GetFullDoucmentArchivePath returns the path to the PDF output file")]
        public void GetFullDocumentArchivePathReturnsPathToPDF()
        {
            var cut = new NoticeDocumentService(null,SetupFileSystem(), GetDocumentOptions());
            var actual = cut.GetFullDocumentArchivePath(@"c:\approot\archive", Mandate.TestNotifications);
            Assert.EndsWith(".pdf", actual);
        }

        [Fact(DisplayName="GetFullPathToPDFForm returns path to PDF Form file")]
        public void GetFullPathToPDFFormReturnsPathToPDFForm()
        {
             var cut = new NoticeDocumentService(null,SetupFileSystem(), GetDocumentOptions());
             var expected = @"c:\approot\templatefolder\TestForm.pdf";
            var actual = cut.GetFullPathToPDFForm("TestForm.pdf", @"c:\approot\templatefolder\");
            Assert.Equal(expected, actual);           
        }

        private IOptions<DocumentServiceOptions> GetDocumentOptions()
        {
            throw new NotImplementedException();
        }

        private MockFileSystem SetupFileSystem ()

        {

            return new MockFileSystem (new Dictionary<string, MockFileData>

                {

                    { MockUnixSupport.Path (@"c:\approot\archive\TestNotifications\a.pdf"), new MockFileData ("Demo content") },

                    { MockUnixSupport.Path (@"c:\approot\archive\TestNotifications\b.pdf"), new MockFileData ("Demo content") },

                });

        }
    }
}