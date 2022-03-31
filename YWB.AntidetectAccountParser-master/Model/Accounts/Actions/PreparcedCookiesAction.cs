using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Helpers;

namespace YWB.AntidetectAccountParser.Model.Accounts.Actions
{
    internal class PreparcedCookiesAction<T> : AccountAction<T> where T : FacebookAccount
    {
        public PreparcedCookiesAction()
        {
            Condition = (fileName) => fileName.Contains("multicookies");
            Action = (stream, sa) => ExtractInfo(stream, sa);
        }

        private void ExtractInfo(System.IO.Stream s, T sa)
        {          
            var content = Encoding.UTF8.GetString(s.ReadAllBytes()).Trim();
            sa.MultiCookieInstance = int.Parse(content);
        }
    }
}
