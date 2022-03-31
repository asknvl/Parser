using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model;


namespace AntidetectAccParcer.Models.Proxies
{
    public class TextProxyParcer : IProxyParcer
    {
        public List<Proxy> Parse(string text, ProxyProtocol protocol, string prefix)
        {
            List<Proxy> proxies = new List<Proxy>();

            if (text == null || text.Equals(""))
                throw new Exception("Не задан ни один адрес");

            string[] lines = text.Split(Environment.NewLine);

            int cntr = 0;
            
            foreach (var item in lines)
            {
                string[] line = item.Split(':');

                try
                {
                    Proxy proxy = new Proxy();

                    switch (line.Length)
                    {
                        case 2:
                            proxy.Address = line[0];
                            proxy.Port = line[1];
                            break;
                        case 3:
                            string pass = line[1].Split("@")[0];
                            string host = line[1].Split("@")[1];
                            proxy.Address = host;
                            proxy.Port = line[2];
                            proxy.Login = line[0];
                            proxy.Password = pass;
                            break;
                        case 4:
                            proxy.Address = line[0];
                            proxy.Port = line[1];
                            proxy.Login = line[2];
                            proxy.Password = line[3];
                            break;
                        default:
                            break;
                    }

                    proxy.Title = (prefix.Equals("")) ? proxy.Address : $"{prefix} {++ cntr}";
                    proxy.Type = protocol.ToString();

                    try
                    {
                        IPAddress.Parse(proxy.Address);
                    } catch
                    {
                        throw new Exception(proxy.Address);
                    }
                        
                    proxies.Add(proxy);

                } catch (Exception ex)
                {
                    throw new Exception($"Строка {item} имеет неправильный формат");
                }
            }

            return proxies;
        }
    }
}
