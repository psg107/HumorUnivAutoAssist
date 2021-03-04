using Newtonsoft.Json;
using SimpleHttpClientWrapper.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleHttpClientWrapper
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
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            cookieContainer = new CookieContainer();
            httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            client = new HttpClient(httpClientHandler);
        }

        public static void SetCookie(string rawStringCookie, string domain)
        {
            foreach (var cookie in rawStringCookie.Split(';').Select(x => x.Trim()))
            {
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    continue;
                }

                var match = Regex.Match(cookie.Trim(), "^([%a-zA-Z0-9_-]+)=(.*)$");
                if (match.Success)
                {
                    var key = match.Groups[1].Value.Trim();
                    var value = match.Groups[2].Value.Trim();

                    cookieContainer.Add(new Cookie(key, value, "/", domain));
                }
                else
                {
                    throw new Exception($"" +
                        $"쿠키 정보를 세팅하지 못했습니다.\n" +
                        $"예외쿠키 : {cookie}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<RequestResult<T>> GetAsync<T>(string url, object data = null, RequestOption option = null)
            where T : class
        {
            option = option ?? new RequestOption();

            RequestResult<T> result = new RequestResult<T>();

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            
            if (data != null)
            {
                var queryString = QueryStringHelper.ObjectToQueryString(data);
                url = $"{url}?{queryString}";
            }

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith((content) =>
                {
                    switch (option.ResponseType)
                    {
                        case ResponseType.Json:
                            result.Data = JsonConvert.DeserializeObject<T>(content.Result);
                            break;

                        default:
                            throw new NotImplementedException($"정의되지 않은 타입 : {option.ResponseType}");
                    }
                });
            }

            result.StatusCode = (int)response.StatusCode;
            result.Headers = response.Headers.Concat(response.Content.Headers).ToList();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<RequestResult> GetStringAsync(string url, RequestOption option = null)
        {
            option = option ?? new RequestOption();

            RequestResult result = new RequestResult();

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
                    result.Data = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var byteContent = await response.Content.ReadAsByteArrayAsync();
                    if (byteContent.Length > 0)
                    {
                        result.Data = Encoding.GetEncoding(option.ResponseEncoding).GetString(byteContent, 0, byteContent.Length);
                    }
                }
            }

            result.StatusCode = (int)response.StatusCode;
            result.Headers = response.Headers.Concat(response.Content.Headers).ToList();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">response</typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<RequestResult<T>> PostAsync<T>(string url, object data, RequestOption option = null)
            where T : class
        {
            option = option ?? new RequestOption();

            RequestResult<T> result = new RequestResult<T>();

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            MediaTypeFormatter formatter = new Func<MediaTypeFormatter>(() =>
            {
                switch (option.RequestType)
                {
                    case RequestType.Json:
                        return new JsonMediaTypeFormatter();

                    case RequestType.QueryString:
                        return new QueryStringFormatter();

                    default:
                        throw new Exception($"Formatter?? {option.RequestType}");
                }
            }).Invoke();
            var mediaType = new Func<string>(() =>
            {
                switch (option.RequestType)
                {
                    case RequestType.Json:
                        return "application/json";

                    case RequestType.QueryString:
                        return "application/x-www-form-urlencoded";

                    default:
                        throw new Exception($"Formatter?? {option.RequestType}");
                }
            }).Invoke();

            var response = await client.PostAsync(url, data, formatter, mediaType);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync().ContinueWith((content) =>
                {
                    switch (option.ResponseType)
                    {
                        case ResponseType.Json:
                            result.Data = JsonConvert.DeserializeObject<T>(content.Result);
                            break;

                        default:
                            throw new NotImplementedException($"정의되지 않은 타입 : {option.ResponseType}");
                    }
                });
            }

            result.StatusCode = (int)response.StatusCode;
            result.Headers = response.Headers.Concat(response.Content.Headers).ToList();

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
        public static async Task<RequestResult> PostStringAsync<T>(string url, T data, RequestOption option = null) where T : class
        {
            option = option ?? new RequestOption();

            RequestResult result = new RequestResult();

            client.DefaultRequestHeaders.Clear();
            foreach (var header in option?.RequestHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            MediaTypeFormatter formatter = new Func<MediaTypeFormatter>(() =>
            {
                switch (option.RequestType)
                {
                    case RequestType.Json:
                        return new JsonMediaTypeFormatter();

                    case RequestType.QueryString:
                        return new QueryStringFormatter();

                    default:
                        throw new Exception($"Formatter?? {option.RequestType}");
                }
            }).Invoke();
            var mediaType = new Func<string>(() =>
            {
                switch (option.RequestType)
                {
                    case RequestType.Json:
                        return "application/json";

                    case RequestType.QueryString:
                        return "application/x-www-form-urlencoded";

                    default:
                        throw new Exception($"Formatter?? {option.RequestType}");
                }
            }).Invoke();

            var response = await client.PostAsync(url, data, formatter, mediaType);
            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(option.ResponseEncoding))
                {
                    result.Data = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var byteContent = await response.Content.ReadAsByteArrayAsync();
                    if (byteContent.Length > 0)
                    {
                        result.Data = Encoding.GetEncoding(option.ResponseEncoding).GetString(byteContent, 0, byteContent.Length);
                    }
                }
            }

            result.StatusCode = (int)response.StatusCode;
            result.Headers = response.Headers.Concat(response.Content.Headers).ToList();

            return result;
        }
    }
}
