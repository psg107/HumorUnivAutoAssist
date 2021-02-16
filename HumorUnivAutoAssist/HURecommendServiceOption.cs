using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HumorUnivAutoAssist.HURecommendService"/>
    public class HURecommendServiceOption
    {
        public HURecommendServiceOption()
        {
#warning 비추천 가중치 '3' 정확하지 않음. 확인 필요

            this.UpWeight = 1;
            this.DownWeight = 3;
            this.MinimumScore = 39;
        }

        /// <summary>
        /// 추천 가중치
        /// </summary>
        public int UpWeight;

        /// <summary>
        /// 비추천 가중치
        /// </summary>
        public int DownWeight 
        {
            get => downWeight;
            set => downWeight = Math.Abs(value);
        }
        private int downWeight;

        /// <summary>
        /// 가져올 게시글의 최소 점수
        /// </summary>
        public int MinimumScore { get; set; }
    }
}
