using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Notices.NoticeData;
using Notices.NoticeStorage;

namespace Notices.EmailService
{
    public class NoticeEmail : INoticeEmail
    {
        protected const string SubjectKey = "Subject";
        protected const string SenderAddressError = @"Please define NoticeEmailOptions.EmailSenderName and 
        NoticeEmailOptions.EmailSenderAddress in configuration";
        protected const string MissingSubjectError = "Parameters must contain a value for '{SubjectKey}'";
        protected const string MissingTemplateFolderError = @"Please define NoticeEmailOptions.EmailTemplateFolder 
        in configuration";
        protected const string MissingRecipientError = "At least one email recipient is required";
        protected const string MandateArchiveFoldernameError = @"Please define NoticeEmailOptions.EmailArchiveFolder 
        in configuration";
        private const string ParamDelimiter = "%%";
        private NoticeEmailOptions _options;
        private ILogger<NoticeEmail> _logger;
        private IFileSystem _fileSystem;
        private INoticeStorage _noticeStorage;

        public NoticeEmail (IOptions<NoticeEmailOptions> options, ILogger<NoticeEmail> logger, INoticeStorage noticeStorage, IFileSystem fileSystem)
        {
            _options = options.Value;
            _logger = logger;
            _fileSystem = fileSystem;
            _noticeStorage = noticeStorage;
        }
        public async Task<string> SendNoticeEmail (string templateFilename,
            List<string> recipients,
            Dictionary<string, string> parameters)
        {
            var message = CreateMessage (templateFilename, recipients, parameters);
            var archiveFile = await ArchiveMessage (message);
            await SendMessage (message);
            return archiveFile;

        }

        public async Task<string> SendNoticeEmailWithAttachments (
            string emailTemplate,
            List<string> emailAddresses,
            Dictionary<string, string> emailParameters,
            List<EmailAttachment> attachmentFiles)
        {
            var message = await CreateMessage (emailTemplate, emailAddresses, emailParameters, attachmentFiles);
            var archiveFile = await ArchiveMessage (message);
            await SendMessage (message);
            return archiveFile;
        }

        private  async Task<MimeMessage> CreateMessage (string emailTemplate, List<string> emailAddresses, Dictionary<string, string> emailParameters, List<EmailAttachment> attachments)
        {
            var body = LoadBody (emailTemplate, emailParameters);
            var message = new MimeMessage ();
            message.Sender = GetFromMailboxAddress ();
            message.From.Add (message.Sender);
            message.To.AddRange (GetToMailboxAddresses (emailAddresses));
            message.Subject = GetSubject (emailParameters);

            var bodyBuilder = new BodyBuilder ();
            bodyBuilder.HtmlBody = body;
            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment.DisplayName,
                await _noticeStorage.GetFileStream(attachment.StorageName));
            }

            message.Body = bodyBuilder.ToMessageBody ();
            return message;
        }

        internal MimeMessage CreateMessage (string templateFilename, List<string> recipients, Dictionary<string, string> parameters)
        {
            var body = LoadBody (templateFilename, parameters);
            var message = new MimeMessage ();
            message.Sender = GetFromMailboxAddress ();
            message.From.Add (message.Sender);
            message.To.AddRange (GetToMailboxAddresses (recipients));
            message.Subject = GetSubject (parameters);
            var bodyBuilder = new BodyBuilder ();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody ();
            return message;
        }

        internal async Task SendMessage (MimeMessage message)
        {

            using (var client = new SmtpClient ())
            {
                try
                {
                    await client.ConnectAsync (_options.SmtpHost, _options.SmtpPort);
                    //Todo worry about authentication later.
                    await client.SendAsync (message);
                }
                catch (Exception ex)
                {
                    _logger.LogError (ex.Message);
                    throw new EmailServiceException (ex);
                }
            };

        }

        internal async Task<string> ArchiveMessage (MimeMessage message)
        {
            string filename = $"{Guid.NewGuid().ToString("N")}.eml";
            try
            {
                var memoryStream = new MemoryStream ();

                message.WriteTo (memoryStream);

                memoryStream.Seek (0, SeekOrigin.Begin);

                await _noticeStorage.UploadFileFromStream (filename, memoryStream);

                return filename;
            }
            catch (Exception ex)
            {
                _logger.LogError (ex.Message, ex);
                throw new EmailServiceException (ex);
            }

        }

        internal string GetCompleteFilename (string mandateArchiveFoldername)
        {
            var basePath = EnsureFolder (mandateArchiveFoldername);
            return Path.Combine (basePath, $"{Guid.NewGuid().ToString("N")}.eml");

        }

        internal string EnsureFolder (string mandateArchiveFoldername)
        {
            if (string.IsNullOrWhiteSpace (_options.EmailArchiveFolder))
            {
                throw new EmailServiceException (MandateArchiveFoldernameError);
            }

            var basePath = Path.Combine (_options.EmailArchiveFolder, mandateArchiveFoldername);

            try
            {
                var di = _fileSystem.DirectoryInfo.FromDirectoryName (basePath);
                if (!di.Exists)
                {
                    di.Create ();
                }
                return basePath;
            }
            catch (Exception ex)
            {
                _logger.LogError ($"Unable to create archive folder {mandateArchiveFoldername} in {_options.EmailTemplateFolder}");
                throw new EmailServiceException (ex);
            }

        }
        internal string GetSubject (Dictionary<string, string> parameters)
        {
            if (parameters != null && parameters.ContainsKey (SubjectKey))
            {
                var subjectValue = parameters[SubjectKey];
                if (!string.IsNullOrWhiteSpace (subjectValue))
                {
                    return RemoveMissingParameters (ReplaceParameters (subjectValue, parameters));
                }
            }
            throw new EmailServiceException (MissingSubjectError);
        }

        internal List<MailboxAddress> GetToMailboxAddresses (List<string> recipientAddresses)
        {
            if (recipientAddresses == null || recipientAddresses.Count == 0)
                throw new EmailServiceException (MissingRecipientError);

            var result = new List<MailboxAddress> ();
            recipientAddresses.ForEach (address => result.Add (new MailboxAddress (address)));
            return result;

        }
        internal MailboxAddress GetFromMailboxAddress ()
        {
            if (string.IsNullOrWhiteSpace (_options.EmailSenderName) || string.IsNullOrWhiteSpace (_options.EmailSenderAddress))
            {
                throw new EmailServiceException (SenderAddressError);
            }
            return new MailboxAddress (_options.EmailSenderName, _options.EmailSenderAddress);
        }

        internal string LoadBody (string templateFilename, Dictionary<string, string> parameters)
        {
            var template = LoadTemplate (templateFilename);
            return ReplaceParameters (template, parameters);

        }

        internal string ReplaceParameters (string template, Dictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                parameters.Keys.ToList ().ForEach (k => template = template.Replace ($"{ParamDelimiter}{k}{ParamDelimiter}", parameters[k]));
            }
            return template;
        }

        internal string LoadTemplate (string templateFilename)
        {
            if (string.IsNullOrWhiteSpace (_options.EmailTemplateFolder))
            {
                throw new EmailServiceException (MissingTemplateFolderError);
            }
            var filePath = Path.Combine (_options.EmailTemplateFolder, templateFilename);
            try
            {
                return _fileSystem.File.ReadAllText (filePath);
            }
            catch (Exception ex)
            {
                throw new EmailServiceException (ex);
            }
        }

        internal string RemoveMissingParameters (string templateString)
        {
            if (!string.IsNullOrEmpty (templateString))
            {
                var paramStart = -1;
                while ((paramStart = templateString.IndexOf (ParamDelimiter)) > -1)
                {
                    var paramEnd = templateString.IndexOf (ParamDelimiter, paramStart + 1);
                    templateString = templateString.Remove (paramStart, paramEnd - paramStart + 2);
                }
            }

            return templateString;
        }

    }
}