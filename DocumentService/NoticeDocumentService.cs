using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notices.NoticeData;
using Notices.NoticeStorage;

namespace Notices.DocumentService
{
    public class NoticeDocumentService : IDocumentService
    {
        private IFileSystem _fileSystem;
        private INoticeStorage _noticeStorage;
        private ILogger<NoticeDocumentService> _logger;
        private DocumentServiceOptions _options;

        public NoticeDocumentService (
            ILogger<NoticeDocumentService> logger,
            IFileSystem fileSystem,
            INoticeStorage noticeStorage,
            IOptions<DocumentServiceOptions> options)
        {
            _fileSystem = fileSystem;
            _noticeStorage = noticeStorage;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<DocumentRecord> CreateNoticeDocument (
            ProcessingInformation principalInformation,
            Mandate mandate)
        {

            var templatePath = GetFullPathToPDFForm (principalInformation.DocumentTemplate, _options.TemplateDirectory);
            var outputName = $"{Guid.NewGuid().ToString("N")}.pdf";
            var memorystream = new MemoryStream ();
            var outputWriter = new PdfWriter (memorystream);
            outputWriter.SetCloseStream(false);
            
            var pdf = new PdfDocument (new PdfReader (templatePath), outputWriter);
            var form = PdfAcroForm.GetAcroForm (pdf, true);
            var fields = form.GetFormFields ();
            PdfFormField toSet;

            foreach (var valuePair in principalInformation.FormParameters)
            {
                if (fields.TryGetValue (valuePair.Key, out toSet))
                {
                    toSet.SetValue (valuePair.Value);
                }
                else
                {
                    throw new DocumentServiceException ($"Key {valuePair.Key} does not exist in form parameters");
                }
            }

            form.FlattenFields ();
            pdf.Close ();
            memorystream.Seek(0, SeekOrigin.Begin);
            await _noticeStorage.UploadFileFromStream(outputName, memorystream);

            return new DocumentRecord
            {
                    DocumentName = outputName,
                    WasSuccessful = true
            };
        }

        internal string GetFullPathToPDFForm (string templateName, string documentTemplateFolder)
        {
            return Path.Combine (documentTemplateFolder, templateName);
        }
    }
}