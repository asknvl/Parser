//using Avalonia.Media.Imaging;
using ReactiveUI;
using AntidetectAccParcer.ViewModels;
using YWB.AntidetectAccountParser.Model;
using RestSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;
using Svg;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AntidetectAccParcer.Models.Proxies
{
    public class ProxyParameters : ViewModelBase
    {
        #region const
        Dictionary<string, string> proxyGeoExceptions = new Dictionary<string, string>() {
            { "ee-w1.makeadsafe.com", "EE" },
            { "ee-w2.makeadsafe.com", "EE" },
            { "int-w1.makeadsafe.com", "UA" },
        };
        #endregion

        #region properties
        bool ischecked;
        [JsonProperty]
        public bool IsChecked
        {
            get => ischecked;
            set
            {
                this.RaiseAndSetIfChanged(ref ischecked, value);
                ProxyCheckedEvent?.Invoke(this);

            }
        }

        Bitmap flag;
        public Bitmap Flag
        {
            get => flag;
            set => this.RaiseAndSetIfChanged(ref flag, value);
        }

        string country;
        [JsonProperty]
        public string Country
        {
            get => country;
            set => this.RaiseAndSetIfChanged(ref country, value);
        }

        string path;
        public string Path
        {
            get => path;
            set => this.RaiseAndSetIfChanged(ref path, value);
        }

        Proxy proxy;
        [JsonProperty]
        public Proxy Proxy
        {
            get => proxy;
            set => this.RaiseAndSetIfChanged(ref proxy, value);

        }
        #endregion

        #region vars
        IProxyChecker proxyChecker;
        #endregion
        public ProxyParameters(Proxy proxy, IProxyChecker checker)
        {
            Proxy = proxy;
            proxyChecker = checker;

        }

        #region public
        public async Task<bool> Check()
        {
            bool res = true;
            dynamic p = null;

#if DEBUG
            Random random = new Random();
            string[] countries = new string[] {
                "AB",
                "AO",
                "CN",
                "EE",
                "GH",
                "HN",
                "IS",
                "IR",
                "SM",
                "EE"
            };
            Country = countries[random.Next(countries.Length)];
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            Flag = new Bitmap(assets.Open(new Uri($"avares://AntidetectAccParcer/Assets/{Country}.png")));
#else


            p = await proxyChecker.Check(Proxy.Address);
            res = p.success;

            if (res)
            {
                Country = p.country_code;
                if (proxyGeoExceptions.ContainsKey(Proxy.Address))
                    Country = proxyGeoExceptions[Proxy.Address];
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                Flag = new Bitmap(assets.Open(new Uri($"avares://AntidetectAccParcer/Assets/{Country}.png")));
            } else
                throw new Exception("Не удалось проверить прокси");

#endif

            return res;
        }
        #endregion

        #region callbacks
        public event Action<ProxyParameters> ProxyCheckedEvent;
        #endregion
    }
}
