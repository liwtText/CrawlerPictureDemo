using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace CrawlerPicture
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "https://blog.csdn.net/chordwang/article/details/54291361";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Process Start.");
            string htmlcode = CrawlerHelp.GetHtmlSourceCode(uri);
            List<string> picturelist = CrawlerHelp.GetPictureUrls(htmlcode);
            for (int i = 0; i < picturelist.Count; i++)
            {
                if (picturelist[i].StartsWith("//"))
                {
                    picturelist[i] = string.Concat(uri.Substring(0, uri.IndexOf("//")), picturelist[i]);
                }
            }
            string imageCollectionDirectory = Path.Combine(Directory.GetCurrentDirectory(),  "Images");
            if (!Directory.Exists(imageCollectionDirectory))
            {
                Directory.CreateDirectory(imageCollectionDirectory);
            }
            Console.WriteLine("Download images into {0}", imageCollectionDirectory);
            foreach (string str in picturelist)
            {
                CrawlerHelp.DownLoadImage(str, imageCollectionDirectory);
            }
            Console.WriteLine(string.Format("{0} pictures download over.", picturelist.Count));
            List<string> linklist = CrawlerHelp.GetLinkUrls(htmlcode);
            List<string> linkList = new List<string>();
            foreach (string str in linklist)
            {
                linkList.AddRange(CrawlerHelp.GetLinkUrls(CrawlerHelp.GetHtmlSourceCode(str)));
            }
            Parallel.For(0, linklist.Count, str =>
            {
                linkList.AddRange(CrawlerHelp.GetLinkUrls(CrawlerHelp.GetHtmlSourceCode(linklist[str])));
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
