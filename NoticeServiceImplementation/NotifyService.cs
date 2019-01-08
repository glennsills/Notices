using Notices.NoticeService;
using Notices.DCNotifier;
using Notices.TestNotifiers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Notices.NoticeData;

namespace Notices.NoticeServiceImplementation
{
    public class NotifyService : INotify
    {
        private readonly INotify _dcNotifier;
        private readonly INotify _testNotifier;
        private readonly Dictionary<Mandate, INotify> _mandateProviderMap;

        public NotifyService (IDCWageNotifier dcNotifier, ITestNotifier testNotifier)
        {
            _dcNotifier = dcNotifier;
            _testNotifier = testNotifier;

            _mandateProviderMap = new Dictionary<Mandate, INotify>()
            {
                {Mandate.DCWageNotifications, _dcNotifier},
                {Mandate.TestNotifications, _testNotifier}
            };
        }

        public async Task<NoticeRecord> Notify(string employeeIdentifier, Mandate mandate, string purpose)
        {
            var provider = _mandateProviderMap[mandate];
            return await provider.Notify(employeeIdentifier, mandate, purpose);
        }

        public async Task<NoticeRecord> Remind(string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAddress)
        {
            var provider = _mandateProviderMap[mandate];
            return await provider.Remind(employeeIdentifier, mandate, notificationId, alternateEmailAddress);
            
        }

        public Task<NoticeRecord> UploadNotification(string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose)
        {
            throw new NotImplementedException();
        }
    }
}