using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Currencies
{
    internal interface ICurrencyConverter
    {
        bool Init();
        double Convert(double value, string currencyCode);
    }
}
