using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public class Manager
    {
        public Manager(string url, ILog logger)
        {
            this.url = url;
            this.logger = logger;
            PageUrlCollection = new ConcurrentDictionary<string, bool>();
            PageUrlCollection.TryAdd(url, false);
        }
        private readonly string url;
        private ILog logger;
        private ConcurrentDictionary<string, bool> PageUrlCollection;
        private List<string> FileUrlCollection = new List<string>();

        public (int, int) DownloadFiles(string folder)
        {
            int success = 0, failure = 0;
            Parallel.For(0, FileUrlCollection.Count, (index) =>
            {
                if (HttpUtil.DownloadFile(FileUrlCollection[index], folder)) { success++; }
                else { failure++; }
            });
            return (success, failure);
        }

        public int StartGrawl()
        {
            int addCount = PageUrlCollection.Count;
            List<string> unUsedList = PageUrlCollection.Where(item => !item.Value).Select(item => item.Key).ToList();
            Parallel.For(0, unUsedList.Count, (index) =>
            {
                List<string> urlList = new Extractor(unUsedList[index], logger)?.GetAttributesAsync("//a[@href]", "href").Result;
                if (urlList == null || urlList.Count <= 0) { return; }
                Parallel.For(0, urlList.Count, i =>
                {
                    if (urlList[i].StartsWith("//"))
                    {
                        urlList[i] = string.Concat(unUsedList[index].Substring(0, unUsedList[index].IndexOf("//")), urlList[i]);
                    }
                    PageUrlCollection.TryAdd(urlList[i], false);
                });
            });
            int result = PageUrlCollection.Count - addCount;
            logger.WriteLog($"Inserted {result} url.");
            return result;
        }
    }
}
