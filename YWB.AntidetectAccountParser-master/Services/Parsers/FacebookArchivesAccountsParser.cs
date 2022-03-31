using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YWB.AntidetectAccountParser.Helpers;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Model.Accounts.Actions;

namespace YWB.AntidetectAccountParser.Services.Parsers
{
    public class FacebookArchivesAccountsParser : AbstractArchivesAccountsParser<FacebookAccount>
    {
        
        public FacebookArchivesAccountsParser(string litera, int startcounter) {
            Litera = litera;
            StartCounter = startcounter;
        }

        public override ActionsFacade<FacebookAccount> GetActions(string filePath)
        {
            var fa = new FacebookAccount(Path.GetFileNameWithoutExtension(filePath));
            fa.Path = filePath;

            //Console.WriteLine($"Parsing file: {filePath}");
            return new ActionsFacade<FacebookAccount>()
            {
                Account = fa,
                AccountActions = new List<AccountAction<FacebookAccount>>()
                {
                    new PasswordAccountAction<FacebookAccount>(),
                    new WasParsedAction<FacebookAccount>(),
                    new PreparcedCookiesAction<FacebookAccount>(),
                    new CookiesAccountAction<FacebookAccount>(),
                    new TokenAccountAction<FacebookAccount>(),
                    new UserAgentAction<FacebookAccount>(),
                    new InfoAccountAction<FacebookAccount>(),
                    new DisplayInfoAccountAction<FacebookAccount>()
                }
            };

        }

        public override AccountValidity IsValid(FacebookAccount fa)
        {
            if (fa.AllCookies.Any(c => CookieHelper.HasCUserCookie(c)))
            {
                var uid=CookieHelper.GetCUserCookie(fa.AllCookies);

                //var ch=FbHeadersChecker.Check(uid);
                var ch = true;

                if (!ch) return AccountValidity.Invalid;
                return AccountValidity.Valid;
            }
            else if (fa.Login != null && fa.Password != null)
                return AccountValidity.PasswordOnly;
            else
                return AccountValidity.Invalid;
        }

        public override IEnumerable<FacebookAccount> MultiplyCookies(IEnumerable<FacebookAccount> accounts)
        {
            var finalRes = new List<FacebookAccount>();
            //If we have cookies from multiple accounts we should create an account for each cookie set
            int cntr = StartCounter;

            foreach (var fa in accounts)
            {
                if (fa.AllCookies.Count == 1 || fa.WasParced)
                {
                    if (fa.WasParced)
                    {
                        fa.AccountName = $"{Litera}{cntr++}";
                    }

                    finalRes.Add(fa);
                    continue;
                }

                for (int i = 0; i < fa.AllCookies.Count && fa.MultiCookieInstance == 0; i++)
                {
                    var cookies = fa.AllCookies[i];

                    var newFa = new FacebookAccount()
                    {
                        Path = fa.Path,
                        AccountInfoString = fa.AccountInfoString,
                        Birthday = fa.Birthday,
                        BmLinks = fa.BmLinks,
                        Cookies = cookies,
                        EmailLogin = fa.EmailLogin,
                        EmailPassword = fa.EmailPassword,
                        Logins = fa.Logins,
                        Passwords = fa.Passwords,
                        LoginsPasswords = fa.LoginsPasswords,
                        DisplayInfoString = fa.DisplayInfoString,
                        Token = fa.Token,
                        TwoFactor = fa.TwoFactor,
                        UserAgent = fa.UserAgent,
                        AccountName = $"{fa.AccountName}.{i + 1}",
                        MultiCookieInstance = i + 1
                    };
                    finalRes.Add(newFa);
                }

            }
            return finalRes;
        }
    }
}
