using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer {
    public interface ILifeCicle {
        void onClose();
        void onStart();        
    }
}
