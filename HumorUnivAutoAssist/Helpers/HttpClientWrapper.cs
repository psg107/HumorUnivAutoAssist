using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Helpers
{
    /// <summary>
    /// Http Client Wrapper
    /// </summary>
    public class HttpClientWrapper
    {
        private static readonly CookieContainer cookieContainer;
        private static readonly HttpClientHandler httpClientHandler;
        private static readonly HttpClient client;

        static HttpClientWrapper()
        {
            cookieContainer = new CookieContainer();
            httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
            };
            client = new HttpClient(httpClientHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url, RequestOption option = null) where T : class
        {
            option = option ?? new RequestOption();

            T result = null;

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith((content) =>
                {
                    switch (option.ResponseType)
                    {
                        case ResponseType.Json:
                            result = JsonConvert.DeserializeObject<T>(content.Result);
                            break;

                        default:
                            throw new NotImplementedException($"정의되지 않은 타입 : {option.ResponseType}");
                    }
                });
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, RequestOption option = null)
        {
            option = option ?? new RequestOption();

            string result = null;

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(option.ResponseEncoding))
                {
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var byteContent = await response.Content.ReadAsByteArrayAsync();
                    result = Encoding.GetEncoding("euc-kr").GetString(byteContent, 0, byteContent.Length);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, T data, RequestOption option = null) where T : class
        {
            option = option ?? new RequestOption();

            T result = null;

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.PostAsync(url, data, new JsonMediaTypeFormatter());
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith((content) =>
                {
                    switch (option.ResponseType)
                    {
                        case ResponseType.Json:
                            result = JsonConvert.DeserializeObject<T>(content.Result);
                            break;

                        default:
                            throw new NotImplementedException($"정의되지 않은 타입 : {option.ResponseType}");
                    }
                });
            }

            return result;
        }
    }
}
