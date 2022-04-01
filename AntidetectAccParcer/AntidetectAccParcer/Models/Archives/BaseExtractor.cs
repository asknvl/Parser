using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public abstract class BaseExtractor : IExtractor, IProgress
    {

        #region vars
        string spent;
        string currency;
        string duty;
        string limit;
        string prepay;
        string status;
        string cards;
        string geo_rk;
        string bm;
        string fp;
        string geo_sc;
        #endregion

        #region helpers
        void initValues()
        {
            spent = "?";
            currency = "?";
            duty = "?";
            limit = "?";
            prepay = "?";
            status = "?";
            cards = "?";
            geo_rk = "?";
            bm = "?";
            fp = "?";
            geo_sc = "?";
        }

        string combineDescription()
        {
            return $"{spent}_" +
                   $"{currency}_" +
                   $"{duty}_" +
                   $"{limit}_" +
                   $"{prepay}_" +
                   $"{status}_" +
                   $"{cards}_" +
                   $"{geo_rk}_" +
                   $"{bm}_" +
                   $"{fp}_" +
                   $"{geo_sc}";
        }
        #endregion

        #region protected
        public void OnProgressEvent(int progress, int total)
        {
            ProgressEvent?.Invoke(progress, total);
        }

        protected string getDescription(string input)
        {
            string res = "Неверный формат";

            //check simple
            try
            {

                var name = input.Replace(")", "")
                       .Replace("(", "")
                       .Replace("/", "")
                       .Replace(@"\", "");
                var splt = name.Split("_");

                double.Parse(splt[0]);
                double.Parse(splt[2]);
                double.Parse(splt[3]);
                bool.Parse(splt[4]);

                initValues();

                spent = splt[0];
                currency = splt[1];
                duty = splt[2];
                limit = splt[3];
                prepay = splt[4].ToLower().Equals("true") ? "Да" : "Нет";
                status = splt[5].ToLower().Equals("active") ? "Активен" : "Неактивен";
                string c = $"{splt[6]}, {splt[7]}";
                cards = c;
                geo_rk = splt[8].ToUpper();
                //bm = splt[splt.Length - 4].ToLower();
                //bm = bm.Contains("bm") ? bm.Replace("bm", "") : "?";
                //fp = splt[splt.Length - 3].ToLower();
                //fp = fp.Contains("fp") ? fp.Replace("fp", "") : "?";
                
                var sbm = splt.FirstOrDefault(o => o.ToLower().Contains("bm"));                
                bm = (sbm != null) ? sbm.ToLower().Replace("bm", "") : "Нет";

                var sfp = splt.FirstOrDefault(o => o.ToLower().Contains("fp"));
                fp = (sfp != null) ? sfp.ToLower().Replace("fp", "") : "Нет";

                geo_sc = splt[splt.Length - 2].ToUpper();
                return combineDescription();

            } catch
            {
                initValues();
            }

            //paranoid
            try
            {
                if (input.ToLower().Contains("paranoid"))
                {

                    var splt = input.Split('_');
                    status = splt[2].ToLower().Contains("act") ? "Активен" : "Неактивен";
                    var bms = splt[3].Split('(', ')');
                    bm = bms[1].ToLower().Equals("false") ? "Нет" : bms[1];
                    geo_rk = splt[4];
                    currency = splt[5];
                    limit = splt[6].Split('(', ')')[1];
                    spent = splt[7].Split('(', ')')[1];
                    duty = splt[8].Split('(', ')')[1];
                    fp = splt[9].Split('(', ')')[1];
                    return combineDescription();
                }
            } catch {
                initValues();
            }

            return combineDescription();
        }
        protected bool checkDescription(string descrption)
        {
            bool res = false;

            try
            {
                string[] split = descrption.Split("_");
                double.Parse(split[0]);
                double.Parse(split[2]);
                double.Parse(split[3]);

                bool f = descrption.ToLower().Contains("false");
                bool t = descrption.ToLower().Contains("true");

                res = f | t;

            } catch (Exception ex)
            {
            }

            return res;
        }
        #endregion

        #region public
        public abstract void Extract(string source, string destination, string litera, ref int startnumber);
        #endregion

        #region callbacks
        public event Action<int, int> ProgressEvent;
        #endregion
    }
}
