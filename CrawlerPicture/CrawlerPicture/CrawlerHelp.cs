using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerPicture
{
    public class CrawlerHelp
    {
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        public static void DownLoadImage(string url, string path)
        {
            using (WebClient mywebclient = new WebClient())
            {
                string filename = url;
                filename = filename.Substring(filename.LastIndexOf('/') + 1);
                mywebclient.DownloadFile(url, Path.Combine(path, filename));
            }
        }
        /// <summary>
        /// 获取网页源代码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHtmlSourceCode(string url)
        {
            Console.WriteLine("Get html source code from {0}", url);
            Uri uri = new Uri(url);
            string strHTML = "";
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
                //添加http请求head
                myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
                myReq.Accept = "*/*";
                myReq.KeepAlive = true;
                myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //获取http响应
                HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
                Stream receviceStream = result.GetResponseStream();
                StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
                //获取网页html源码
                strHTML = readerOfStream.ReadToEnd();
                readerOfStream.Close();
                receviceStream.Close();
                result.Close();
            }
            catch (Exception)
            {
                strHTML = "";
            }
            return strHTML;
        }
        /// <summary>
        /// 取出源代码中的图片信息
        /// </summary>
        /// <param name="htmlcode"></param>
        /// <returns></returns>
        public static List<string> GetPictureUrls(string htmlcode)
        {
            Console.WriteLine("Extract images from html code.");
            List<string> retlist = new List<string>();
            string imgPattern = @"img src=[""'](.+?)[""']";//@"img src=[""'](.+?\.(jpg|png|bmp))[""']"
            if (Regex.IsMatch(htmlcode, imgPattern))
            {
                MatchCollection matchcol = Regex.Matches(htmlcode, imgPattern);
                foreach (Match match in matchcol)
                {
                    retlist.Add(match.Groups[1].ToString());
                }
            }
            return retlist;
        }
        /// <summary>
        /// 取出源代码中的链接地址
        /// </summary>
        /// <param name="htmlcode"></param>
        /// <returns></returns>
        public static List<string> GetLinkUrls(string htmlcode)
        {
            Console.WriteLine("Extract link urls from html code.");
            List<string> retlist = new List<string>();
            string utlPattern = "href=(?:[\"'](?<1>http[^\"']*)[\"']|(?<1>\\S+))";
            if (Regex.IsMatch(htmlcode, utlPattern))
            {
                MatchCollection matchcol = Regex.Matches(htmlcode, utlPattern);
                Parallel.For(0, matchcol.Count, i =>
                    {
                        if (!matchcol[i].Groups[1].ToString().StartsWith("\"") && !matchcol[i].Groups[1].ToString().EndsWith(".css") && !matchcol[i].Groups[1].ToString().EndsWith(".js")
                        && !matchcol[i].Groups[1].ToString().EndsWith(".ico") && !retlist.Contains(matchcol[i].Groups[1].ToString()))
                        {
                            retlist.Add(matchcol[i].Groups[1].ToString());
                        }
                    });
            }
            return retlist;
        }
    }
}
