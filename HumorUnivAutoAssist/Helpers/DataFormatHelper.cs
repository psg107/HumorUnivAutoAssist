using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Helpers
{
    public static class DataFormatHelper
    {
        /// <summary>
        /// string -> int
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int ToInt(this string number)
        {
            if (int.TryParse(number, out int output))
            {
                return output;
            }
            return 0;
        }
    }
}
