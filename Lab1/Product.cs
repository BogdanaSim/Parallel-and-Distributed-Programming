using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab1
{
    public class Product
    {
        public string Name { get; private set; }
        public double Price { get; private set; }

        public Product(string name, double price)
        {
            this.Name = name;
            this.Price = price;
        }

        public Product()
        {
            this.Name = "";
            this.Price = 0;
        }
    }


}
