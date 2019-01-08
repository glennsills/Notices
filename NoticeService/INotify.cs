using System;
using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.NoticeService
{
    public interface INotify
    {
        Task<NoticeRecord> Notify (string principalIdentifier, Mandate mandate, string purpose);

        Task<NoticeRecord> UploadNotification(string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose );

        Task<NoticeRecord> Remind(string principalIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAddress);
     }
}