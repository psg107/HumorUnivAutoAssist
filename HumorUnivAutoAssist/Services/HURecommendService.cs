﻿using HtmlAgilityPack;
using HumorUnivAutoAssist.Models;
using SimpleHttpClientWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class HURecommendService : IHURecommendService
    {
        private const string BASE_URL = "http://web.humoruniv.com/board/humor";

        private readonly HURecommendServiceOption option;

        public HURecommendService()
        {
            this.option = new HURecommendServiceOption();
        }

        public HURecommendService(HURecommendServiceOption option)
        {
            this.option = option ?? new HURecommendServiceOption();
        }

        public async Task<bool> Login(string id, string password)
        {
            var result = await HttpClientWrapper.PostStringAsync("https://web.humoruniv.com/user/login_process.php", new Login
            {
                Url = string.Empty,
                Id = id,
                Pw = password
            }, 
            new RequestOption
            {
                RequestType = RequestType.QueryString,
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Referer", "https://web.humoruniv.com/user/login.html" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "ko,en-US;q=0.9,en;q=0.8" },
                }
            });

            var setCookieCount = result.Headers.Count(x => x.Key == "Set-Cookie");

            return setCookieCount != 0;
        }

        public async Task<List<HumorPosting>> GetPostings(int minScore)
        {
            var result = await HttpClientWrapper.GetStringAsync("http://web.humoruniv.com/board/humor/list.html?table=pdswait&st=day", new RequestOption
            {
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" }
                },
                ResponseEncoding = "euc-kr",
            });

            if (!result.Success)
            {
                Debugger.Break();
                throw new Exception($"Fail..");
            }

            List<HumorPosting> postings = new List<HumorPosting>();

            var doc = new HtmlDocument();
            doc.LoadHtml(result.Data);
            var postNodes = doc.DocumentNode.SelectNodes("//tr[contains(@id, 'li_chk_pdswait')]");

            foreach (var postNode in postNodes)
            {
                var aTagNode = postNode.SelectSingleNode("td[2]/a");
                var upTagNode = postNode.SelectSingleNode("td[6]/span");
                var downTagNode = postNode.SelectSingleNode("td[7]/font");

                var title = Regex.Replace(aTagNode.InnerHtml, "<span class=\"list_comment_num\"> \\[\\d+\\]</span>", string.Empty).Trim();
                var url = $"{BASE_URL}/{aTagNode.GetAttributeValue("href", "")}";
                var up = int.Parse(upTagNode.InnerText);
                var down = int.Parse(downTagNode.InnerText);

                var score = up * this.option.UpWeight - down * this.option.DownWeight;

                if (score >= minScore)
                {
                    postings.Add(new HumorPosting
                    {
                        Title = title,
                        UpScore = up,
                        DownScore = down,
                        Url = url
                    });
                }
            }

            return postings;
        }

        public Task TryAssist(HumorPosting posting)
        {
            Debugger.Break();
            throw new NotImplementedException();
        }
    }
}
