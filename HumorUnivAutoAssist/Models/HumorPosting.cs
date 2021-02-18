using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Models
{
    /// <summary>
    /// 게시글
    /// </summary>
    public class HumorPosting
    {
        /// <summary>
        /// 제목
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 추천수
        /// </summary>
        public int UpScore { get; set; }

        /// <summary>
        /// 비추천수
        /// </summary>
        public int DownScore { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string Url { get; set; }
    }
}
