using System;

namespace HumorUnivAutoAssist.Helpers
{
    /// <summary>
    /// 로그 헬퍼
    /// </summary>
    public static class LogHelper
    {
#warning 현재 단순 콘솔에 출력만 하는 중
        /// <summary>
        /// 로그 기록
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}] {message}");
        }
    }
}
