using System;
using System.Threading.Tasks;

namespace CrawlerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //可以用来处理异步程序异常时，由于没有wait或者result导致主线程没有检测到异常发生，当异步线程被回收时，会调用此方法（包含异步异常）。
            //TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Console.WriteLine("Hello World!");

            Manager manager = new Manager("https://blog.csdn.net/rookie_is_me/article/details/81634048", new LazyLogger());
            manager.StartGrawl();
            manager.StartGrawl();
            Console.WriteLine("Ding dong.");
            Console.ReadKey();
        }

        //private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        //{
        //    if (e.Exception != null) {
        //       Console.WriteLine(e.Exception);
        //    }
        //}
    }
}
