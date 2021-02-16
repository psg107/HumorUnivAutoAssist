using System;
using System.Text;

namespace HumorUnivAutoAssist
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Console.WriteLine("Hello World!");
            var service = new HURecommendService();
            service.GetList().Wait();
        }
    }
}
