using SimpleHttpClientWrapper;

namespace HumorUnivAutoAssist.Models
{
    /// <summary>
    /// 로그인 데이터
    /// </summary>
    public class Login
    {
        [KeyName(Name = "url")]
        public string Url { get; set; }

        [KeyName(Name = "url")]
        public string Id { get; set; }

        [KeyName(Name = "url")]
        public string Pw { get; set; }
    }
}
