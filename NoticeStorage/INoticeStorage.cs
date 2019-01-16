using System.IO;
using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.NoticeStorage
{
    public interface INoticeStorage
    {
         Task UploadFileFromStream(string fileName, Stream stream);
         Task<Stream> GetFileStream(string fileName);
    }
}