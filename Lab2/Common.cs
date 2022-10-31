using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class Common
    {
        public object sync { get; set; }
        public double currentProduct { get; set; }
        public double total { get; set; }
        public bool done { get; set; }

        public Common(object sync, double currentProduct, double total, bool done)
        {
            this.sync = sync;
            this.currentProduct = currentProduct;
            this.total = total;
            this.done = done;
        }
    }
}
