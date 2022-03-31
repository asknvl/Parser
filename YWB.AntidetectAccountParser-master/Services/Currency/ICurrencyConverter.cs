using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YWB.AntidetectAccountParser.Services.Currency
{
    internal interface ICurrencyConverter
    {
        string BaseCurrency { get; set; }
        bool Init();
        string Convert(string svalue, string currencyCode);
    }
}
