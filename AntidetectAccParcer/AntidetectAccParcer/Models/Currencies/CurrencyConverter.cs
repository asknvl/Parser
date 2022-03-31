using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Currencies
{
    public abstract class CurrencyConverter : ICurrencyConverter
    {

        #region vars
        protected Dictionary<string, double> allToUsdRate = new Dictionary<string, double>();
        #endregion

        public abstract bool Init();

        public double Convert(double value, string currencyCode)
        {          

            if (allToUsdRate.ContainsKey(currencyCode.ToUpperInvariant()))
                return value / allToUsdRate[currencyCode];
            else
                throw new Exception("Не удалось определить курс");

        }
    }
}
