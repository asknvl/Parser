using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;

namespace AntidetectAccParcer.Models.Proxies
{

    public enum ProxyProtocol
    {
        socks5 = 0,
        http,
        ssh
    }
    public interface IProxyParcer
    {
        List<Proxy> Parse(string text, ProxyProtocol protocol, string prefix);
    }

    public class IProxyParcerException { }
}
