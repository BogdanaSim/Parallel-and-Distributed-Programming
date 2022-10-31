﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class Consumer
    {
        private Common common;

        public Consumer( Common common)
        {
            this.common = common;
        }

        public void consume()
        {
            lock (common.sync)
            {
                while (true)
            {
                
                    common.total += common.currentProduct;
                    Console.WriteLine("Consumed -->" + common.currentProduct);
                    Monitor.Pulse(common.sync);
                    if (common.done == true)
                    {
                        break;
                    }
                    Monitor.Wait(common.sync);
                    
                }
            }
           

        }
    }
}
