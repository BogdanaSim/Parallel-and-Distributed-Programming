using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{

    public class Bill : Inventory
    {
        public Bill(Dictionary<Product, int> products) : base(products)
        {
        }

        public double GetTotalPrice()
        {
            double Total = 0;
            foreach (Product product in this.GetAll())
            {
          
                Total+=product.Price*this.GetQuantiyProduct(product);
            }
            return Total;
        }

    }
}
