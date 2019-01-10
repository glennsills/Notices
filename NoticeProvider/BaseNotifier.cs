using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;

namespace Notices.NoticeService
{
    public abstract class BaseNotifier
    {
        protected readonly IDocumentService _documentService;
        protected readonly INoticeEmail _emailService;
        protected readonly ILogger<BaseNotifier> _logger;

        public BaseNotifier (ILogger<BaseNotifier> logger, INoticeEmail emailService, IDocumentService documentService)
        {
            _documentService = documentService;
            _emailService = emailService;
            _logger = logger;
        }

        virtual public async Task<NoticeRecord> Notify (string principalIdentifier, Mandate mandate, string purpose)
        {
            var information = await GetPrincipalInformationFromSource (principalIdentifier, purpose);
            var documentRecord = await CreateNotificationDocument (information);
            if (documentRecord.WasSuccessful)
            {
                information.EmailParameters.Add("DocumentName", documentRecord.DocumentName);
                information.EmailParameters.Add("DocumentFilePath", documentRecord.DocumentFilePath);
                var result = await SendEmail (information);
                if (result.wasSuccess)
                {
                    return new NoticeRecord
                    {
                        WasSuccessful = true,
                            EmailArchiveFilename = result.archiveFile,
                            EmployeeIdentifier = principalIdentifier,
                            NoticeFilename = documentRecord.DocumentName,
                            ProcessMessage = "Email and Document Sent"
                    };
                }

                return new NoticeRecord
                {
                    WasSuccessful = false,
                        EmailArchiveFilename = null,
                        EmployeeIdentifier = principalIdentifier,
                        NoticeFilename = documentRecord.DocumentName,
                        ProcessMessage = "Notice was created but email failed"
                };

            }
            return new NoticeRecord
            {
                WasSuccessful = false,
                    EmailArchiveFilename = null,
                    EmployeeIdentifier = principalIdentifier,
                    NoticeFilename = null,
                    ProcessMessage = "Notice could not be created"
            };
        }

        public abstract Task<PrincipalInformation> GetPrincipalInformationFromSource (string principalIdentifier, string purpose);
        public abstract Task<DocumentRecord> CreateNotificationDocument (PrincipalInformation principalInfo);
        virtual protected async Task < (bool wasSuccess, string archiveFile) > SendEmail (PrincipalInformation principalInformation)
        {
            var pathToEmail = await _emailService.SendNoticeEmail(principalInformation.EmailTemplate, 
            principalInformation.EmailAddresses,
            principalInformation.EmailParameters, 
            principalInformation.Mandate);
            if ( !string.IsNullOrEmpty(pathToEmail))
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