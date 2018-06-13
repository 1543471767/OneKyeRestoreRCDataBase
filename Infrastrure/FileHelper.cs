using Model.Const;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastrure
{
    public static class FileHelper
    {
        public static void CreateDicIfNoExist(string dicPath)
        {
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
        }
    }
}
