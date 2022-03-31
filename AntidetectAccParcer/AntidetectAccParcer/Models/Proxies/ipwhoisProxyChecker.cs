using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;

namespace AntidetectAccParcer.Models.Proxies
{
    public class ipwhoisProxyChecker : IProxyChecker
    {
        public async Task<JObject> Check(string address)
        {
            var rc = new RestClient("http://ipwhois.app/json/");
            var r = new RestRequest(address, Method.GET);
            var resp = await rc.ExecuteAsync(r);            
            var res = JsonConvert.DeserializeObject<JObject>(resp.Content);            
            return res;
        }
    }
}
