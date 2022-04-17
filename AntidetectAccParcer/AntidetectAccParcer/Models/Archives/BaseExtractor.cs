using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        string billing;
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
            billing = "?";            
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
                   $"{billing}_" +
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
                limit = splt[3].ToLower().Equals("-1") ? "UNLIM" : splt[3];
                prepay = splt[4].ToLower().Equals("true") ? "Да" : "Нет";
                status = splt[5].ToLower().Equals("active") ? "Активен" : "Неактивен";
                //string c = $"{splt[6]}, {splt[7]}";
                billing = splt[6].ToLower().Equals("x") ? "Нет" : splt[6];
                cards = splt[7].ToLower().Equals("x") ? "Нет" : splt[7];
                
                geo_rk = splt[8].ToUpper();
                
                var sbm = splt.FirstOrDefault(o => o.ToLower().Contains("bm"));
                int bmIndex = 0;
                if (sbm != null)
                {
                    bm = sbm.ToLower().Replace("bm", "");                    
                    int ibm;
                    if (int.TryParse(bm, out ibm))
                    {
                        bmIndex = splt.ToList().IndexOf(sbm);
                    } else bm = "Нет";
                } else
                    bm = "Heт";

                var sfp = splt.FirstOrDefault(o => o.ToLower().Contains("fp"));
                int fpIndex = 0;
                if (sfp != null)
                {
                    fp = sfp.ToLower().Replace("fp", "");
                    int ifp;
                    if (int.TryParse(fp, out ifp))
                    {
                        fpIndex = splt.ToList().IndexOf(sfp);
                    } else fp = "Нет";
                } else
                    fp = "Heт";

                int index = Math.Max(bmIndex, fpIndex);

                geo_sc = (index > 0) ? splt[index + 1].ToUpper() : splt[9];
                if (geo_sc.Length >= 2)
                    geo_sc = geo_sc.Substring(0, 2);
                else
                    geo_sc = "?";
                

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
        public abstract void Extract(string source, string destination, string litera, ref int startnumber, CancellationTokenSource cts);
        #endregion

        #region callbacks
        public event Action<int, int> ProgressEvent;
        #endregion
    }
}
