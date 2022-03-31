using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntidetectAccParcer.ViewModels
{
    public class ViewModelBase : ReactiveObject, IViewModelRequests {
        public event Action onCloseRequest;
        public void OnCloseRequest() {
            onCloseRequest?.Invoke();
        }
    }
}
