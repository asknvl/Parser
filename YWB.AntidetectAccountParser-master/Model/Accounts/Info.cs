using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Services.Currency;

namespace YWB.AntidetectAccountParser.Model.Accounts
{
    public class Info
    {
        const string postfix = "__info";

        #region vars
        ICurrencyConverter converter;
        #endregion

        [JsonProperty]
        public string Name { get; set; }

        string birthdate;
        [JsonProperty]
        public string BirthDate {
            get => birthdate;
            set
            {
                birthdate = value;
            }
        }
        string spent;
        [JsonProperty]
        public string Spent { 
            get => spent;
            set
            {
                spent = value;
                SpentUSD = converter.Convert(spent, Currency);
            }
        }
        [JsonProperty]
        public string SpentUSD { get; set; }
        [JsonProperty]
        public string BaseCurrency { get; set; }
        [JsonProperty]
        public string Currency { get; set; }

        string duty;
        [JsonProperty]
        public string Duty { 
            get => duty;
            set
            {
                duty = value;
                DutyUSD = converter.Convert(duty, Currency);
            }
        }
        [JsonProperty]
        public string DutyUSD { get; set; }
        [JsonProperty]
        string limit;
        public string Limit
        {
            get => limit;
            set
            {
                limit = value;
                LimitUSD = converter.Convert(limit, Currency);
            }
        }
        [JsonProperty]
        string LimitUSD { get; set; }
        [JsonProperty]
        public string Prepay { get; set; }
        [JsonProperty]
        public string Status { get; set; }
        [JsonProperty]
        public string Cards { get; set; }
        [JsonProperty]
        public string GEO_RK { get; set; }
        [JsonProperty]
        public string BM { get; set; }
        [JsonProperty]
        public string FP { get; set; }
        [JsonProperty]
        public string GEO_SC { get; set; }       
        
        string useragent;
        [JsonProperty]        
        public string UserAgent
        {
            get => useragent;
            set { 
                useragent = value;
                if (fa != null)
                    fa.UserAgent = value;
            }
        }

        string token;
        [JsonProperty]
        public string Token { 
            get => token;
            set
            {
                token = value;
                if (fa != null)
                    fa.Token = value;
            }
        }

        FacebookAccount fa;
        string path;
        public Info(FacebookAccount fa)
        {
            converter = openexchangerates_org.getInstance();
            BaseCurrency = converter.BaseCurrency;

            this.fa = fa;
            Name = "?";
            BirthDate = "?";           
            
        }

        private void LoadFromInfoString()
        {            
            string s = fa.AccountInfoString;

            try
            {
                string[] splt = s.Split('_');
                Currency = splt[1];
                Spent = splt[0];                
                Duty = splt[2];
                Limit = splt[3];
                Prepay = splt[4];
                Status = splt[5];
                Cards = splt[6];
                GEO_RK = splt[7];
                BM = splt[8];
                FP = splt[9];
                GEO_SC = splt[10];               

            } catch (Exception ex)
            {
                Currency = "?";
                Spent = "?";                
                Duty = "?";
                Limit = "?";
                Prepay = "?"; 
                Status = "?";
                Cards = "?";
                GEO_RK = "?";
                BM = "?";
                FP = "?";
                GEO_SC= "?";
            }
            UserAgent = fa.UserAgent;
            Token = fa.Token;
        }

        public void LoadFromFile()
        {
            //string fileName = Path.Combine(fa.Path, $"{fa.AccountName}{postfix}.json");
            string fileName = Path.Combine(fa.Path, $"{postfix}.json");

            if (!File.Exists(fileName))
            {
                try
                {
                    LoadFromInfoString();
                } catch (Exception ex) {
                    Currency = "?";                    
                    Spent = "?";                    
                    Duty = "?";
                    Limit = "?";
                    Prepay = "?";
                    Status = "?";
                    Cards = "?";
                    GEO_RK = "?";
                    BM = "?";
                    FP = "?";
                    GEO_SC = "?";
                }
                UserAgent = fa.UserAgent;
                Token = fa.Token;

                Save();
            }

            string rd = File.ReadAllText(fileName);
            var p = JsonConvert.DeserializeObject<Info>(rd);

            Name = p.Name;
            BirthDate = p.BirthDate;
            Currency = p.Currency;
            BaseCurrency = p.BaseCurrency;
            Spent = p.Spent;              
            Duty = p.Duty;
            Limit = p.Limit;
            Prepay= p.Prepay;
            Status = p.Status;
            Cards = p.Cards;
            GEO_RK = p.GEO_RK;
            BM = p.BM;
            FP = p.FP; 
            GEO_SC = p.GEO_SC;
            
            UserAgent = p.UserAgent;
            Token = p.Token;
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            try
            {
                string[] jsons = Directory.GetFiles(fa.Path, "*.json");
                foreach (var item in jsons)
                {
                    File.Delete(item);
                }

                //string fileName = Path.Combine(fa.Path, $"{fa.AccountName}{postfix}.json");
                string fileName = Path.Combine(fa.Path, $"{postfix}.json");

                if (File.Exists(fileName))
                    File.Delete(fileName);

                File.WriteAllText(fileName, json);

            } catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить файл JSON");
            }
        }
                
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Имя аккаунта:\t{fa.AccountName}");
            sb.AppendLine($"Имя:\t{Name}");
            sb.AppendLine($"ДР:\t{BirthDate}");
            if (!Currency.Equals("?"))
                sb.AppendLine($"Валюта:\t{Currency}\tUSD");
            if (!Spent.Equals("?"))
                sb.AppendLine($"Спенд:\t{Spent}\t{SpentUSD}");            
            if (!Duty.Equals("?"))
                sb.AppendLine($"Долг:\t{Duty}\t{DutyUSD}");
            if (!Limit.Equals("?"))
                sb.AppendLine($"Лимит:\t{Limit}\t{LimitUSD}");
            if (!Prepay.Equals("?"))
                sb.AppendLine($"Предоплата:\t{Prepay}");
            if (!Status.Equals("?"))
                sb.AppendLine($"Статус:\t{Status}");
            if (!Cards.Equals("?"))
                sb.AppendLine($"Карты:\t{Cards}");
            if (!GEO_RK.Equals("?"))
                sb.AppendLine($"ГЕО РК:\t{GEO_RK}");
            if (!GEO_SC.Equals("?"))
                sb.AppendLine($"ГЕО СЦ:\t{GEO_SC}");
            if (!BM.Equals("?"))
                sb.AppendLine($"БМ:\t{BM}");
            if (!FP.Equals("?"))
                sb.AppendLine($"ФП:\t{FP}");
            sb.AppendLine($"");
            sb.AppendLine($"{fa.DisplayInfoString}");
            sb.AppendLine($"");
            sb.AppendLine($"{UserAgent}");
            sb.AppendLine($"");
            sb.AppendLine($"{Token}");
            return sb.ToString();
        }
    }
}
