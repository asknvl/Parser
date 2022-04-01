using AntidetectAccParcer.ViewModels;
using AntidetectAccParcer.Views;
using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer
{
    public class WindowService : IWindowService
    {

        static WindowService instance;
        Window main;
        List<Window> windowList;
        PixelPoint startupPosition;


        private WindowService()
        {
            windowList = new List<Window>();
        }

        public static WindowService getInstance()
        {
            if (instance == null)
            {
                instance = new WindowService();
            }
            return instance;
        }

        public void ShowWindow(ViewModelBase vm)
        {

            Window wnd = null;
            
            if (vm is initialVM)
            {
                wnd = new initWnd();
            }
            if (vm is importVM)
            {
                wnd = new importWnd();
                main = wnd;
                startupPosition = main.Position;
                main.PositionChanged += Main_PositionChanged;
            }

            wnd.DataContext = vm;
            windowList.Add(wnd);
            vm.onCloseRequest += () => { wnd.Close(); windowList.Remove(wnd); };
            wnd.Show();

        }

        private void Main_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            int x = main.Position.X;
            int y = main.Position.Y;
            int dx = x - startupPosition.X;
            int dy = y - startupPosition.Y;
            startupPosition = new PixelPoint(x, y);

            foreach (var item in windowList)
            {
                if (item is importWnd)
                    continue;

                item.Position = new PixelPoint(item.Position.X + dx, item.Position.Y + dy);

            }            
        }

        public void ShowDialog(ViewModelBase vm)
        {
            Window wnd = null;

            if (vm is loadProxyVM)
            {
                wnd = new loadProxyWnd();
            }

            if (vm is errMsgVM)
            {
                wnd = new errMsgWnd();
            }

            wnd.DataContext = vm;
            windowList.Add(wnd);
            wnd.Closed += (s, e) =>
            {
                main.IsEnabled = true;
            };
            vm.onCloseRequest += () =>
            {
                wnd.Close();
            };

            main.IsEnabled = false;

            wnd.Show(main);
        }

        public void ShowDialog(ViewModelBase vm, ViewModelBase parent)
        {

            Window wnd = null;

            if (vm is loadProxyVM)
            {
                wnd = new loadProxyWnd();
            }

            if (vm is errMsgVM)
            {
                wnd = new errMsgWnd();
            }

            if (vm is initialVM)
            {
                wnd = new initWnd();
            }

            if (vm is infoMsgVM)
            {
                wnd = new infoMsgWnd();
            }

            wnd.DataContext = vm;
            windowList.Add(wnd);
            wnd.Closed += (s, e) =>
            {
                var p = windowList.FirstOrDefault(w => w.DataContext == parent);

                if (windowList.Count <= 2)
                {
                    p.IsEnabled = true;
                    if (p is importWnd)
                    {
                        ((importWnd)p).overlayGrid.IsVisible = false;
                    }
                }
            };
            vm.onCloseRequest += () =>
            {
                wnd.Close();
                windowList.Remove(wnd);
            };

            var p = windowList.FirstOrDefault(w => w.DataContext == parent);
            p.IsEnabled = false;

            if (p is importWnd)
            {
                ((importWnd)p).overlayGrid.IsVisible = true;
            }

            //wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;           

            wnd.Show(main);
            wnd.Position = new PixelPoint(main.Position.X + (int)((main.Width - wnd.Width) / 2), main.Position.Y + 30 + (int)((main.Height - wnd.Height) / 2));
        }

        public async Task<string> ShowFileDialog(string title, ViewModelBase parent)
        {
            var dialog = new OpenFolderDialog() { Title = title };
            var p = windowList.FirstOrDefault(w => w.DataContext == parent);
            return await dialog.ShowAsync(p);
        }

        private void Wnd_Closed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
