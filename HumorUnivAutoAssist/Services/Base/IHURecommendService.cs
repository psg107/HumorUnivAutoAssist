using HumorUnivAutoAssist.Models;
using HumorUnivAutoAssist.Models.DataTransfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Services
{
    public interface IHURecommendService
    {
        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> Login(string id, string password);

        /// <summary>
        /// 게시글 정보 가져오기
        /// </summary>
        /// <param name="checkScore">리스트에서 확인한 최소 점수</param>
        /// <param name="minScore">실제 게시글에서 가져온 최소 점수</param>
        /// <returns></returns>
        Task<List<HumorPosting>> GetPostings(int checkScore, int minScore);

        /// <summary>
        /// 게시글 어시스트
        /// </summary>
        /// <param name="posting"></param>
        /// <returns></returns>
        Task<bool> TryAssist(HumorPosting humorPosting);
    }
}
