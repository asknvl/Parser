using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer {
    public interface IViewModelRequests {
        event Action onCloseRequest;
    }
}
