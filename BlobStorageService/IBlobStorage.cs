using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.BlobStorageService
{
    public interface IBlobStorage
    {
         Task UploadFile(Mandate mandate, BlobUsage blobUsage, string sourceFilePath);
    }
}