using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AntidetectAccParcer.Views
{
    public partial class errMsgWnd : Window
    {
        public errMsgWnd()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
