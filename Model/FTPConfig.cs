using Model.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FTPConfig
    {
        public string DataBaseFtpIP { get; set; }
        public string FtpUserNameFor15 { get; set; } = OneKeyConst.FtpUserNameFor15;
        public string FtpPassWordFor15 { get; set; } = OneKeyConst.FtpPassWordFor15;


        public string RCFtpIP { get; set; }
        public string RCFtpUserName { get; set; }
        public string RCFtpPassWord { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
