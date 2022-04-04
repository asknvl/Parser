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

        public override void Extract(string source, string destination, string litera, ref int startnumber, CancellationTokenSource cts)
        {
            var files = Directory.GetFiles(source, "*.zip");

            if (files == null || files.Length == 0)
                //throw new Exception("В указанной директории нет файлов с архивами ZIP");
                return;

            int progress = 0;

            foreach (var file in files)
            {

                cts?.Token.ThrowIfCancellationRequested();

                using (var archive = ZipFile.Open(file, ZipArchiveMode.Update))
                {

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

                    string zippath = archive.Entries[0].FullName;
                    char[] sep = new char[] { '\\', '/', Path.DirectorySeparatorChar };
                    var sp = zippath.Split(sep);
                    zippath = sp[0];
                                        
                    var description = getDescription(zippath);

                    archive.ExtractToDirectory(destination, true);

                    string name = $"{litera}{startnumber++}";                    
                    string litpath = Path.Combine(destination, name);

                    if (Directory.Exists(litpath))
                        Directory.Delete(litpath, true);
                                        
                    Directory.Move(Path.Combine(destination, zippath), litpath);
                    
                    File.WriteAllText(Path.Combine(litpath, "_infostring.txt"), description);
                    File.WriteAllText(Path.Combine(litpath, "_displaystring.txt"), zippath);
                }
                File.Delete(file);
                OnProgressEvent(++progress, files.Length);                
            }            
        }
    }
}
