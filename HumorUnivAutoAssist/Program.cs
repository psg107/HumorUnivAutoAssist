using HumorUnivAutoAssist.Helpers;
using HumorUnivAutoAssist.Services;
using Microsoft.Extensions.Configuration;
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
#if DEBUG
            var configFileName = "appsettings_debug.json";
#else
            var configFileName = "appsettings.json";
#endif
            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile(configFileName, optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();

            var id = configuration.GetSection("id").Value;
            var password = configuration.GetSection("password").Value;

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

                var loginSuccess = await huService.Login(id, password);
                if (loginSuccess == false)
                {
                    Debugger.Break();
                    throw new Exception("로그인 실패!!!");
                }

                LogHelper.Log("로그인 성공!");

                while (true)
                {
                    var checkScore = 37;
                    var minScore = 39;
                    var humorPostings = await huService.GetPostings(checkScore, minScore);
                    if (humorPostings.Count > 0)
                    {
                        LogHelper.Log($"점수 {minScore} 이상 게시글 {humorPostings.Count}개 발견");
                    }

                    foreach (var humorPosting in humorPostings)
                    {
                        var isSuccessAssist = await huService.TryAssist(humorPosting);
                        LogHelper.Log(isSuccessAssist 
                                        ? $"'{humorPosting.Title}' 게시글 어시스트 성공!!"
                                        : $"'{humorPosting.Title}' 게시글 어시스트 실패..");

                        //Debugger.Break();
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
