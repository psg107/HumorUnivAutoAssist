using System;

namespace HumorUnivAutoAssist.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HumorUnivAutoAssist.HURecommendService"/>
    public class HURecommendServiceOption
    {
        public HURecommendServiceOption()
        {
            this.WaitTime = 100;
#warning 비추천 가중치 '10' 정확하지 않음. 확인 필요, 5보다 높다는건 확인함 (50/2)
            this.UpWeight = 1;
            this.DownWeight = 10;
        }

        /// <summary>
        /// 로그인 이후 '쿠키 갱신 에러' 방지를 위한 대기 시간 (초단위)
        /// </summary>
        public int WaitTime { get; set; }

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
    }
}
