using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public class ZipExtractor : BaseExtractor
    {
        public ZipExtractor()
        {
        }

        public override void Extract(string source, string destination, string litera, ref int startnumber)
        {
            var files = Directory.GetFiles(source, "*.zip");

            if (files == null || files.Length == 0)
                //throw new Exception("В указанной директории нет файлов с архивами ZIP");
                return;

            int progress = 0;

            foreach (var file in files)
            {
                using (var archive = ZipFile.Open(file, ZipArchiveMode.Update))
                {

                    //var description = archive.Entries[0].FullName;

                    //try
                    //{
                    //    description = description.Replace(@"\", "").Replace(@"/", "");
                    //    int i = description.IndexOf("(");
                    //    description = description.Remove(0, i).Replace("(", "").Replace(")", "");
                    //} catch (Exception ex) {
                    //    description = "Неверный формат";
                    //} 

                    List<ZipArchiveEntry> entries = new List<ZipArchiveEntry>();
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.ToLower().Contains("macos") || 
                            entry.FullName.ToLower().Contains(".zip") || 
                            entry.FullName.ToLower().Contains(".rar"))

                            entries.Add(entry);
                    }

                    for (int i = 0; i < entries.Count; i++)
                        entries[i].Delete();

                    //var todel = archive.Entries.Where(o => o.FullName.ToLower().Contains("macos"));
                    //||
                    //                                  o.FullName.ToLower().Contains(".zip") ||
                    //                                  o.FullName.ToLower().Contains(".rar"));




                    var splt = file.Split(Path.DirectorySeparatorChar);
                    string s = splt.FirstOrDefault(p => p.Contains(".zip"));
                    s = s.Replace(".zip", "");

                    var description = getDescription(s);

                    archive.ExtractToDirectory(destination, true);

                    string name = $"{litera}{startnumber++}";
                    string litpath = Path.Combine(destination, name);

                    if (Directory.Exists(litpath))
                        Directory.Delete(litpath, true);

                    Directory.Move(Path.Combine(destination, archive.Entries[0].FullName), litpath);

                    File.WriteAllText(Path.Combine(litpath, "_infostring.txt"), description);
                    File.WriteAllText(Path.Combine(litpath, "_displaystring.txt"), s);
                }
                File.Delete(file);
                OnProgressEvent(++progress, files.Length);                
            }            
        }
    }
}
