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


namespace Notices.DocumentService
{
    public class NoticeDocumentService : IDocumentService
    {
        private IFileSystem _fileSystem;
        private ILogger<NoticeDocumentService> _logger;
        private DocumentServiceOptions _options;

        public NoticeDocumentService (ILogger<NoticeDocumentService> logger, IFileSystem fileSystem, IOptions<DocumentServiceOptions> options)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _options = options.Value;
        }

        public Task<DocumentRecord> CreateNoticeDocument (
            PrincipalInformation principalInformation,
            Mandate mandate)
        {

            var templatePath = GetFullPathToPDFForm (principalInformation.DocumentTemplate, _options.TemplateDirectory);
            var outputPath = GetFullDocumentArchivePath (_options.ArchiveDirectory, mandate);

            PdfDocument pdf = new PdfDocument (new PdfReader (templatePath), new PdfWriter (outputPath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm (pdf, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields ();
            PdfFormField toSet;

            foreach(var valuePair in principalInformation.FormParameters)
            {
                if (fields.TryGetValue (valuePair.Key, out toSet))
                {
                    toSet.SetValue(valuePair.Value);
                }
                else {
                    throw new DocumentServiceException($"Key {valuePair.Key} does not exist in form parameters");
                }
            }

            form.FlattenFields ();
            pdf.Close ();

            return Task.FromResult (new DocumentRecord
            {
                DocumentFilePath = outputPath,
                DocumentName = Path.GetFileName(outputPath),
                WasSuccessful = true
            });
        }

        internal string GetFullDocumentArchivePath (string documentOutputFolder, Mandate mandate)
        {
            var basepath= EnsureArchiveFolder(documentOutputFolder, mandate);
            var filename = $"{Guid.NewGuid().ToString("N")}.pdf";
            return Path.Combine(basepath, filename);
        }

        internal string EnsureArchiveFolder(string documentOutputFolder, Mandate mandate)
        {
            var folderPath = Path.Combine(documentOutputFolder, mandate.ToString());
            var di = _fileSystem.DirectoryInfo.FromDirectoryName(folderPath);
            if (!di.Exists)
            {
                di.Create();
            }
            return folderPath;
        }

        internal string GetFullPathToPDFForm (string templateName, string documentTemplateFolder)
        {
            return Path.Combine(documentTemplateFolder, templateName);
        }
    }
}