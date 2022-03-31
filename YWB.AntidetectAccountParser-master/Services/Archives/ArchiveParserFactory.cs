using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YWB.AntidetectAccountParser.Model.Accounts;

namespace YWB.AntidetectAccountParser.Services.Archives
{
    public class ArchiveParserFactory<T> where T:SocialAccount
    {
        public const string Folder = "logs";
        public IArchiveParser<T> GetArchiveParser(string fullDirPath)
        {
            //var fullDirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Folder);

            //var files = Directory.GetFiles(fullDirPath, "*.zip");
            //if (files.Length != 0)
            //    return new ZipArchiveParser<T>(files.ToList(), litera);
            //files = Directory.GetFiles(fullDirPath, "*.rar");
            //if (files.Length != 0)
            //    return new RarArchiveParser<T>(files.ToList());


            //DirectoryInfo di = new DirectoryInfo(fullDirPath);
            //DirectoryInfo[] diArr = di.GetDirectories().OrderBy(p => p.CreationTime.Ticks).ToArray();

            //List<string> dirs = new List<string>();
            //foreach (var item in diArr)
            //    dirs.Add(item.FullName);

            ////var dirs=Directory.GetDirectories(fullDirPath);
            //if (dirs.Count != 0)
            //    return new DirParser<T>(dirs.ToList());
            //throw new FileNotFoundException("Didn't find any ZIP/RAR archives or Folders to parse!"); }
            return null;
        }
    }
}
