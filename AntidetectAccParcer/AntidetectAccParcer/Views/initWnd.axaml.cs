using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;

namespace AntidetectAccParcer.Views {
    public partial class initWnd : Window {

        Grid mainGrid;

        public initWnd() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            mainGrid = this.FindControl<Grid>("MainGrid");
            mainGrid.PointerPressed += MainGrid_PointerPressed;
        }

        private void MainGrid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            mainGrid.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            try
            {
                ((ILifeCicle)DataContext).onClose();
            } catch { };
        }

        protected override void OnOpened(EventArgs e) {
            base.OnOpened(e);
            try
            {
                ((ILifeCicle)DataContext).onStart();
            } catch { };
        }

        
    }
}
