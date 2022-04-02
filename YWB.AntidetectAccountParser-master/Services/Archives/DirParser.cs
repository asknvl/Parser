using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Model.Accounts.Actions;

namespace YWB.AntidetectAccountParser.Services.Archives
{
    public class DirParser<T> : IArchiveParser<T> where T : SocialAccount
    {
        public List<string> Containers { get; set; }

        public DirParser(/*List<string> dirs*/string fullDirPath, string litera)
        {
            DirectoryInfo di = new DirectoryInfo(fullDirPath);
            DirectoryInfo[] diArr = di.GetDirectories();
            try
            {
                diArr = diArr.OrderBy(p => int.Parse(p.Name.Replace(litera, ""))).ToArray();

            } catch { }

            List<string> dirs = new List<string>();
            foreach (var item in diArr)
            {               
                dirs.Add(item.FullName);
            }
            
            Containers = dirs;
        }

        public T Parse(ActionsFacade<T> af, string dirPath)
        {
            
            foreach (var entry in Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories))
            {
                foreach (var a in af.AccountActions)
                {
                    if (a.Condition(entry.ToLowerInvariant()))
                    {
                        //Console.WriteLine($"{a.Message}{entry}");
                        using (var s = File.OpenRead(entry))
                        {
                            a.Action(s, af.Account);
                        }
                    }
                }                
            }

            return af.Account;
        }
    }
}
