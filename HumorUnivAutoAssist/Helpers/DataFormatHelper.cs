namespace HumorUnivAutoAssist.Helpers
{
    /// <summary>
    /// 데이터 포맷 헬퍼
    /// </summary>
    public static class DataFormatHelper
    {
        /// <summary>
        /// string -> int
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int ToInt(this string number)
        {
            if (int.TryParse(number, out int output))
            {
                return output;
            }
            return 0;
        }
    }
}
