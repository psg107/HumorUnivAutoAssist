using HtmlAgilityPack;
using HumorUnivAutoAssist.Models;
using HumorUnivAutoAssist.Models.DataTransfers;
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
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                throw new Exception("아이디 또는 패스워드가 입력되지 않음..");
            }

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

                var id = int.Parse(postNode.Id.Replace("li_chk_pdswait-", ""));
                var title = Regex.Replace(aTagNode.InnerHtml, "<span class=\"list_comment_num\"> \\[\\d+\\]</span>", string.Empty).Trim();
                var url = $"{BASE_URL}/{aTagNode.GetAttributeValue("href", "")}";
                var up = int.Parse(upTagNode.InnerText);
                var down = int.Parse(downTagNode.InnerText);

                var score = up * this.option.UpWeight - down * this.option.DownWeight;

                if (score >= minScore)
                {
                    postings.Add(new HumorPosting
                    {
                        Id = id,
                        Title = title,
                        UpScore = up,
                        DownScore = down,
                        Url = url
                    });
                }
            }

            return postings;
        }

        public async Task TryAssist(HumorPosting posting)
        {
            throw new NotImplementedException();

            //http://web.humoruniv.com/js/hidden_recomm_hash.js
            //hform.hash_check.value = henc_a3(document.recomm.number.value + document.recomm.hash.value);

            //http://web.humoruniv.com/js/h_hash.js
            //henc_a3(str) : return henc_a1(henc_a5(str + '9' + 'h' + '1' + 'u' + '3' + 'm' + '1' + 'o' + '7' + 'r' + '9'))

#warning 테스트..
            var hash = GenerateHashCheck(8462086, "c7eaa269d923812d8430aff71b642d1f7494314e");

            var result = await HttpClientWrapper.PostStringAsync($"http://web.humoruniv.com/board/humor/ok.php?mode=board&table=pdswait&number={posting.Id}&pg=0&st=day", new Assist
            {
                Number = posting.Id,
                Hash = "..",
                HashCheck = ".."
            },
            new RequestOption
            {
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Referer", $"http://web.humoruniv.com/board/humor/read.html?table=pdswait&st=day&pg=0&number={posting.Id}" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "ko,en-US;q=0.9,en;q=0.8" },
                }
            });
            Debugger.Break();
        }

        /// <summary>
        /// Hash_Check 생성 (node 설치 필요)
        /// </summary>
        /// <param name="number"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private string GenerateHashCheck(int number, string hash)
        {
            var hashCheck = string.Empty;

            var proc = new System.Diagnostics.Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
#warning 노드 설치 경로 확인 필요
            proc.StartInfo.FileName = @"C:\Program Files\nodejs\node.exe";
            proc.StartInfo.Arguments = @$"Scripts\GenerateHashCheckValue_Deobfuscation.js {number} {hash}";
            proc.Start();
            proc.BeginOutputReadLine();

            proc.OutputDataReceived += (s, e) =>
            {
                hashCheck = e.Data;
                proc.CancelOutputRead();
            };
            proc.WaitForExit();

            return hashCheck;
        }
    }
}
