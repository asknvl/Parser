using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.ViewModels
{
    public class errMsgVM : ViewModelBase
    {
        #region properties
        string title;
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        string message;
        public string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
        #endregion

        #region commands
        public ReactiveCommand<Unit, Unit> okCmd { get; }
        #endregion
        public errMsgVM(string message)
        {
            Title = "Ошибка!";
            Message = message;

            #region commands
            okCmd = ReactiveCommand.Create(() => {
                OnCloseRequest();
            });
            #endregion
        }
    }
}
