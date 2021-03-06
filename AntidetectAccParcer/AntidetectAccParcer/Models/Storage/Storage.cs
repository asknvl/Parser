using AntidetectAccParcer.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AntidetectAccParcer.Models.Storage {
    public class Storage<T> : IStorage<T> {

        #region vars
        T t;
        string path;
        #endregion

        public Storage(string path, T t) {
            this.path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), path);
            this.t = t;
        }

        #region public
        public T load() {

            if (!File.Exists(path)) {
                save(t);
            }

             string rd = File.ReadAllText(path);

             var p = JsonConvert.DeserializeObject<T>(rd);

             return p;
            

        }

        public void save(T p) {

            var json = JsonConvert.SerializeObject(p, Formatting.Indented);
            try {

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllText(path, json);

            }
            catch (Exception ex) {
                throw new Exception("Не удалось сохранить файл JSON");
            }

        }
        #endregion
    }
}
