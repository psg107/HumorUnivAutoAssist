using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
#warning 비추천 가중치 '6' 정확하지 않음. 확인 필요, 5보다 높다는건 확인함 (50/2)
            this.UpWeight = 1;
            this.DownWeight = 6;
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
    }
}
