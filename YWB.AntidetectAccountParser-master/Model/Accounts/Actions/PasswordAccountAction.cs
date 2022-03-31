using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YWB.AntidetectAccountParser.Helpers;

namespace YWB.AntidetectAccountParser.Model.Accounts.Actions {


    class PasswordItem {        
        public string login { get; set; }
        public string password { get; set; }
        public int entrances { get; set; }
        public List<string> logins { get; set; }
    }

    public class PasswordAccountAction<T> : AccountAction<T> where T : SocialAccount {

        public PasswordAccountAction() {
            Condition = (fileName) => fileName.Contains("password");
            Action = (stream, sa) => ExtractLoginAndPassword(stream, sa);
            Message = "Found file with passwords: ";
        }

        private void ExtractLoginAndPassword(System.IO.Stream s, T sa) {
            var needle = sa is FacebookAccount ? "facebook" : "google.com";

            var lines = Encoding.UTF8.GetString(s.ReadAllBytes()).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int index = -1;
            while ((index = lines.FindIndex(index + 1, l => l.ToLowerInvariant().Contains(needle))) != -1) {
                if (index + 2 >= lines.Count) continue;
                var split = lines[index + 1].Split(' ');
                if (split.Length != 2) continue;
                var login = split[1];
                split = lines[index + 2].Split(' ');
                if (split.Length != 2) continue;
                var password = split[1];
                if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)) {
                    if (sa.AddLoginPassword(login, password)) { }

                    var found = sa.LoginsPasswords.FirstOrDefault(o => o.Password.Equals(password));
                    if (found == null)
                        sa.LoginsPasswords.Add(new LoginPassword(login, password) { Note = "FB" });
                    else
                        if (!found.Logins.Contains(login))
                            found.Logins.Add(login);                    

                    index += 2;
                }
            }

            string keyp = "Password: ";
            string keyl = "Username: ";

            //List<PasswordItem> plist = new List<PasswordItem>();

            //foreach (var l in lines)
            //{
            //    string p = "";
            //    if (l.Contains(keyp))
            //    {
            //        p = l.Replace(keyp, "");
            //        if (p.Equals(""))
            //            continue;
            //        var item = plist.FirstOrDefault(o => o.password.Equals(p));
            //        if (item == null)
            //        {
            //            plist.Add(new PasswordItem() { password = p, entrances = 1 });
            //        } else
            //            item.entrances++;
            //    }
            //}
            //List<PasswordItem> sorted = plist.OrderByDescending(o => o.entrances).ToList();

            //foreach (var item in sorted)
            //    sa.AddPassword(item.password);


            List<LoginPassword> tmp = new List<LoginPassword>();

            for (int i = 0; i < lines.Count-1; i++ )
            {
                if (lines[i].Contains(keyl) && lines[i+1].Contains(keyp))
                {
                    string l = lines[i].Replace(keyl, "");
                    string p = lines[i + 1].Replace(keyp, "");

                    if (!string.IsNullOrEmpty(l) && !string.IsNullOrEmpty(p))
                    {
                        var found = sa.LoginsPasswords.FirstOrDefault(o => o.Password.Equals(p));
                        if (found != null)
                            continue;                        
                        found = tmp.FirstOrDefault(o => o.Password.Equals(p));
                        if (found == null)
                            tmp.Add(new LoginPassword(l, p) { Note = "" + 1 });
                        else
                        {
                            if (!found.Logins.Contains(l))
                                found.Logins.Add(l);
                            found.Note = "" + found.Logins.Count; 
                        }
                            
                    }                 
                }                
            }

            var sorted = tmp.OrderByDescending(o => o.Logins.Count);
            sa.LoginsPasswords = sa.LoginsPasswords.OrderByDescending(o => o.Logins.Count).Concat(sorted).ToList();            

        }
    }
}
