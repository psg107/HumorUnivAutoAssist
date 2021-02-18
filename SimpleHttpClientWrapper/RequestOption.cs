using System.Collections.Generic;

namespace SimpleHttpClientWrapper
{
    public enum RequestType
    {
        Default,
        Json,
        QueryString
    }

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
            RequestType = RequestType.Default;
            RequestHeaders = new Dictionary<string, string>();
            ResponseType = ResponseType.Default;
            ResponseEncoding = "euc-kr";
        }

        /// <summary>
        /// 요청 데이터 종류
        /// </summary>
        public RequestType RequestType { get; set; }

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
