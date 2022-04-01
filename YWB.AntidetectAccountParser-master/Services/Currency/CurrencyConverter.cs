using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YWB.AntidetectAccountParser.Services.Currency
{
    public abstract class CurrencyConverter : ICurrencyConverter
    {

        #region vars
        protected Dictionary<string, double> allToUsdRate = new Dictionary<string, double>();
        #endregion

        #region properties
        public string BaseCurrency { get; set; }
        #endregion

        public abstract bool Init();

        public string Convert(string svalue, string currencyCode)
        {
            string res = "?";
            try
            {
                double dvalue = double.Parse(svalue.Replace(",", "."), NumberFormatInfo.InvariantInfo);
                double rate = allToUsdRate[currencyCode];
                double dres = dvalue / rate;
                res = string.Format("{0:0.00}", dres);

            } catch
            {               
            }
            return res;            
        }
    }
}
