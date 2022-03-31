using AntidetectAccParcer.Models.Storage;
using Avalonia.Data;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.ViewModels
{
    public class initialVM : ViewModelBase, ILifeCicle
    {

        #region vars
        IStorage<initialVM> storage;
        #endregion

        #region properties
        string litera;
        [JsonProperty]
        public string Litera
        {
            get => litera;
            set
            {
                if (value == "")
                    throw new DataValidationException("");
                this.RaiseAndSetIfChanged(ref litera, value);
            }
        }

        string startNUmber;
        [JsonProperty]
        public string StartNumber
        {
            get => startNUmber;
            set
            {
                if (value == "")
                    throw new DataValidationException("");
                this.RaiseAndSetIfChanged(ref startNUmber, value);
            }
        }

        bool octo;
        [JsonProperty]
        public bool IsOcto
        {
            get => octo;
            set => this.RaiseAndSetIfChanged(ref octo, value);
        }

        bool dolphin;
        [JsonProperty]
        public bool IsDolphin
        {
            get => dolphin;
            set => this.RaiseAndSetIfChanged(ref dolphin, value);
        }

        bool fbtool;
        [JsonProperty]
        public bool IsFBTool
        {
            get => fbtool;
            set => this.RaiseAndSetIfChanged(ref fbtool, value);
        }

        bool archive;
        [JsonProperty]
        public bool IsArchive
        {
            get => archive;
            set => this.RaiseAndSetIfChanged(ref archive, value);
        }

        bool text;
        [JsonProperty]
        public bool IsText
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        bool win;
        [JsonProperty]
        public bool IsWindows
        {
            get => win;
            set => this.RaiseAndSetIfChanged(ref win, value);
        }

        bool mac;
        [JsonProperty]
        public bool IsMac
        {
            get => mac;
            set => this.RaiseAndSetIfChanged(ref mac, value);
        }
        #endregion

        #region commands
        public ReactiveCommand<Unit, Unit> continueCmd { get; }
        public ReactiveCommand<Unit, Unit> cancelCmd { get; }
        #endregion
        public initialVM()
        {

            storage = new Storage<initialVM>("init.json", this);

            Litera = "Y";
            StartNumber = "1";
            IsOcto = true;
            IsArchive = true;
            IsWindows = true;

            #region commands
            continueCmd = ReactiveCommand.Create(() =>
            {
                InitParameters p = packParemeters();
                onContinue?.Invoke(p);
                OnCloseRequest();
            });

            cancelCmd = ReactiveCommand.Create(() => { 
                OnCloseRequest();
            });
            #endregion
        }

        #region helpers
        InitParameters packParemeters()
        {
            InitParameters p = new InitParameters();

            if (IsOcto)
                p.Destination = ImportDestination.octo;
            else
                if (IsDolphin)
                p.Destination = ImportDestination.dolphin;
            else
                if (IsFBTool)
                p.Destination = ImportDestination.fbtool;

            if (IsArchive)
                p.Type = FileType.archive;
            else
                if (IsText)
                p.Type = FileType.text;

            if (IsWindows)
                p.OS = Platform.win;
            else
                if (IsMac)
                p.OS = Platform.mac;

            p.Litera = Litera;
            p.StartNumber = int.Parse(StartNumber);

            return p;
        }
        #endregion

        #region public
        public void Update()
        {
            InitParameters p = packParemeters();
            onContinue?.Invoke(p);
        }
        #endregion

        #region callbacks
        public event Action<InitParameters> onContinue;

        public void onClose()
        {
            storage.save(this);
        }

        public void onStart()
        {
            var t = storage.load();
            Litera = t.Litera;
            StartNumber = t.StartNumber;

            IsOcto = t.IsOcto;
            IsDolphin = t.IsDolphin;
            IsFBTool = t.IsFBTool;

            IsArchive = t.IsArchive;
            IsText = t.IsText;

            IsWindows = t.IsWindows;
            IsMac = t.IsMac;
        }
        #endregion
    }

    public enum ImportDestination
    {
        octo,
        dolphin,
        fbtool
    }
    public enum FileType
    {
        archive,
        text
    }
    public enum Platform
    {
        win,
        mac
    }

    public class InitParameters
    {
        public ImportDestination Destination { get; set; }
        public FileType Type { get; set; }
        public Platform OS { get; set; }
        public string Litera { get; set; }
        public int StartNumber { get; set; }
    }
}
