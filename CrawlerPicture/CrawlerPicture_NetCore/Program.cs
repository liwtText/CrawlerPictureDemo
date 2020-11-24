using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlerPicture_NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "https://m.bnmanhua.com/comic/10237/1212736.html";
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Crawler Process Start.");
            HtmlAgilityPack.HtmlDocument htmlcode = CrawlerHelper.GetHtmlSourceCode(uri);
            var picturelist = CrawlerHelper.GetPictureUrls(htmlcode);
            for (int i = 0; i < picturelist.Count; i++)
            {
                if (picturelist[i].StartsWith("//"))
                {
                    picturelist[i] = string.Concat(uri.Substring(0, uri.IndexOf("//")), picturelist[i]);
                }
            }
            string imageCollectionDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(imageCollectionDirectory))
            {
                Directory.CreateDirectory(imageCollectionDirectory);
            }
            Console.WriteLine("Download images into {0}", imageCollectionDirectory);
            foreach (string str in picturelist)
            {
                CrawlerHelper.DownLoadImage(str, imageCollectionDirectory);
            }
            Console.WriteLine(string.Format("{0} pictures download over.", picturelist.Count));
            var linklist = CrawlerHelper.GetLinkUrls(htmlcode);
            var linkList = new List<string>();
            linklist = linklist.Distinct().ToList();
            Parallel.For(0, linklist.Count, i =>
            {
                linkList.AddRange(CrawlerHelper.GetLinkUrls(CrawlerHelper.GetHtmlSourceCode(linklist[i])));
            });
            linklist = linklist.Distinct().ToList();
            Parallel.For(0, linklist.Count, str =>
            {
                linkList.AddRange(CrawlerHelper.GetLinkUrls(CrawlerHelper.GetHtmlSourceCode(linklist[str])));
            });
            linklist = linklist.Distinct().ToList();
            foreach (string str in linklist)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine(string.Format("Found {0} links.", linklist.Count + linkList.Count));
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine("All cost {0} ms.", ts.TotalMilliseconds);
            Console.ReadKey();
        }
    }
}
