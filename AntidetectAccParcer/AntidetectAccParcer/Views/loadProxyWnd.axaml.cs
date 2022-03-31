using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Input;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.IO;

namespace AntidetectAccParcer.Views {
    public partial class loadProxyWnd : Window {

        #region vars
        Grid dragContent;
        Grid grid;
        bool IsOntoDragConntent;
        TextBox textBox;
        #endregion

        public loadProxyWnd() {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            grid = this.FindControl<Grid>("DragGrid");
            textBox = this.FindControl<TextBox>("ProxiesTextBox");
            dragContent = this.FindControl<Grid>("DragContent");

            grid.AddHandler(DragDrop.DropEvent, (sender, args) =>
            {
                //Debug.WriteLine();
                var s = args.Data.GetDataFormats();

                dynamic fn = args.Data.Get("FileNames");

                List<string> names = new List<string>();
                foreach (var item in fn)
                {
                    names.Add(item);
                }

                //List<string> names = (List<string>)(args.Data.Get("FileNames"));
                string r = File.ReadAllText(Path.GetFullPath(names[0]));
                textBox.Text = r;
                dragContent.IsVisible = false;
                textBox.Focus();

            });

            grid.PointerPressed += Grid_PointerPressed;            
            grid.PointerEnter += Grid_PointerEnter;
            grid.PointerLeave += Grid_PointerLeave;

            dragContent.PointerEnter += DragContent_PointerEnter;
            dragContent.PointerLeave += DragContent_PointerLeave;
        }
              
        private void Grid_PointerEnter(object? sender, PointerEventArgs e)
        {
            Debug.WriteLine("gridContent enter");
        }

        private void Grid_PointerLeave(object? sender, PointerEventArgs e)
        {
            Debug.WriteLine("gridContent leave");
        }

        private void DragContent_PointerLeave(object? sender, PointerEventArgs e)
        {
            IsOntoDragConntent = false;
        }

        private void DragContent_PointerEnter(object? sender, PointerEventArgs e)
        {
            IsOntoDragConntent = true;
        }
        
        private void Grid_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (IsOntoDragConntent)
            {
                dragContent.IsVisible = false;
                textBox.Focus();
            } else
            {
                if (textBox.Text.Equals(""))
                {
                    dragContent.IsVisible = true;
                    grid.Focus();
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                ((ILifeCicle)DataContext).onClose();
            } catch { };
        } 

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            try
            {
                ((ILifeCicle)DataContext).onStart();
                if (!textBox.Text.Equals(""))
                {
                    dragContent.IsVisible = false;
                    textBox.Focus();
                }
            } catch { };

            

        }
    }
}
