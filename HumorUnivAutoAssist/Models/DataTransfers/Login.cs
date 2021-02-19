using SimpleHttpClientWrapper;

namespace HumorUnivAutoAssist.Models.DataTransfers
{
    /// <summary>
    /// 로그인 전송 데이터
    /// </summary>
    public class Login
    {
        [KeyName(Name = "url")]
        public string Url { get; set; }

        [KeyName(Name = "id")]
        public string Id { get; set; }

        [KeyName(Name = "pw")]
        public string Pw { get; set; }
    }
}
