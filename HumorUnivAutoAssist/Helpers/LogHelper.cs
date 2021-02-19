using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Helpers
{
    public static class LogHelper
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}] {message}");
        }
    }
}
