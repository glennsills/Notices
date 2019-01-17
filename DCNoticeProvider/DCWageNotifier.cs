using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;
using Notices.NoticeStorage;

namespace Notices.DCNotifier
{
    public class DCWageNotifier : BaseNoticeProvider, IDCWageNotifier
    {

        public DCWageNotifier (ILogger<BaseNoticeProvider> logger, INoticeEmail emailService, IDocumentService documentService, INoticeStorage storageService) : base (logger, emailService, documentService, storageService) { }

        public Task<NoticeRecord> Remind (string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAddress)
        {
            throw new NotImplementedException ();
        }

        override public Task<ProcessingInformation> GetProcessingInformationFromSource (string principalIdentifier, string purpose)
        {
            var now = DateTime.Now;
            return Task.FromResult (
                new ProcessingInformation
                {
                    DocumentTemplate = "Notice of Hire-English_OWH Revised_OPA edits.pdf",
                        EmailAddresses = new List<string> { "person.to.be.notified@gmail.com" },
                        FormParameters = new Dictionary<string, string>
                        { 
                            { "AtHireBox", "checked"},
                            { "EffectiveDate", now.ToShortDateString () },
                            { "Company Name", "ACME Inventions" },
                            { "Permanent Address", "1 One Way" },
                            { "City_2", "Black Hole" },
                            { "State_2", "Arizona" },
                            { "Zip Code_2", "85002" },
                            { "EmployerMailingAddressSameAsPhysicalAddress", "checked" }
                        },
                        EmailTemplate = "DC_Wage_Notice_Template.html",
                        EmailParameters = new Dictionary<string, string>
                        {   
                            { "Subject", "Your District of Columnbia Wage Notification"},
                            { "Firstname", "Wiley" },
                            { "Lastname", "Coyote" },
                            { "BaseWebAddress", "https://localhost:5001/notices"},
                            { "Link1Title", "DC Wage Notification"},
                        }

                }

            );
        }

        public Task<NoticeRecord> UploadNotification (string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose)
        {
            throw new NotImplementedException ();
        }

        public override async Task<DocumentRecord> CreateNotificationDocument (ProcessingInformation processingInfo)
        {
           var documentRecord =   await _documentService.CreateNoticeDocument(processingInfo, Mandate.TestNotifications);
           if (documentRecord.WasSuccessful)
           {
                documentRecord.RequestKey = Guid.NewGuid().ToString("N");
                processingInfo.EmailParameters.Add("Link1HREF", documentRecord.RequestKey)  ;
           }
           return documentRecord;
        }

    }
}