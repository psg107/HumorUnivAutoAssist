using SimpleHttpClientWrapper;

namespace HumorUnivAutoAssist.Models.DataTransfers
{
    /// <summary>
    /// 추천 전송 데이터
    /// </summary>
    public class Assist
    {
        [KeyName(Name = "number")]
        public int Number { get; set; }

        [KeyName(Name = "hash")]
        public string Hash { get; set; }

        [KeyName(Name = "hash_check")]
        public string HashCheck { get; set; }
    }
}
