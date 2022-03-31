using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;

namespace AntidetectAccParcer.Models.Proxies
{
    public interface IProxyChecker
    {
        Task<JObject> Check(string address);
    }
}
