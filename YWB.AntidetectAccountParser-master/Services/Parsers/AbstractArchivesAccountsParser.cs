using System.Collections.Generic;
using System.IO;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Model.Accounts.Actions;
using YWB.AntidetectAccountParser.Services.Archives;
using YWB.AntidetectAccountParser.Services.Proxies;

namespace YWB.AntidetectAccountParser.Services.Parsers
{
    public enum AccountValidity { Valid, PasswordOnly, Invalid }    

    public abstract class AbstractArchivesAccountsParser<T> : IAccountsParser<T> where T : SocialAccount
    {

        public string Litera { get; set; }
        public int StartCounter { get; set; }

        public event System.Action<int, int> ParceEvent;

        public IEnumerable<T> Parse(string path)
        {
            var apf = new ArchiveParserFactory<T>();
            var ap = new DirParser<T>(path, Litera);//apf.GetArchiveParser(path);
            List<T> accounts = new List<T>();
            //var proxyProvider = new FileProxyProvider(); //pfg
            //var proxies=proxyProvider.Get();

            for (int i = 0; i < ap.Containers.Count; i++)
            {
                string archive = ap.Containers[i];
                var actions = GetActions(archive);
                var acc = ap.Parse(actions, archive);  
                var validity = IsValid(acc);
                

                ParceEvent?.Invoke(i, ap.Containers.Count);

                //acc.CoocieCntr = 0;                

                switch (validity)
                {
                    case AccountValidity.Valid:
                        //if (proxies.Count==ap.Containers.Count) acc.Proxy=proxies[i];
                        accounts.Add(acc);
                        break;
                    case AccountValidity.PasswordOnly:
                        //if (proxies.Count==ap.Containers.Count) acc.Proxy=proxies[i];
                        acc.AccountName = $"PasswordOnly_{acc.AccountName}";
                        accounts.Add(acc);
                        break;
                    case AccountValidity.Invalid:
                        var invalid = Path.Combine(ArchiveParserFactory<T>.Folder, "Invalid");
                        if (!Directory.Exists(invalid)) Directory.CreateDirectory(invalid);
                        DirectoryInfo a = new DirectoryInfo(archive);
                        DirectoryInfo p = a.Parent;
                        Directory.Move(archive,  Path.Combine(p.ToString(), acc.AccountName + "-inv"));
                        break;
                }
            }

            ParceEvent?.Invoke(100, 100);
            return MultiplyCookies(accounts);

        }

        public abstract ActionsFacade<T> GetActions(string filePath);
        public abstract AccountValidity IsValid(T account);
        public abstract IEnumerable<T> MultiplyCookies(IEnumerable<T> accounts);
    }
}
