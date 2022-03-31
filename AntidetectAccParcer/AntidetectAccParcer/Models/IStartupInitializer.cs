using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models {
    internal interface IStartupInitializer {
        void load(string path);
        void save(string path);

    }
}
