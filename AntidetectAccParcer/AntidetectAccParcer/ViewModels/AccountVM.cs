using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YWB.AntidetectAccountParser.Model.Accounts;

namespace AntidetectAccParcer.ViewModels
{
    public class AccountVM : ViewModelBase
    {
        bool ischecked;
        public bool IsChecked
        {
            get => ischecked;
            set {                                 
                this.RaiseAndSetIfChanged(ref ischecked, value);
                OnAccountChecked?.Invoke(this);
            }
        }

        string tmpAccountName;
        string accountName;
        public string AccountName
        {
            get => accountName;
            set
            {              
                this.RaiseAndSetIfChanged(ref accountName, value);               
            }
        }

        public FacebookAccount Account { get; set; }

        public AccountVM(FacebookAccount account)
        {
            Account = account;
            AccountName = account.AccountName;
        }

        #region private
        void replace(string oldpath, string newpath)
        {
            Directory.Move(oldpath, newpath);
        }
        #endregion

        #region public
        public void Restore()
        {

            if (AccountName.Equals(""))
            {
                AccountName = Account.AccountName;
                throw new Exception("Имя аккаунта не может быть пустым");
            }

            string curpath = Account.Path;

            if (!Account.AccountName.Equals(AccountName))
            {                
                DirectoryInfo dp = Directory.GetParent(curpath);
                string dirpath = dp.FullName;
                string newpath = Path.Combine(dirpath, AccountName);
                Debug.WriteLine($"{dirpath}");
                Debug.WriteLine($"{curpath}");
                Debug.WriteLine($"{newpath}");

                string[] accpaths = Directory.GetDirectories(dirpath);
                var search = accpaths.FirstOrDefault(p => p.Equals(newpath));
                if (search != null)
                {
                    string tmp = AccountName;
                    AccountName = Account.AccountName;
                    throw new Exception($"Имя {tmp} уже используется");
                }
                
                Account.AccountName = AccountName;
                Account.Path = newpath;

                replace(curpath, Account.Path);
            } 
        }
        #endregion

        #region callbacks
        public event Action<AccountVM> OnAccountChecked;        
        #endregion
    }
}
