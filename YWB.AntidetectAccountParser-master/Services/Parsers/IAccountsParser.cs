using System;
using System.Collections.Generic;
using System.Threading;
using YWB.AntidetectAccountParser.Model.Accounts;

namespace YWB.AntidetectAccountParser.Services.Parsers
{
    public interface IAccountsParser<out A> where A : SocialAccount
    {
        IEnumerable<A> Parse(string path, CancellationTokenSource cts);
        event Action<int, int> ParceEvent;
    }
}
