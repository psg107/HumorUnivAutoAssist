using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Helpers
{
    public enum ResponseType
    {
        Default,
        Json,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HttpClientWrapper"/>
    public class RequestOption
    {
        public RequestOption()
        {
            RequestHeaders = new Dictionary<string, string>();
            ResponseType = ResponseType.Default;
            ResponseEncoding = "euc-kr";
        }

        /// <summary>
        /// 요청 헤더
        /// </summary>
        public Dictionary<string, string> RequestHeaders { get; set; }

        /// <summary>
        /// 응답 데이터 종류
        /// </summary>
        public ResponseType ResponseType { get; set; }

        /// <summary>
        /// 응답 데이터 인코딩
        /// </summary>
        public string ResponseEncoding { get; set; }
    }
}
