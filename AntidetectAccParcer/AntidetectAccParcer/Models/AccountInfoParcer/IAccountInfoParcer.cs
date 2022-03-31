using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.AccountInfoParcer {
    public interface IAccountInfoParcer {
        AccountInfo Parse(string accName, string info);
    }

    public class AccountInfo {

    }
}
