using System;
using System.Collections.Generic;
using System.Linq;
using YWB.AntidetectAccountParser.Model.Accounts;

namespace YWB.AntidetectAccountParser.Helpers
{
    internal static class AccountNamesHelper
    {
        internal static void Process(IEnumerable<SocialAccount> accounts)
        {
            if (accounts.All(a => !string.IsNullOrEmpty(a.AccountName))) return;
            Console.Write("Enter account name prefix:");
            var namePrefix = Console.ReadLine();
            Console.Write("Enter starting index (For example, 1):");
            var sIndex = int.Parse(Console.ReadLine());
            int i = 0;
            foreach (var acc in accounts)
            {
                acc.AccountName = $"{namePrefix}{i + sIndex}";
                i++;
            }
        }
    }
}