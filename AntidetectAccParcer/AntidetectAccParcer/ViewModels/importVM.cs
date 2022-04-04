using AntidetectAccParcer.Models.Browsers;
using AntidetectAccParcer.Models.Proxies;
using AntidetectAccParcer.Models.Storage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Services.Browsers;
using YWB.AntidetectAccountParser.Services.Parsers;
using YWB.AntidetectAccountParser.Services.Proxies;
using AntidetectAccParcer.Models.Archives;
using YWB.AntidetectAccountParser.Model;
using Avalonia.Controls;
using System.Threading;
using TextCopy;
using System.IO;
using Newtonsoft.Json;
using YWB.AntidetectAccountParser.Services.Currency;
using Avalonia.Threading;

namespace AntidetectAccParcer.ViewModels
{
    public class importVM : ViewModelBase, ILifeCicle
    {
        #region vars
        IWindowService ws = WindowService.getInstance();
        IStorage<importVM> storage;
        IProxyProvider<SocialAccount> fileProxyProvider;
        AbstractAntidetectApiService browser;
        IBrowserDataProvider browserData;
        IAccountsParser<FacebookAccount> parcer;
        List<AccountVM> memAccounts = new List<AccountVM>();
        initialVM init;
        CancellationTokenSource cts;
        #endregion

        #region properties
        InitParameters parameters;
        [JsonProperty]
        public InitParameters Parameters
        {
            get => parameters;
            set => this.RaiseAndSetIfChanged(ref parameters, value);
        }

        List<string> tags;
        public List<string> Tags
        {
            get => tags;
            set => this.RaiseAndSetIfChanged(ref tags, value);
        }

        [JsonProperty]
        public ObservableCollection<string> SelectedTags { get; set; } = new ObservableCollection<string>();

        ObservableCollection<ProxyParameters> proxies;
        public ObservableCollection<ProxyParameters> Proxies
        {
            get => proxies;
            set => this.RaiseAndSetIfChanged(ref proxies, value);
        }
        List<Proxy> massImportProxies { get; set; }
        [JsonProperty]
        public ObservableCollection<ProxyParameters> MassImportProxies { get; set; }
        List<Proxy> browserProxies { get; set; }
        [JsonProperty]
        public ObservableCollection<ProxyParameters> BrowserProxies { get; set; }

        ObservableCollection<AccountVM> accounts;
        public ObservableCollection<AccountVM> Accounts
        {
            get => accounts;
            set => this.RaiseAndSetIfChanged(ref accounts, value);
        }
        private ObservableCollection<AccountVM> MemAccounts { get; set; } = new ObservableCollection<AccountVM>();

        AccountVM selectedAccount;
        public AccountVM SelectedAccount
        {
            get => selectedAccount;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedAccount, value);
            }
        }
        [JsonProperty]
        public bool IsAllProxyCheckedBrowserStore { get; set; }

        bool needNotifyCheck { get; set; } = true;

        bool isAllProxyCheckedBrowser;        
        public bool IsAllProxyCheckedBrowser
        {
            get => isAllProxyCheckedBrowser;
            set
            {
                if (BrowserProxies != null && BrowserProxies.Count > 0 && needNotifyCheck)
                {
                    foreach (var proxy in BrowserProxies)
                        proxy.IsChecked = value;
                }
                this.RaiseAndSetIfChanged(ref isAllProxyCheckedBrowser, value);
                
            }
        }

        [JsonProperty]
        public bool IsAllProxyCheckedMassStore { get; set; }
        bool isAllProxyCheckedMass;        
        public bool IsAllProxyCheckedMass
        {
            get => isAllProxyCheckedMass;
            set
            {
                if (MassImportProxies != null && MassImportProxies.Count > 0 && needNotifyCheck)
                {
                    foreach (var proxy in MassImportProxies)
                        proxy.IsChecked = value;
                }                
                this.RaiseAndSetIfChanged(ref isAllProxyCheckedMass, value);
               
            }
        }

        bool allowProxyChange = false;
        public bool AllowProxyChange
        {
            get => allowProxyChange;
            set => this.RaiseAndSetIfChanged(ref allowProxyChange, value);
        }

        bool isAllAccountsChecked;
        public bool IsAllAccountsChecked
        {
            get => isAllAccountsChecked;
            set
            {
                if (Accounts != null && Accounts.Count > 0 && needNotifyCheck)
                {
                    foreach (var item in Accounts)
                        item.IsChecked = value;
                }
                this.RaiseAndSetIfChanged(ref isAllAccountsChecked, value);
            }
        }

        private bool isProxies;
        public bool IsProxies
        {
            get => isProxies;
            set
            {
                this.RaiseAndSetIfChanged(ref isProxies, value);
                AllowExport = isProxies & IsAccounts & IsTags;
            }
        }

        private bool isAccounts;
        public bool IsAccounts
        {
            get => isAccounts;
            set
            {
                this.RaiseAndSetIfChanged(ref isAccounts, value);
                AllowExport = isAccounts & IsProxies & IsTags;
            }
        }

        private bool isTags;
        public bool IsTags
        {
            get => isTags;
            set
            {
                this.RaiseAndSetIfChanged(ref isTags, value);
                AllowExport = isTags & IsProxies & IsAccounts;
            }
        }

        private bool allowExport;
        public bool AllowExport
        {
            get => allowExport;
            set => this.RaiseAndSetIfChanged(ref allowExport, value);
        }
    
        int progress;
        public int Progress
        {
            get => progress;
            set => this.RaiseAndSetIfChanged(ref progress, value);
        }

        string searchName;
        public string SearchName
        {
            get => searchName;
            set
            {

                if (!value.Equals(""))
                    Accounts = new ObservableCollection<AccountVM>(MemAccounts.Where(p => p.Account.AccountName.Contains(value)));
                else
                    Accounts = MemAccounts;

                this.RaiseAndSetIfChanged(ref searchName, value);
                //else
                //    Accounts = memAccounts;
            }
        }

        LoginPassword selectedPassword;
        public LoginPassword SelectedPassword
        {
            get => selectedPassword;
            set
            {
                if (value == null)
                    return;
                Clipboard clipboard = new();
                clipboard.SetText(value.Password);
                this.RaiseAndSetIfChanged(ref selectedPassword, value);
            }
        }

        bool isDragTextVisible;
        public bool IsDragTextVisible
        {
            get => isDragTextVisible;
            set => this.RaiseAndSetIfChanged(ref isDragTextVisible, value);
        }             

        bool allowImport;
        public bool AllowImport
        {
            get => allowImport;
            set => this.RaiseAndSetIfChanged(ref allowImport, value);
        }

        bool fileImport;
        public bool FileImport
        {
            get => fileImport;
            set => this.RaiseAndSetIfChanged(ref fileImport, value);
        }
        #endregion

        #region commands        
        public ReactiveCommand<Unit, Unit> callParameters { get; set; }
        public ReactiveCommand<Unit, Unit> loadBrowserProxies { get; }
        public ReactiveCommand<Unit, Unit> loadFileProxies { get; }
        public ReactiveCommand<Unit, Unit> loadAccounts { get; }
        public ReactiveCommand<Unit, Unit> exportAccounts { get; }
        public ReactiveCommand<Unit, Unit> copyData { get; set; }
        public ReactiveCommand<Unit, Unit> massProxyImport { get; }
        #endregion
        public importVM()
        {
            #region dependencies
            storage = new Storage<importVM>("import.json", this);
            fileProxyProvider = new FileProxyProvider();
            browser = new Octo();
            browserData = (IBrowserDataProvider)browser;
            var ExtractorFactory = new ExtractorFactory();          
            #endregion

            #region init            
            IsDragTextVisible = true;            
            IsAllAccountsChecked = true;
            AllowImport = true;
            AllowProxyChange = false;

            Parameters = new InitParameters();
            init = new initialVM();
            init.onContinue += Init_onContinue;
            init.Update();
            #endregion

            #region commands
            callParameters = ReactiveCommand.Create(() =>
            {
                ws.ShowDialog(init, this);
            });

            loadBrowserProxies = ReactiveCommand.CreateFromTask(async () =>
            {
                FileImport = false;
                Proxies = BrowserProxies;
                IsProxies = Proxies.Any(p => p.IsChecked);
            });

            loadProxyVM vm = new loadProxyVM();
            vm.OnProxiesLoaded += async (proxies) =>
            {
                massImportProxies = proxies;
                var t = storage.load();
                MassImportProxies = await getProxies(massImportProxies, t.MassImportProxies, IsAllProxyCheckedMass, MassProxyCheckedEvent, false);
                Proxies = MassImportProxies;
                IsProxies = Proxies.Any(p => p.IsChecked);
            };

            massProxyImport = ReactiveCommand.CreateFromTask(async () =>
            {               
                ws.ShowDialog(vm, this);
            });

            loadFileProxies = ReactiveCommand.CreateFromTask(async () =>
            {
                FileImport = true;
                Proxies = MassImportProxies;
                IsProxies = Proxies.Any(p => p.IsChecked);
            });

            loadAccounts = ReactiveCommand.CreateFromTask(async () =>
            {
                //IsAccounts = false;
                //string path = await ws.ShowFileDialog("Выберите директорию с аккаунтами", this);
                //await importAccounts(path);

                IsAccounts = false;
                string path = await ws.ShowFileDialog("Выберите директорию с аккаунтами", this);
                if (path == null)
                    return;
                List<string> lpath = new List<string>() { path };
                await Task.Run(() =>
                {
                    OnDropEvent(lpath);
                });


            });

            exportAccounts = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    int res = 0;
                    await Task.Run(async () =>
                    {
                        switch (parameters.Destination)
                        {
                            case ImportDestination.octo:
                                res = await exportToOcto();
                                break;
                        }
                    });
                    exportResultMsg(res);
                } catch (OperationCanceledException ex)
                {
                    infoMsg("Действие отменено");
                } 
                catch (Exception ex)
                {
                    errMsg(ex.Message);
                } finally
                {
                    Progress = 0;
                }
            });

            copyData = ReactiveCommand.Create(() =>
            {
                if (SelectedAccount != null)
                {
                    Clipboard clipboard = new();
                    clipboard.SetText(SelectedAccount.Account.Info.ToString());
                }
            });
            #endregion

        }

        #region helpers    
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Директория не найдена: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        async Task importAccounts(string path)
        {

            AllowImport = false;
            Accounts = new ObservableCollection<AccountVM>();
            MemAccounts.Clear();
            SelectedAccount = null;            
            cts = new CancellationTokenSource();

            if (path != null)
            {
                IsDragTextVisible = false;

                try
                {
                    Progress = 0;

                    parcer = new FacebookArchivesAccountsParser(Parameters.Litera, Parameters.StartNumber);
                    parcer.ParceEvent += (p, t) =>
                    {
                        Progress = (int)(p * 100 / t);
                    };

                    List<FacebookAccount> accs = new List<FacebookAccount>();
                    await Task.Run(() =>
                    {

                        BaseExtractor extractor = new ComboExtractor();
                        extractor.ProgressEvent += (p, t) =>
                        {
                            Progress = (int)(p * 100 / t);
                        };
                        int num = Parameters.StartNumber;
                        extractor.Extract(path, path, Parameters.Litera, ref num, cts);

                        accs = parcer.Parse(path, cts).ToList();
                        List<string> toDeleteList = new List<string>();

                        for (int i = 0; i < accs.Count; i++)
                        {
                            Progress = (int)(i * 100 / accs.Count);
                            if (accs[i].WasParced)
                            {
                                DirectoryInfo dp = Directory.GetParent(accs[i].Path);
                                string tmppath = Path.Combine(dp.FullName, "tmp", accs[i].AccountName);
                                string newpath = Path.Combine(dp.FullName, accs[i].AccountName);
                                CopyDirectory(accs[i].Path, tmppath, true);
                                Directory.Delete(accs[i].Path, true);
                                accs[i].Path = newpath;
                                continue;
                            }
                            if (!accs[i].Path.Contains(accs[i].AccountName))
                            {
                                DirectoryInfo dp = Directory.GetParent(accs[i].Path);
                                string newpath = Path.Combine(dp.FullName, accs[i].AccountName);
                                CopyDirectory(accs[i].Path, newpath, true);
                                if (!toDeleteList.Contains(accs[i].Path))
                                    toDeleteList.Add(accs[i].Path);
                                accs[i].Path = newpath;
                                File.WriteAllText(Path.Combine(newpath, "multicookies"), $"{accs[i].MultiCookieInstance}");
                            }
                        }

                        string tmpPath = "";
                        foreach (var item in accs)
                        {
                            DirectoryInfo dp = Directory.GetParent(item.Path);
                            string tmppath = Path.Combine(dp.FullName, "tmp", item.AccountName);
                            if (tmpPath.Equals(""))
                                tmpPath = Directory.GetParent(tmppath).FullName;

                            if (Directory.Exists(tmppath))
                            {
                                CopyDirectory(tmppath, item.Path, true);
                            }
                        }

                        foreach (var item in toDeleteList)
                        {
                            Directory.Delete(item, true);
                        }

                        if (Directory.Exists(tmpPath))
                            Directory.Delete(tmpPath, true);

                        accs.ForEach(acc =>
                        {
                            acc.Info.LoadFromFile();
                            acc.SavePasswords();
                        });

                        Progress = 100;

                    });

                    //Accounts = new ObservableCollection<AccountVM>();
                    //MemAccounts.Clear();

                    foreach (var acc in accs)
                    {
                        AccountVM a = new AccountVM(acc);
                        a.OnAccountChecked += A_OnAccountChecked;

                        a.IsChecked = IsAllAccountsChecked;
                        Accounts.Add(a);
                        MemAccounts.Add(a);
                    }

                    Accounts[0].IsChecked = true;
                    SelectedAccount = Accounts[0];

                    Progress = 0;

                } catch (OperationCanceledException ex)
                {
                    infoMsg("Действие отменено");                    
                }
                
                catch (Exception ex)
                {
                    errMsg(ex.Message);
                } finally
                {
                    Progress = 0;
                    AllowImport = true;
                } 

            } else
                errMsg("Не выбрана директория с аккаунтами");
            
        }
        void errMsg(string msg)
        {            
            Dispatcher.UIThread.InvokeAsync(() => {
                ws.ShowDialog(new errMsgVM(msg), this);                
            });
            Progress = 0;
        }
        void infoMsg(string msg)
        {
            Dispatcher.UIThread.InvokeAsync(() => {
                ws.ShowDialog(new infoMsgVM(msg), this);
            });            
        }
        void exportResultMsg(int res)
        {
            string r = "" + res;
            string expening = "";

            if ((r.EndsWith("1") || r.EndsWith("01")) && !r.EndsWith("11"))
                expening = "";
            else
                expening = "о";

            string akending = "нт";
            if (r.EndsWith("1"))
                akending = "";
            if (r.EndsWith("2") || r.EndsWith("3") || r.EndsWith("4"))
                akending = "а";
            if (r.EndsWith("0") || r.EndsWith("5") || r.EndsWith("6") || r.EndsWith("7") || r.EndsWith("8") || r.EndsWith("9") || r.EndsWith("11") || r.EndsWith("12") || r.EndsWith("13") || r.EndsWith("14"))
                akending = "ов";

            infoMsg($"Экспортирован{expening} {res} аккаунт{akending}");
        }
        async Task<int> exportToOcto()
        {
            

            var proxies = Proxies.Where(p => p.IsChecked).ToList();
            if (proxies.Count == 0)
                throw new Exception("Не выбран ни один прокси");

            int i = 0;
            foreach (var item in Accounts)
            {
                if (item.IsChecked)
                {
                    var proxyIndex = i < proxies.Count - 1 ? i : i % proxies.Count;
                    item.Account.Proxy = proxies[proxyIndex].Proxy;
                    i++;
                }
            }
                        
            var accounts = Accounts.Where(p => p.IsChecked).Select(a => (SocialAccount)a.Account).ToList();
            int itteration = 0;

            cts = new CancellationTokenSource();
            var profiles = await browser.ImportAccountsAsync(accounts, parameters.OS.ToString(), SelectedTags.ToArray(), (p, t) => { Progress = (int)(itteration++ * 100 / (t * 2)); }, cts);

            Progress = 100;

            return profiles.Count;
        }
        void stop()
        {
            cts?.Cancel();
        }
        async Task<ObservableCollection<ProxyParameters>> getProxies(List<Proxy> loaded, 
                                                                     ObservableCollection<ProxyParameters> stored,
                                                                     bool allChecked,
                                                                     Action<ProxyParameters> onChecked, bool sort)
        {
            List<ProxyParameters> res = new List<ProxyParameters>();
            IProxyChecker checker = new ipwhoisProxyChecker();

            List<Proxy>? tmp = (loaded != null) ? loaded : stored?.Select(o => o.Proxy).ToList();
            if (tmp == null)
                return new ObservableCollection<ProxyParameters>(res);

            Progress = 0;
            int cntr = 0;

            foreach (var proxy in tmp)
            {
                ProxyParameters p = new ProxyParameters(proxy, checker);

                if (await p.Check())
                {
                    var s = stored?.FirstOrDefault(ps => ps.Proxy.Address.Equals(p.Proxy.Address) && ps.Proxy.Title.Equals(p.Proxy.Title));
                    bool storedChecked = false;
                    if (s != null)
                    {
                        storedChecked = s.IsChecked;
                    }

                    p.ProxyCheckedEvent += onChecked;
                    p.IsChecked = allChecked | storedChecked;                    
                    
                    res.Add(p);
                    Progress = (int)(++cntr * 100 / tmp.Count);
                }
            }

            Progress = 0;
            if (sort)
                res = res.OrderBy(o => o.Proxy.Title).ToList();           

            return new ObservableCollection<ProxyParameters>(res);
        }
        #endregion

        #region public
        public void Restore()
        {
            try
            {
                SelectedAccount?.Restore();
            } catch (Exception ex)
            {
                errMsg(ex.Message);
            }
        }
        public void onClose()
        {
            IsAllProxyCheckedBrowserStore = IsAllProxyCheckedBrowser;
            IsAllProxyCheckedMassStore = IsAllProxyCheckedMass;
            storage.save(this);
        }       

        public async void onStart()
        {
            var t = storage.load();
            Parameters = t.parameters;

            try
            {
                openexchangerates_org.getInstance().Init();
            } catch (Exception ex)
            {
                errMsg("Не удалось загрузить курсы валют");
            }

            try
            {
                browserProxies = await browserData.getProxiesAsync();

            } catch
            {
                errMsg("Не удалось загрузить прокси из браузера");
            }

            try
            {
                await Task.Run(async () =>
                {
                    Tags = await browserData.getTags("Без тегов");
                });

                SelectedTags.Clear();

                foreach (var item in t.SelectedTags)
                {
                    if (Tags.Contains(item))
                        SelectedTags.Add(item);
                }
            } catch (Exception ex)
            {
                errMsg("Не удалось загрузить теги из браузера");
            }

            BrowserProxies = await getProxies(browserProxies, t.BrowserProxies, IsAllProxyCheckedBrowser, BrowserProxyCheckedEvent, true);
            MassImportProxies = await getProxies(massImportProxies, t.MassImportProxies, IsAllProxyCheckedMass, MassProxyCheckedEvent, false);
            BrowserProxyCheckedEvent(null);
            MassProxyCheckedEvent(null);

            loadBrowserProxies.Execute();

            AllowProxyChange = true;

        }
        #endregion

        #region callbacks
        private void Init_onContinue(InitParameters p)
        {
            Parameters.Destination = p.Destination;
            Parameters.Litera = p.Litera;
            Parameters.StartNumber = p.StartNumber;
            Parameters.OS = p.OS;
        }
        private void BrowserProxyCheckedEvent(ProxyParameters obj)
        {
            if (BrowserProxies != null)
            {
                IsProxies = BrowserProxies.Any(p => p.IsChecked);
                int cnt = BrowserProxies.Count(p => p.IsChecked);
                if (IsAllProxyCheckedBrowser && cnt < BrowserProxies.Count)
                {
                    needNotifyCheck = false;
                    IsAllProxyCheckedBrowser = false;
                    needNotifyCheck = true;
                }
                if (!IsAllProxyCheckedBrowser && cnt == BrowserProxies.Count)
                {
                    needNotifyCheck = false;
                    IsAllProxyCheckedBrowser = true;
                    needNotifyCheck = true;
                }
            }
            
            
        }
        private void MassProxyCheckedEvent(ProxyParameters obj)
        {
            if (MassImportProxies != null)
            {
                IsProxies = MassImportProxies.Any(p => p.IsChecked);
                int cnt = MassImportProxies.Count(p => p.IsChecked);
                if (IsAllProxyCheckedMass && cnt < MassImportProxies.Count)
                {
                    needNotifyCheck = false;
                    IsAllProxyCheckedMass = false;
                    needNotifyCheck = true;
                }
                if (!IsAllProxyCheckedMass && cnt == MassImportProxies.Count)
                {
                    needNotifyCheck = false;
                    IsAllProxyCheckedMass = true;
                    needNotifyCheck = true;
                }
            }
        }
        private void A_OnAccountChecked(AccountVM obj)
        {
            //var chkd = Accounts?.Where(p => p.IsChecked == true).ToList();
            //IsAccounts = (chkd != null & chkd.Count > 0);
            if (Accounts != null)
            {
                IsAccounts = Accounts.Any(p => p.IsChecked);
                int cnt = Accounts.Count(p => p.IsChecked);

                if (IsAllAccountsChecked && cnt < Accounts.Count)
                {
                    needNotifyCheck = false;
                    IsAllAccountsChecked = false;
                    needNotifyCheck = true;
                }
                if (!IsAllAccountsChecked && cnt == Accounts.Count)
                {
                    needNotifyCheck = false;
                    IsAllAccountsChecked = true;
                    needNotifyCheck = true;
                }
            }

        }
        public async void OnDropEvent(List<string> filenames)
        {
            try
            {
                DirectoryInfo input = Directory.GetParent(filenames[0]);
                string tmp;

                //папка с аккаунтами в виде папок или архивов
                if (filenames.Count == 1 && Directory.Exists(filenames[0]) && !Directory.GetFiles(filenames[0]).Any(o => o.Contains(".txt")))
                {
                    tmp = Path.Combine(input.FullName, $"{new DirectoryInfo(filenames[0]).Name}_parsed");

                    if (Directory.Exists(tmp))
                        Directory.Delete(tmp, true);

                    CopyDirectory(filenames[0], tmp, true);

                    await importAccounts(tmp);
                } else
                {

                    string parsed = $"{input}_parsed";
                    tmp = parsed;
                    //tmp = Path.Combine($"{input}", "ParsedAccounts");
                    if (Directory.Exists(tmp))
                        Directory.Delete(tmp, true);
                    Directory.CreateDirectory(tmp);

                    foreach (var fn in filenames)
                    {
                        if (File.GetAttributes(fn).HasFlag(FileAttributes.Directory))
                        {
                            if (Directory.GetFiles(fn).Any(o => o.Contains(".txt")))
                            {
                                string path = Path.Combine(tmp, new DirectoryInfo(fn).Name);
                                CopyDirectory(fn, path, true);
                            }
                        }

                        if (fn.Contains(".zip") || fn.Contains(".rar"))
                        {
                            string name = Path.GetFileName(fn);
                            File.Copy(fn, Path.Combine(tmp, name));
                        }
                    }
                    if (Directory.GetFiles(tmp).Length > 0 || Directory.GetDirectories(tmp).Length > 0)
                        await importAccounts(tmp);
                }
            } catch (Exception ex)
            {
                errMsg($"Не удалось импортировать аккаунты {ex.Message}");
            }


        }
        public void OnStopRequest() {
            stop();
        }
        #endregion
    }
}
