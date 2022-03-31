using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    internal class DirExtractor : BaseExtractor
    {
        public override void Extract(string source, string destination, string litera, ref int startnumber)
        {
            var dirs = Directory.GetDirectories(source);
            if (dirs == null || dirs.Length == 0)
                return;

            int progress = 0;

            foreach (var folder in dirs)
            {

                string[] files = Directory.GetFiles(folder);
                var _info = files.FirstOrDefault(p => p.Contains("_info.json"));
                if (_info != null)
                    continue;

                string s = new DirectoryInfo(folder).Name;
                var description = getDescription(s);

                string name = $"{litera}{startnumber++}";
                string litpath = Path.Combine(destination, name);

                if (Directory.Exists(litpath))
                    Directory.Delete(litpath, true);

                Directory.Move(folder, litpath);
                Directory.GetCreationTime(litpath);

                //File.WriteAllText(Path.Combine(litpath, "infostring.txt"), descriptions[0]);
                File.WriteAllText(Path.Combine(litpath, "_infostring.txt"), description);
                File.WriteAllText(Path.Combine(litpath, "_displaystring.txt"), s);
                OnProgressEvent(++progress, dirs.Length);
                
            }
        }
    }
}
