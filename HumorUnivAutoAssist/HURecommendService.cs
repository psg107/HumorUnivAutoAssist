using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist
{
    /// <summary>
    /// 
    /// </summary>
    public class HURecommendService
    {
        private const string BASE_URL = "http://web.humoruniv.com/board/humor";
        private const string URL = "http://web.humoruniv.com/board/humor/list.html?table=pdswait&st=day";

        private readonly CookieContainer cookieContainer;
        private readonly HttpClientHandler httpClientHandler;
        private readonly HttpClient client;
        private readonly HURecommendServiceOption option;

        public HURecommendService(HURecommendServiceOption option = null)
        {
            this.cookieContainer = new CookieContainer();
            this.httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
            };
            this.client = new HttpClient(httpClientHandler);

            this.option = option ?? new HURecommendServiceOption();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task GetList()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");

            var response = await client.GetAsync(URL);
            if (response.IsSuccessStatusCode)
            {
                var byteContent = await response.Content.ReadAsByteArrayAsync();
                var content = Encoding.GetEncoding("euc-kr").GetString(byteContent, 0, byteContent.Length);

                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                var postNodes = doc.DocumentNode.SelectNodes("//tr[contains(@id, 'li_chk_pdswait')]");

                foreach (var postNode in postNodes)
                {
                    var aTagNode = postNode.SelectSingleNode("td[2]/a");
                    var upTagNode = postNode.SelectSingleNode("td[6]/span");
                    var downTagNode = postNode.SelectSingleNode("td[7]/font");

                    var title = Regex.Replace(aTagNode.InnerHtml, "<span class=\"list_comment_num\"> \\[\\d+\\]</span>", string.Empty).Trim();
                    var url = $"{BASE_URL}/{aTagNode.GetAttributeValue("href", "")}";
                    var up = upTagNode.InnerText;
                    var down = downTagNode.InnerText;

                    var score = int.Parse(up) * this.option.UpWeight - int.Parse(down) * this.option.DownWeight;
                    
                    if (score >= this.option.MinimumScore)
                    {
                        Debugger.Break();
                    }
                }
            }
        }
    }
}
