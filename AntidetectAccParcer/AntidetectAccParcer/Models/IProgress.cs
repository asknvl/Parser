using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models
{
    public interface IProgress
    {        
        event Action<int, int> ProgressEvent;
        void OnProgressEvent(int progress, int total);
    }
}
