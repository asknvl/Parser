using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AntidetectAccParcer.Views
{
    public partial class ProcessProxyWnd : Window
    {
        public ProcessProxyWnd()
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
