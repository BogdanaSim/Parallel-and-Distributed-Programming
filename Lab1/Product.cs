using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Product
    {
        private int id { get; set; }
        private string name { get; set; }
        private double price { get; set; }

        public Product(string name, double price)
        {
            this.name = name;
            this.price = price;
        }
    }


}
