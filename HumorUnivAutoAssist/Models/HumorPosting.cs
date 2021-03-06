﻿using HumorUnivAutoAssist.Models.DataTransfers;

namespace HumorUnivAutoAssist.Models
{
    /// <summary>
    /// 게시글
    /// </summary>
    public class HumorPosting
    {
        /// <summary>
        /// /Key
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        /// 어시스트 정보
        /// </summary>
        public Assist AssistInfo { get; set; }
    }
}
