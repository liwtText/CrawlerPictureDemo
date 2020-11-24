using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlerCore
{
    public interface ILog
    {
        bool WriteLog(string message);
    }

    public class LazyLogger : ILog
    {
        public bool WriteLog(string message)
        {
            Console.WriteLine($"LogInfo: {message}");
            return true;
        }
    }
}
