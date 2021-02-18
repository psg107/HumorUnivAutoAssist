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

                Console.WriteLine("로그인 성공..");

                while (true)
                {
                    var minScore = 39;
                    var postings = await huService.GetPostings(minScore);
                    if (postings.Count > 0)
                    {
                        Console.WriteLine($"점수 {minScore} 이상 게시글 {postings.Count}개 발견");
                    }

                    foreach (var posting in postings)
                    {
                        Debugger.Break();
                        await huService.TryAssist(posting);

                        Console.WriteLine($"게시글 '{posting.Title}' 어시스트 시도!!");
                    }

#warning 특정 점수 구간에서는 대기 시간을 더 짧게 설정할 수 있도록 처리 필요 (HURecommendServiceOption)
                    Console.WriteLine($"대기중..");
                    await Task.Delay(5000);
                }
            }).Wait();

            Console.ReadLine();
        }
    }
}
