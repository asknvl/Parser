using AntidetectAccParcer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AntidetectAccParcer.Views
{
    public partial class importWnd : Window
    {

        Grid statusGrid;

        Grid mainGrid;
        Grid dragContent;

        ListBox tagsListBox;        

        ListBox accountsListBox;
        Border infoBorder;

        TextBox accountNameTextBox;
        public Grid overlayGrid;

        Button importButton;

        public importWnd()
        {
            InitializeComponent();

            statusGrid = this.FindControl<Grid>("StatusGrid");
            statusGrid.PointerPressed += StatusGrid_PointerPressed;

            mainGrid = this.FindControl<Grid>("MainGrid");
            mainGrid.PointerPressed += MainGrid_PointerPressed;

            tagsListBox = this.FindControl<ListBox>("TagsListBox");
            tagsListBox.SelectionChanged += TagsListBox_SelectionChanged;

            accountsListBox = this.FindControl<ListBox>("AccountsListBox");
            accountsListBox.AddHandler(DragDrop.DropEvent, (sender, args) => {
                var s = args.Data.GetDataFormats();                                
                dynamic fn = args.Data.Get("FileNames");
                List<string> names = new List<string>();
                foreach (var item in fn)
                {
                    names.Add(item);
                }
                dragContent.IsVisible = false;
                ((importVM)DataContext).OnDropEvent(names);
            });

            infoBorder = this.FindControl<Border>("InfoBorder");
            infoBorder.LostFocus += InfoBorder_LostFocus;
            infoBorder.PointerLeave += InfoBorder_PointerLeave;

            accountNameTextBox = this.FindControl<TextBox>("AccountNameTextBox");
            accountNameTextBox.LostFocus += AccountsListBox_LostFocus;

            overlayGrid = this.FindControl<Grid>("OverlayGrid");
            overlayGrid.IsVisible = false;

            dragContent = this.FindControl<Grid>("DragContent");
            importButton = this.FindControl<Button>("ImportButton");            


#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void StatusGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ((importVM)DataContext)?.OnStopRequest();
        }

        private void MainGrid_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            mainGrid.Focus();                     
        }

        private void AccountsListBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ((importVM)DataContext).Restore();
        }

        private void InfoBorder_PointerLeave(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            ((importVM)DataContext).Restore();
        }

        private void InfoBorder_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            ((importVM)DataContext).SelectedAccount?.Account.Info.Save();            


            //try
            //{
            //    ((importVM)DataContext).SelectedAccount.Account.Info.Save();
            //} catch (Exception ex)
            //{

            //}            

        }

        private void TagsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (tagsListBox.SelectedItems.Contains("Без тегов"))
            {
                tagsListBox.SelectedItems.Clear();
            }
            dynamic c = this.DataContext;
            c.IsTags = tagsListBox.SelectedItems.Count > 0;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                ((ILifeCicle)DataContext).onClose();
            }
            catch { };
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            try
            {
                //((ILifeCicle)DataContext).onStart();

                ((importVM)DataContext).OnStart();
            }
            catch { };
        }

    }
}
