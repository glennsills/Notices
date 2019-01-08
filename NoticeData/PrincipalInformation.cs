using System.Collections.Generic;

namespace Notices.NoticeData
{
    public class PrincipalInformation
    {
        public List<string> EmailAddresses {get;set;}

        public Dictionary<string,string> EmailParameters {get;set;}

        public Dictionary<string,string> FormParameters {get;set;}

        public string DocumentTemplate {get;set;}
    }
}