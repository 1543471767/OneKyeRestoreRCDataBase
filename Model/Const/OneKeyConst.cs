using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Const
{
    public static class OneKeyConst
    {
        public static readonly string FtpUserNameFor15 = System.Configuration.ConfigurationManager.AppSettings["FtpUserNameFor15"] ?? "";
        public static readonly string FtpPassWordFor15 = System.Configuration.ConfigurationManager.AppSettings["FtpPassWordFor15"] ?? "";
        public const string LocalBakFilePath = "d:\\rcbak\\BakFile";
        public const string LocalDiffFilePath = "d:\\rcbak\\DiffFile";
        public const string LocalTrnFilePath = "d:\\rcbak\\TrnFile";
        public const string FtpFilePathFor15 = "d:\\rcbak";
        public const string SplitStr = "--============================================================";


        public static readonly string FtpIPFor149 = System.Configuration.ConfigurationManager.AppSettings["FtpIPFor149"] ?? "";
        public static readonly string FtpUserNameFor149 = System.Configuration.ConfigurationManager.AppSettings["FtpUserNameFor149"] ?? "";
        public static readonly string FtpPassWordFor149 = System.Configuration.ConfigurationManager.AppSettings["FtpPassWordFor149"] ?? "";

        public static readonly string SqlTemplatePath = AppDomain.CurrentDomain.BaseDirectory + "\\Template\\SqlTemplate.txt";
    }
}
