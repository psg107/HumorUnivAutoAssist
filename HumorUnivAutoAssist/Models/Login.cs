using System.ComponentModel.DataAnnotations;

namespace HumorUnivAutoAssist.Models
{
    /// <summary>
    /// 로그인 데이터
    /// </summary>
    public class Login
    {
        [Display(Name = "url")]
        public string Url { get; set; }

        [Display(Name = "id")]
        public string Id { get; set; }

        [Display(Name = "pw")]
        public string Pw { get; set; }
    }
}
