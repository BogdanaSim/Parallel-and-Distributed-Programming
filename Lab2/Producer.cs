using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class Producer
    {
        private SynchronizedCollection<double> v1;
        private SynchronizedCollection<double> v2;
        private Common common;


        public Producer(SynchronizedCollection<double> v1, SynchronizedCollection<double> v2, Common common)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.common = common;
        }

        public void produce()
        {
            
            for (int i=0; i < v1.Count; i++)
            {
                
                lock (common.sync) 
                {
                    if (!common.cond)
                    {
                        common.currentProduct = v1[i] * v2[i];
                        Console.WriteLine("Produced -->" + v1[i] + "*" + v2[i] + " = " + common.currentProduct);
                        common.cond = true;
                        Monitor.Pulse(common.sync);
                        if (i == v1.Count - 1)
                        {
                            common.done = true;
                            break;
                        }
                    }
                    Monitor.Wait(common.sync);
                    
                }
                
            }
            
        }
    }
}
