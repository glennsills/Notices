using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.DocumentService
{
    public interface IDocumentService
    {
        Task<DocumentRecord> CreateNoticeDocument(
             PrincipalInformation principalInformation, 
             Mandate mandate);
    }
}