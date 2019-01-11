using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;

namespace Notices.TestNotifiers
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
            return  _documentService.CreateNoticeDocument(principalInfo, Mandate.TestNotifications);
        }

        public override Task<PrincipalInformation> GetPrincipalInformationFromSource(string principalIdentifier, string purpose)
        {
            var principalInfo = new PrincipalInformation
            {
                EmailAddresses = new List<string>{"someone@somewhere.com"},
                EmailParameters = new Dictionary<string,string>{
                     {"Link1HREF", "https://www.google.com"},
                     {"Link1Title", "Just Google It."},
                     {"Subject", "Consider just googling it."}
                },
                FormParameters = new Dictionary<string, string>{
                    {"Start of Calendar Year", "2019-01-01"},
                    {"End of Calendar Year", "2019-12-31"}
                },
                DocumentTemplate = "PaidSickLeave-MandatoryNotice-English.pdf",   
                EmailTemplate = "EmailTemplate.html"            
            };
            return Task.FromResult(principalInfo);
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