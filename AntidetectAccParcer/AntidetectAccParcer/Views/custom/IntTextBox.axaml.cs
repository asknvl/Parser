using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Text.RegularExpressions;

namespace AntidetectAccParcer.Views.custom {
    public partial class IntTextBox : TextBox, IStyleable {

        Type IStyleable.StyleKey => typeof(TextBox);

        public IntTextBox() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnTextInput(TextInputEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            base.OnTextInput(e);
        }
    }
}
