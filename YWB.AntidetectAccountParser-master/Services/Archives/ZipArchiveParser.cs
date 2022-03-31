using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Model.Accounts.Actions;

namespace YWB.AntidetectAccountParser.Services.Archives
{
    public class ZipArchiveParser<T> : IArchiveParser<T> where T : SocialAccount
    {
        public List<string> Containers { get; set; }
        string Litera { get; }
        int Counter { get; set; }

        public ZipArchiveParser(List<string> archives, string litera)
        {
            Containers = archives;
            Litera = litera;
        }


        //public T Parse(ActionsFacade<T> af, string filePath)
        //{
        //    using (var archive = ZipFile.OpenRead(filePath))
        //    {
        //        foreach (var entry in archive.Entries)
        //        {
        //            foreach (var a in af.AccountActions)
        //            {
        //                if (a.Condition(entry.FullName.ToLowerInvariant()))
        //                {
        //                    ""($"{a.Message}{entry.FullName}");
        //                    using (var s = entry.Open())
        //                    {
        //                        a.Action(s,af.Account);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return af.Account;
        //}

        public T Parse(ActionsFacade<T> af, string filePath)
        {
            string dirpath = Directory.GetParent(filePath).FullName;
            string zippath = filePath.Replace(@".zip", "");

            Directory.CreateDirectory(dirpath);

            if (Directory.Exists(zippath))
                Directory.Delete(zippath, true);

            string descriprion = "";
            using (var archive = ZipFile.OpenRead(filePath))
            {
                descriprion = archive.Entries[0].FullName;
            }

            ZipFile.ExtractToDirectory(filePath, dirpath);

            string name = $"{Litera}{++Counter}";
            string litpath = Path.Combine(dirpath, name);
            if (Directory.Exists(litpath))
                Directory.Delete(litpath, true);

            Directory.Move(zippath, litpath);

            string[] files = Directory.GetFiles(litpath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                foreach (var a in af.AccountActions)
                {
                    if (a.Condition(file.ToLowerInvariant()))
                    {                        
                        using (var s = File.Open(file, FileMode.Open))
                        {
                            a.Action(s, af.Account);
                        }
                    }
                }
            }

            //using (var archive = ZipFile.OpenRead(filePath))
            //{
            //    foreach (var entry in archive.Entries)
            //    {
            //        foreach (var a in af.AccountActions)
            //        {
            //            if (a.Condition(entry.FullName.ToLowerInvariant()))
            //            {
            //                ""($"{a.Message}{entry.FullName}");
            //                using (var s = entry.Open())
            //                {
            //                    a.Action(s, af.Account);
            //                }
            //            }
            //        }
            //    }
            //}

            af.Account.AccountName = name;
            af.Account.AccountInfoString = descriprion;

            return af.Account;
        }

    }
}
