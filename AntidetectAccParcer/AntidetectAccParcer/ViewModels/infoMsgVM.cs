using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.ViewModels
{
    public class infoMsgVM : ViewModelBase
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
        public infoMsgVM(string message)
        {
            Title = "Сообщение";
            Message = message;

            #region timer
            var timer = new System.Timers.Timer(3000);            
            timer.Elapsed += (source, args) =>
            {
                Dispatcher.UIThread.InvokeAsync(() => {
                    OnCloseRequest();
                });
                
            };
            timer.Start();
            #endregion

            #region commands
            okCmd = ReactiveCommand.Create(() => {
                OnCloseRequest();
            });
            #endregion
        }
    }
}
