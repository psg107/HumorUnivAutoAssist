using HumorUnivAutoAssist.Helpers;
using HumorUnivAutoAssist.Services;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;

                LogHelper.Log(ex.Message);
                LogHelper.Log(ex.StackTrace);
                //ex.Message
                //ex.StackTrace
                Debugger.Break();
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Task.Run(async () =>
            {
                var huService = new HURecommendService();

#warning 로그인 정보 외부에서 주입 하도록 처리 필요
                var loginSuccess = await huService.Login("", "");
                if (loginSuccess == false)
                {
                    Debugger.Break();
                    throw new Exception("로그인 실패!!!");
                }

                LogHelper.Log("로그인 성공!");

                while (true)
                {
                    var minScore = 39;
                    var postings = await huService.GetPostings(minScore);
                    if (postings.Count > 0)
                    {
                        LogHelper.Log($"점수 {minScore} 이상 게시글 {postings.Count}개 발견");
                    }

                    foreach (var posting in postings)
                    {
                        var assist = await huService.TryAssist(posting);
                        LogHelper.Log(assist 
                                        ? $"게시글 '{posting.Title}' 어시스트 시도!!"
                                        : $"게시글 '{posting.Title}' 어시스트 실패..");

                        Debugger.Break();
                    }

#warning 특정 점수 구간에서는 대기 시간을 더 짧게 설정할 수 있도록 처리 필요 (HURecommendServiceOption)
                    LogHelper.Log($"대기중..");
                    await Task.Delay(5000);
                }
            }).Wait();

            Console.ReadLine();
        }
    }
}
