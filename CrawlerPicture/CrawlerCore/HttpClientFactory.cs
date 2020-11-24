using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public class HttpClientFactory
    {
        private static ConcurrentDictionary<int, HttpClient> dicHttpClients => new ConcurrentDictionary<int, HttpClient>();

        /// <summary>
        /// 创建HttpClient
        /// </summary>
        public static HttpClient CreateClient(int threadId)
        {
            if (!dicHttpClients.TryGetValue(threadId, out HttpClient httpClient) || httpClient == null)
            {
                httpClient = new HttpClient
                {
                    Timeout = new TimeSpan(0, 0, 3)
                };
                httpClient.DefaultRequestHeaders.Add("refer", "https://www.baidu.com/");
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("accept", "*/*");
                dicHttpClients.TryAdd(threadId, httpClient);
            }
            return httpClient;
        }
    }

    public static class HttpUtil
    { 
        private static HttpClient _imageDownloadClient;
        public static bool DownloadFile(string url, string folder)
        {
            if (string.IsNullOrWhiteSpace(url)) { return false; }
            try
            {
                _imageDownloadClient = HttpClientFactory.CreateClient(Thread.CurrentThread.ManagedThreadId);
                using (var responseStream = new MemoryStream(_imageDownloadClient.GetByteArrayAsync(url).Result))
                {
                    using (var writeStream = new FileStream(Path.Combine(folder, url.Substring(url.LastIndexOf('/') + 1)), FileMode.Create))
                    {
                        var bufferArr = new byte[2048];
                        var readSize = responseStream.Read(bufferArr, 0, bufferArr.Length);
                        while (readSize > 0)
                        {
                            writeStream.Write(bufferArr, 0, readSize);
                            readSize = responseStream.Read(bufferArr, 0, bufferArr.Length);
                        }
                        writeStream.Close();
                        responseStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
