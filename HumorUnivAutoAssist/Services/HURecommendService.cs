using HtmlAgilityPack;
using HumorUnivAutoAssist.Helpers;
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
        private readonly HURecommendServiceOption option;

        #region ctor

        public HURecommendService()
        {
            this.option = new HURecommendServiceOption();
        }

        public HURecommendService(HURecommendServiceOption option)
        {
            this.option = option ?? new HURecommendServiceOption();
        } 

        #endregion

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
            var loginSuccess = setCookieCount != 0;
            if (loginSuccess == false)
            {
                throw new Exception($"로그인 실패.. {result.Data}");
            }

            await WaitForCookieDelay();

            return loginSuccess;
        }

        public async Task<List<HumorPosting>> GetPostings(int checkScore, int minScore)
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

            List<HumorPosting> assistInfoList = new List<HumorPosting>();

            var doc = new HtmlDocument();
            doc.LoadHtml(result.Data);
            var postNodes = doc.DocumentNode.SelectNodes("//tr[contains(@id, 'li_chk_pdswait')]");

            foreach (var postNode in postNodes)
            {
                var aTagNode = postNode.SelectSingleNode("td[2]/a");
                var upTagNode = postNode.SelectSingleNode("td[6]/span");
                var downTagNode = postNode.SelectSingleNode("td[7]/font");

                var id = int.Parse(postNode.Id.Replace("li_chk_pdswait-", ""));
                var up = int.Parse(upTagNode.InnerText);
                var down = int.Parse(downTagNode.InnerText);

                var score = up * this.option.UpWeight - down * this.option.DownWeight;

                if (score >= checkScore)
                {
                    var assistInfo = await GetRealPostingInfo(id);
                    if (assistInfo != null)
                    {
                        if (assistInfo.UpScore * this.option.UpWeight - assistInfo.DownScore * this.option.DownWeight >= minScore)
                        {
                            assistInfoList.Add(assistInfo);
                        }
                    }
                }
            }

            return assistInfoList;
        }

        public async Task<bool> TryAssist(HumorPosting humorPosting)
        {
            if (humorPosting.AssistInfo == null)
            {
                throw new Exception($"'{humorPosting.Id}'번 게시글 '{humorPosting.Title}'의 어시스트 정보 없음..");
            }

            var result = await HttpClientWrapper.PostStringAsync($"http://web.humoruniv.com/board/humor/ok.php?mode=board&table=pdswait&number={humorPosting.Id}&pg=0&st=day", humorPosting.AssistInfo,
            new RequestOption
            {
                RequestType = RequestType.QueryString,
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Referer", $"http://web.humoruniv.com/board/humor/read.html?table=pdswait&st=day&pg=0&number={humorPosting.Id}" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "ko,en-US;q=0.9,en;q=0.8" },
                }
            });

            if (result.Data.Contains("귀하의 추천으로 게시물이 웃긴자료 게시판으로 이동되었습니다"))
            {
                return true;
            }
            
            return false;
        }

        #region private methods
        
        /// <summary>
        /// 쿠키 갱신 오류를 위한 딜레이
        /// </summary>
        /// <returns></returns>
        private async Task WaitForCookieDelay()
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
            
            LogHelper.Log($"{this.option.WaitTime}초 대기 시작!");
            await Task.Delay(this.option.WaitTime * 1000);
            LogHelper.Log($"{this.option.WaitTime}초 대기 완료!");
        }

        /// <summary>
        /// 해시 가져오기
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private async Task<string> GetHash(int number)
        {
            var result = await HttpClientWrapper.GetStringAsync($"http://web.humoruniv.com/board/humor/read.html?table=pdswait&st=day&pg=0&number={number}", new RequestOption
            {
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "ko,en-US;q=0.9,en;q=0.8" },
                },
                ResponseEncoding = "euc-kr",
            });

            if (!result.Success)
            {
                Debugger.Break();
                throw new Exception($"Fail..");
            }

            if (result.Data.Contains("rf_hash") == false)
            {
                LogHelper.Log($"해시 가져오기 실패.. {result.Data}");
                //throw new Exception($"해시 가져오기 실패.. {result.Data}");
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result.Data);
            var hash = doc.DocumentNode.SelectSingleNode("//input[@id='rf_hash']").GetAttributeValue("value", "");

            return hash;
        }

        /// <summary>
        /// Hash_Check 생성 (node 설치 필요)
        /// </summary>
        /// <param name="number"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private string GenerateHashCheck(int number, string hash)
        {
            //http://web.humoruniv.com/js/hidden_recomm_hash.js
            //hform.hash_check.value = henc_a3(document.recomm.number.value + document.recomm.hash.value);

            //http://web.humoruniv.com/js/h_hash.js
            //henc_a3(str) : return henc_a1(henc_a5(str + '9' + 'h' + '1' + 'u' + '3' + 'm' + '1' + 'o' + '7' + 'r' + '9'))

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

        /// <summary>
        /// 실제 게시글에서 정보 가져오기
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private async Task<HumorPosting> GetRealPostingInfo(int number)
        {
            var result = await HttpClientWrapper.GetStringAsync($"http://web.humoruniv.com/board/humor/read.html?table=pdswait&st=day&pg=0&number={number}", new RequestOption
            {
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                    { "Accept-Encoding", "gzip, deflate, br" },
                    { "Accept-Language", "ko,en-US;q=0.9,en;q=0.8" },
                },
                ResponseEncoding = "euc-kr",
            });

            if (!result.Success)
            {
                Debugger.Break();
                throw new Exception($"Fail..");
            }

            if (result.Data.Contains("rf_hash") == false)
            {
                LogHelper.Log($"해시 가져오기 실패.. {result.Data}");
                //throw new Exception($"해시 가져오기 실패.. {result.Data}");
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result.Data);

            var title = doc.DocumentNode.SelectSingleNode("//span[@id='ai_cm_title']").InnerText;
            var upScore = doc.DocumentNode.SelectSingleNode("//span[@id='ok_div']").InnerText.ToInt();
            var downScore = doc.DocumentNode.SelectSingleNode("//span[@id='not_ok_span']").InnerText.ToInt();
            var hash = doc.DocumentNode.SelectSingleNode("//input[@id='rf_hash']").GetAttributeValue("value", "");
            var hashCheck = GenerateHashCheck(number, hash);

#warning url..
            var posting = new HumorPosting
            {
                Id = number,
                Title = title,
                UpScore = upScore,
                DownScore = downScore,
                Url = "",
                AssistInfo = new Assist
                {
                    Number = number,
                    Hash = hash,
                    HashCheck = hashCheck,
                }
            };

            return posting;
        }

        #endregion
    }
}
