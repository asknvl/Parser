using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public class RarExtractor : BaseExtractor
    {
        public RarExtractor()
        {
        }

        public override void Extract(string source, string destination, string litera, ref int startnumber)
        {
            var files = Directory.GetFiles(source, "*.rar");

            if (files == null || files.Length == 0)
                //throw new Exception("В указанной директории нет файлов с архивами RAR");
                return;

            int progress = 0;

            foreach (var file in files)
            {
                using (var archive = RarArchive.Open(file))
                {
                    //string rarpath = file.Replace(@".rar", "");
                    string fullPath = Path.GetFullPath(file).TrimEnd(Path.DirectorySeparatorChar);
                    //string rarpath = fullPath.Split(Path.DirectorySeparatorChar).Last().Replace(".rar", "");


                    //var description = rarpath.Replace(@"\", "").Replace(@"/", "");
                    //int i = description.IndexOf("(");
                    //description = description.Remove(0, i).Replace("(", "").Replace(")", "");

                    //string[] descriptions = fullPath.Split('(', ')');
                    //string description = descriptions[1];


                    string rarpath = string.Empty;

                    foreach (var entry in archive.Entries/*.Where(entry => !entry.IsDirectory)*/)
                    {
                        if (rarpath == string.Empty)
                        {
                            rarpath = entry.Key.Split(Path.DirectorySeparatorChar)[0];
                        }
                        if (!entry.Key.ToLower().Contains("macos") &&
                            !entry.Key.ToLower().Contains(".zip") &&
                            !entry.Key.ToLower().Contains(".rar"))
                            
                            entry.WriteToDirectory(destination, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }

                    //string description = "";
                    //try
                    //{
                    //    description = rarpath.Split('(', ')')[1];
                    //    if (!checkDescription(description))
                    //        throw new Exception();
                    //} catch (Exception ex)
                    //{
                    //    description = "Неверный формат";
                    //}
                    var description = getDescription(rarpath);

                    string name = $"{litera}{startnumber++}";                    
                    string litpath = Path.Combine(destination, name);

                    if (Directory.Exists(litpath))
                        Directory.Delete(litpath, true);

                    Directory.Move(Path.Combine(destination, rarpath), litpath);
                    
                    File.WriteAllText(Path.Combine(litpath, "_infostring.txt"), description);
                    File.WriteAllText(Path.Combine(litpath, "_displaystring.txt"), rarpath);                    
                }
                File.Delete(file);
                OnProgressEvent(++progress, files.Length);
            }
        }
    }
}
