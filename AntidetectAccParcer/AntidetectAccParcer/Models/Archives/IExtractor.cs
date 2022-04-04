using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public interface IExtractor
    {
        void Extract(string source, string destination, string litera, ref int startnumber, CancellationTokenSource cts);
    }
}
