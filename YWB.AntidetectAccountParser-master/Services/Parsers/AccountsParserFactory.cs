using System;
using System.Collections.Generic;
using YWB.AntidetectAccountParser.Helpers;
using YWB.AntidetectAccountParser.Model.Accounts;

namespace YWB.AntidetectAccountParser.Services.Parsers
{
    public class AccountsParserFactory
    {
        public enum AccountTypes { Google, Facebook };
        public AccountTypes AccountType { get; private set; }
        public IAccountsParser<SocialAccount> CreateParser()
        {
            
            AccountType= SelectHelper.Select(new[] { AccountTypes.Google, AccountTypes.Facebook });
            IAccountsParser<SocialAccount> parser = null;
            switch (AccountType)
            {
                case AccountTypes.Google:
                    parser = new GoogleArchivesAccountsParser();
                    break;
                case AccountTypes.Facebook:
            
                    var parsers = new Dictionary<string, Func<IAccountsParser<FacebookAccount>>> {
                        {"Accounts from text file",()=>new FacebookTextAccountsParser() },
                        {"Accounts from ZIP/RAR files or Folders",()=>new FacebookArchivesAccountsParser("", 0) }
                    };
                    parser = SelectHelper.Select(parsers, a => a.Key).Value();
                    break;
            }
            return parser;
        }
    }
}
