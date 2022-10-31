using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace Lab2
{

    public class MainClass
    {
        private static SynchronizedCollection<double> v1 = new SynchronizedCollection<double>() { 1,2,3,4};
        private static SynchronizedCollection<double> v2 = new SynchronizedCollection<double>() { 5,6,7,8};
        private static readonly object sync = new object();
        private static Double currentProduct=1;
        private static Double total = 0;
        private static Boolean done=false;
        static void Main(string[] args)
        {
            Common common = new Common(sync, currentProduct, total, done);
            Producer producer = new Producer(v1, v2, common);
            Consumer consumer = new Consumer(common);
            Thread t1 = new Thread(producer.produce);
            Thread t2 = new Thread(consumer.consume);
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Console.WriteLine("total sum = "+ common.total);
        }
    }
}