using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;

namespace TestNotifiers
{
    public class NotificationAsAttachmentNotificeProvider : BaseNotifier, INoticeProvider
    {
        public NotificationAsAttachmentNotificeProvider(ILogger<BaseNotifier> logger, INoticeEmail emailService, IDocumentService documentService) : 
            base (logger, emailService, documentService)
        {          
        }


        protected async override Task<(bool wasSuccess, string archiveFile)> SendEmail(PrincipalInformation principalInformation)
        {
            var pathToEmail = await _emailService.SendNoticeEmailWithAttachments(principalInformation.EmailTemplate, 
            principalInformation.EmailAddresses,
            principalInformation.EmailParameters, 
            principalInformation.Mandate,
            new List<string>{principalInformation.EmailParameters["DocumentFilePath"]});

            if ( !string.IsNullOrEmpty(pathToEmail))
            {
                return (true, pathToEmail);
            }
            return (false, null);
        }
        

        public override Task<DocumentRecord> CreateNotificationDocument(PrincipalInformation principalInfo)
        {
            throw new System.NotImplementedException();
        }

        public override Task<PrincipalInformation> GetPrincipalInformationFromSource(string principalIdentifier, string purpose)
        {
            throw new System.NotImplementedException();
        }

        public Task<NoticeRecord> Remind(string principalIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<NoticeRecord> UploadNotification(string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose)
        {
            throw new NotImplementedException();
        }
    }
}