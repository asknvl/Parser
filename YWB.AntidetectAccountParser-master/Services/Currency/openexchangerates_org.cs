using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YWB.AntidetectAccountParser.Services.Currency
{
    public class openexchangerates_org : CurrencyConverter
    {

        static openexchangerates_org instance;

        private openexchangerates_org()
        {
            BaseCurrency = "USD";
        }

        public static openexchangerates_org getInstance()
        {
            if (instance == null)
            {                
                instance = new openexchangerates_org();
            }
            return instance;
        }

        public override bool Init()
        {           
            string url = $"https://openexchangerates.org/api/";
            var rc = new RestClient(url);
            var r = new RestRequest($"latest.json?app_id=e55b8d2d17644044ba904305abe37033&{BaseCurrency}", Method.GET);

            Task.Run(async () =>
            {
                var resp = await rc.ExecuteAsync(r, new System.Threading.CancellationToken());
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject jObject = JObject.Parse(resp.Content);
                    JToken list = jObject["rates"];
                    foreach (JProperty item in list)
                    {
                        allToUsdRate.Add(item.Name, double.Parse(item.Value.ToString()));
                    }
                }                
            });

            return allToUsdRate.Count > 0;
        }
    }
}
