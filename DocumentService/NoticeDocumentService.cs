using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Notices.NoticeData;

namespace Notices.DocumentService
{
    public class NoticeDocumentService : IDocumentService
    {
        private IFileSystem _fileSystem;

        public NoticeDocumentService (IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Task<DocumentRecord> CreateNoticeDocument (
            PrincipalInformation principalInformation,
            string templateName,
            string documentTemplateFolder,
            string documentOutputFolder,
            Mandate mandate)
        {

            var templatePath = GetFullPathToPDFForm (templateName, documentTemplateFolder);
            var outputPath = GetFullDocumentArchivePath (documentOutputFolder, mandate);

            PdfDocument pdf = new PdfDocument (new PdfReader (templatePath), new PdfWriter (outputPath));
            PdfAcroForm form = PdfAcroForm.GetAcroForm (pdf, true);
            IDictionary<String, PdfFormField> fields = form.GetFormFields ();
            PdfFormField toSet;
            fields.TryGetValue ("Start of Calendar Year", out toSet);
            toSet.SetValue ("2019-01-01");
            form.FlattenFields ();
            pdf.Close ();

            return Task.FromResult (new DocumentRecord
            {
                DocumentFilePath = outputPath,
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