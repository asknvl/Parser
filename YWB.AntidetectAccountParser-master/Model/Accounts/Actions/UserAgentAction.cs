using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Helpers;

namespace YWB.AntidetectAccountParser.Model.Accounts.Actions
{
    public class UserAgentAction<T> : AccountAction<T> where T : FacebookAccount
    {
        public UserAgentAction()
        {
            Condition = (fileName) => fileName.Contains("agent");
            Action = (stream, sa) => ExtractUserAgent(stream, sa);
        }

        private void ExtractUserAgent(System.IO.Stream s, T sa)
        {
            var content = Encoding.UTF8.GetString(s.ReadAllBytes()).Trim();
            sa.UserAgent = content;
        }
    }
}
