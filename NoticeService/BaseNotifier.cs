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
        private readonly INoticeEmail _emailService;
        private readonly ILogger<BaseNotifier> _logger;

        public BaseNotifier (ILogger<BaseNotifier> logger, INoticeEmail emailService)
        {
            _emailService = emailService;
            _logger = logger;
        }

        virtual public async Task<NoticeRecord> Notify (string principalIdentifier, Mandate mandate, string purpose)
        {
            var information = await GetPrincipalInformationFromSource (principalIdentifier);
            var documentRecord = await CreateNotificationDocument (information);
            if (documentRecord.WasSuccessful)
            {
                var result = await SendEmail (information);
                if (result.wasSuccess)
                {
                    return new NoticeRecord
                    {
                        WasSuccessful = true,
                            EmailArchiveFilename = result.archiveFile,
                            EmployeeIdentifier = principalIdentifier,
                            NoticeFilename = documentRecord.DocumentFilePath,
                            ProcessMessage = "Email and Document Sent"
                    };
                }

                return new NoticeRecord
                {
                    WasSuccessful = false,
                        EmailArchiveFilename = null,
                        EmployeeIdentifier = principalIdentifier,
                        NoticeFilename = documentRecord.DocumentFilePath,
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

        public abstract Task<PrincipalInformation> GetPrincipalInformationFromSource (string principalIdentifier);
        public abstract Task<DocumentRecord> CreateNotificationDocument (PrincipalInformation principalInfo);
        virtual internal Task < (bool wasSuccess, string archiveFile) > SendEmail (PrincipalInformation principalInformation)
        {
            throw new NotImplementedException ();
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