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
                var huService = new HURecommendService(null);
#warning 로그인 정보 외부에서 주입 하도록 처리 필요
                var loginSuccess = await huService.Login("", "");
                if (loginSuccess == false)
                {
                    Debugger.Break();
                    throw new Exception("로그인 실패!!!");
                }

                while (true)
                {
                    var postings = await huService.GetPostings(39);
                    foreach (var posting in postings)
                    {
                        Debugger.Break();
                        await huService.TryAssist(posting);
                    }

    #warning 특정 점수 구간에서는 대기 시간을 더 짧게 설정할 수 있도록 처리 필요 (HURecommendServiceOption)
                    await Task.Delay(5000);
                }
            }).Wait();

            Console.ReadLine();
        }
    }
}
