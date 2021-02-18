﻿using HumorUnivAutoAssist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Services
{
    public interface IHURecommendService
    {
        /// <summary>
        /// 게시글 포스팅 가져오기
        /// </summary>
        /// <param name="minScore">가져올 게시글의 최소 점수</param>
        /// <returns></returns>
        Task<List<HumorPosting>> GetPostings(int minScore);

        /// <summary>
        /// 게시글 어시스트
        /// </summary>
        /// <param name="posting"></param>
        /// <returns></returns>
        Task TryAssist(HumorPosting posting);
    }
}
