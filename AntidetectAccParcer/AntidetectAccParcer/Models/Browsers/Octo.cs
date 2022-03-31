using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;
using YWB.AntidetectAccountParser.Model.Accounts;
using YWB.AntidetectAccountParser.Services.Browsers;

namespace AntidetectAccParcer.Models.Browsers {
    public class Octo : OctoApiService, IBrowserDataProvider {

        public async Task<List<Proxy>> getProxiesAsync() {

            var r = new RestRequest("proxies", Method.GET);
            var json = await ExecuteRequestAsync<JObject>(r);

            return json["data"].Select((dynamic g) => new Proxy() {                
                IsOctoProxy = true,
                Address = g.host,
                Port = g.port,
                Login = g.login,
                Title = g.title,
                Type = g.type,
                Password = g.password,
                Uuid = g.uuid
                
            }).ToList();

        }

        public async Task<List<string>> getTags(string firsttag) {

            List<string> res = new List<string>();
            res.Add(firsttag);

            try {

                await Task.Run(async () => {

                    List<AccountGroup> g = await GetExistingTagsAsync();
                    g.ForEach(i => res.Add(i.Name));
                
                });

            } catch (Exception ex) {
                throw ex;
            }

            return res;
        }

        

    }
}
