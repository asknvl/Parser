using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntidetectAccParcer.Models.Archives
{
    public enum ArhiveType
    {
        rar,
        zip,
        rar_zip,
        combo
    }
    public class ExtractorFactory
    {
        public IExtractor GetExtractor(int archiveType)
        {
            switch (archiveType)
            {
                case 0:
                    return new ZipExtractor();
                case 1:
                    return new RarExtractor();
                case 2:
                    return new ZipExtractor();
                default:
                    return new ZipExtractor();
            }

        }

        public IExtractor GetExtractor(ArhiveType type)
        {
            switch (type)
            {
                case ArhiveType.rar:
                    return new RarExtractor();
                case ArhiveType.zip:
                    return new ZipExtractor();
                case ArhiveType.rar_zip:
                    return new ComboExtractor();
                default:
                    return new ZipExtractor();
            }
        }
    }
}
