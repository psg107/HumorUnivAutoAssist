using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpClientWrapper
{
    /// <summary>
    /// 쿼리스트링 키 이름
    /// </summary>
    /// <seealso cref="HumorUnivAutoAssist.QueryStringFormatter.WriteToStreamAsync"/>
    public class KeyNameAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
