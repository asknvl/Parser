using AntidetectAccParcer.Models.Proxies;
using AntidetectAccParcer.Models.Storage;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;

namespace AntidetectAccParcer.ViewModels
{
    public class loadProxyVM : ViewModelBase, ILifeCicle
    {

        #region vars
        IWindowService ws = WindowService.getInstance();
        IStorage<loadProxyVM> storage;
        #endregion

        #region properties
        bool socks5;
        [JsonProperty]
        public bool IsSocks5
        {
            get => socks5;
            set => this.RaiseAndSetIfChanged(ref socks5, value);   
        }

        bool http;
        [JsonProperty]
        public bool IsHttp
        {
            get => http;
            set => this.RaiseAndSetIfChanged(ref http, value);
        }

        public bool ssh;
        [JsonProperty]
        public bool IsSsh
        {
            get => ssh; set => this.RaiseAndSetIfChanged(ref ssh, value);
        }

        string textProxies;
        [JsonProperty]
        public string TextProxies
        {
            get => textProxies;
            set
            {
                this.RaiseAndSetIfChanged(ref textProxies, value);
                AllowConfirm = !textProxies.Equals("");
            }
        }

        string prefix;
        [JsonProperty]
        public string Prefix
        {
            get => prefix;
            set => this.RaiseAndSetIfChanged(ref prefix, value);
        }

        bool allowConfirm;
        public bool AllowConfirm
        {
            get => allowConfirm;
            set => this.RaiseAndSetIfChanged(ref allowConfirm, value);
        }
        #endregion

        #region commands
        public ReactiveCommand<Unit, Unit> cancelCmd { get; }
        public ReactiveCommand<Unit, Unit> acceptCmd { get; }
        #endregion

        public loadProxyVM()
        {
            storage = new Storage<loadProxyVM>("proxy.json", this);

            #region init
            IsSocks5 = true;
            TextProxies = "";
            Prefix = "";
            #endregion

            #region commands
            cancelCmd = ReactiveCommand.Create(() =>
            {
                OnCancel?.Invoke();
                OnCloseRequest();
            });

            acceptCmd = ReactiveCommand.Create(() =>
            {
                try
                {
                    IProxyParcer parcer = new TextProxyParcer();
                    List<Proxy> res = parcer.Parse(TextProxies, getProtocol(), Prefix);
                    OnProxiesLoaded?.Invoke(res);
                    OnCloseRequest();

                } catch (Exception ex)
                {
                    ws.ShowDialog(new errMsgVM(ex.Message), this);
                }
            });
            #endregion
        }

        #region helpers
        ProxyProtocol getProtocol()
        {
            List<bool> protocols = new List<bool> { IsSocks5, IsHttp, IsSsh };
            int index = protocols.IndexOf(true);            
            return (ProxyProtocol)index;            
        }       
        #endregion

        #region callbacks
        public event Action<List<Proxy>> OnProxiesLoaded;
        public event Action OnCancel;
        public void onClose()
        {
            storage.save(this);
        }

        public void onStart()
        {
            var t = storage.load();
            IsSocks5 = t.IsSocks5;
            IsHttp = t.IsHttp;
            IsSsh = t.IsSsh;
            Prefix = t.Prefix;
            TextProxies = t.TextProxies;
        }
        #endregion
    }
}
