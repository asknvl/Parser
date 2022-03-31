using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public class ComboExtractor : BaseExtractor
    {
        #region vars
        #endregion

        #region const
        string[] extensions = new string[] { "rar", "zip" };
        #endregion

        public override void Extract(string source, string destination, string litera, ref int startnumber)
        {


            try
            {           
                DirExtractor dir = new DirExtractor();
                dir.ProgressEvent += (p, t) =>
                {
                    OnProgressEvent(p, t);
                };
                dir.Extract(source, source, litera, ref startnumber);

            } catch (Exception ex)
            {
            }

            try
            {
                //zips = Directory.GetFiles(source, ".zip").ToList();
                ZipExtractor zip = new ZipExtractor();
                zip.ProgressEvent += (p, t) => {
                    OnProgressEvent(p, t);
                };
                zip.Extract(source, source, litera, ref startnumber);
            } catch (Exception ex)
            {
            }

            try
            {
                RarExtractor rar = new RarExtractor();
                rar.ProgressEvent += (p, t) => {
                    OnProgressEvent(p, t);
                };
                rar.Extract(source, source, litera, ref startnumber);
            } catch (Exception ex)
            {

            }

            //int cntr = 0;
            //foreach (var item in files)           
            //{
            //    cntr++;
            //    OnProgressEvent(cntr);
            //    Thread.Sleep(100);

            //}
        }

        #region callbacks        
        #endregion
    }
}
