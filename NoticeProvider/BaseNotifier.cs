using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;
using Notices.NoticeStorage;

namespace Notices.NoticeService
{
    public abstract class BaseNoticeProvider
    {
        protected readonly IDocumentService _documentService;
        protected readonly INoticeEmail _emailService;
        protected readonly INoticeStorage _storageService;
        protected readonly ILogger<BaseNoticeProvider> _logger;

        public BaseNoticeProvider (
            ILogger<BaseNoticeProvider> logger,
            INoticeEmail emailService,
            IDocumentService documentService,
            INoticeStorage storageService
        )
        {
            _documentService = documentService;
            _emailService = emailService;
            _storageService = storageService;
            _logger = logger;
        }

        virtual public async Task<NoticeRecord> Notify (
            string principalIdentifier,
            Mandate mandate,
            string purpose)
        {
            var information = await GetProcessingInformationFromSource (principalIdentifier, purpose);
            var documentRecord = await CreateNotificationDocument (information);
            if (documentRecord.WasSuccessful)
            {
                information.EmailParameters.Add ("DocumentName", documentRecord.DocumentName);
                information.EmailParameters.Add( "RequestKey", documentRecord.RequestKey );
                information.EmailAttachments.Add (
                    new EmailAttachment
                    {
                        DisplayName = documentRecord.DocumentName,
                        StorageName = documentRecord.DocumentName,
                    }
                );
                var result = await SendEmail (information);
                if (result.wasSuccess)
                {
                    var noticeRecord =  new NoticeRecord
                    {
                        WasSuccessful = true,
                        EmailStorageName = result.archiveFile,
                        EmployeeIdentifier = principalIdentifier,
                        NoticeStorageName = documentRecord.DocumentName,
                        ProcessMessage = "Email and Document Sent"
                    };
                    _storageService.SaveNoticeRecord(noticeRecord);
                }

                return new NoticeRecord
                {
                    WasSuccessful = false,
                        EmailStorageName = null,
                        EmployeeIdentifier = principalIdentifier,
                        NoticeStorageName = documentRecord.DocumentName,
                        ProcessMessage = "Notice was created but email failed"
                };

            }
            return new NoticeRecord
            {
                WasSuccessful = false,
                EmailStorageName = null,
                EmployeeIdentifier = principalIdentifier,
                NoticeStorageName = null,
                ProcessMessage = "Notice could not be created"
            };
        }

        public abstract Task<ProcessingInformation> GetProcessingInformationFromSource (string principalIdentifier, string purpose);
        public abstract Task<DocumentRecord> CreateNotificationDocument (ProcessingInformation principalInfo);
        virtual protected async Task < (bool wasSuccess, string archiveFile) > SendEmail (ProcessingInformation principalInformation)
        {
            var pathToEmail = await _emailService.SendNoticeEmail (principalInformation.EmailTemplate,
                principalInformation.EmailAddresses,
                principalInformation.EmailParameters);
            if (!string.IsNullOrEmpty (pathToEmail))
            {
                return (true, pathToEmail);
            }
            return (false, null);
        }

        virtual protected string GetEmailTemplate (Mandate madate, string purpose, bool isReminder)
        {
            throw new NotImplementedException ();
        }

        virtual protected Dictionary<string, string> GetReplacementDictionary (string recipient, string purpose, bool isReminder)
        {
            throw new NotImplementedException ();
        }

    }
}