using AntidetectAccParcer.ViewModels;
using AntidetectAccParcer.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;

namespace AntidetectAccParcer
{    
    public class App : Application
    {

        WindowService ws = WindowService.getInstance();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {

                importVM import = new importVM();
                ws.ShowWindow(import);
                //initialVM init = new initialVM();                
                //init.onContinue += Init_onContinue;
                //ws.ShowWindow(init);
            }

            base.OnFrameworkInitializationCompleted();
        }

        //private void Init_onContinue(InitParameters parameters) {
        //    importVM import = new importVM(parameters);
        //    ws.ShowWindow(import);
        //}
    }
}
