using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerCore
{
    public interface IExtractor
    {
        Task<int> GetNodesCountAsync(string xpath);
        Task<List<string>> GetAttributesAsync(string xpath, string attributeName, bool distinct = true);
    }

    //观察者
    public class Extractor : IExtractor
    {
        public Extractor(HtmlDocument document, ILog logger)
        {
            this.logger = logger;
            this.document = document;
        }

        public Extractor(string url, ILog logger)
        {
            this.logger = logger;
            this.document = GetHtmlSourceCode(url).Result;
        }
        private ILog logger;
        private HtmlDocument document;
        
        /// <summary>
        /// 获取页面源码
        /// </summary>
        private async Task<HtmlDocument> GetHtmlSourceCode(string url)
        {
            logger.WriteLog($"Get source code from {url}");
            if (string.IsNullOrWhiteSpace(url)) { return null; }
            if (url.StartsWith("#") || url.ToLower().StartsWith("javascript")) { return null; }
            HtmlDocument document = null;
            var webClient = new HtmlWeb
            {
                UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36"
            };
            try
            {
                document = await webClient.LoadFromWebAsync(url);
            }
            catch (Exception ex)
            {
                logger.WriteLog($"An error appear when getting source code of {url}.. The detail of exception is {ex}");
                return null;
            }
            return document;
        }
        /// <summary>
        /// 获取节点个数
        /// </summary>
        public async Task<int> GetNodesCountAsync(string xpath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return document.DocumentNode.SelectNodes(xpath).Count;
                }
                catch (Exception ex)
                {
                    logger.WriteLog($"An exception appear when getting count of nodes. The detail of exception is {ex}");
                    return 0;
                }
            });
        }
        /// <summary>
        /// 获取标签属性
        /// </summary>
        public async Task<List<string>> GetAttributesAsync(string xpath, string attributeName, bool distinct = true)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var attriList = new List<string>();
                    var nodeList = document?.DocumentNode?.SelectNodes(xpath);
                    if (nodeList != null)
                    {
                        foreach (HtmlNode attriNode in nodeList)
                        {
                            HtmlAttribute att = attriNode.Attributes[attributeName];
                            if (!string.IsNullOrWhiteSpace(att.Value))
                            {
                                attriList.Add(att.Value);
                            }
                        }
                    }
                    return distinct ? attriList.Distinct().ToList() : attriList;
                }
                catch (Exception ex)
                {
                    logger.WriteLog($"An exception appear when getting attributes[{attributeName}] of nodes. The detail of exception is {ex}");
                    return null;
                }
            });
        }
    }
}
