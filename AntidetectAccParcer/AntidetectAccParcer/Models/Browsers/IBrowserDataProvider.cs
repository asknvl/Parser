using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;

namespace AntidetectAccParcer.Models.Browsers {
    public interface IBrowserDataProvider {
        Task<List<Proxy>> getProxiesAsync();
        Task<List<string>> getTags(string firsttag);
    }
}
