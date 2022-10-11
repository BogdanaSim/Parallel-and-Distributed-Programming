using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Repository
    {
        private Dictionary<Product, int> _products = new Dictionary<Product, int>();
        

        public int GetQuantiyProduct(Product product)
        {
            return this._products[product];
        }

        public HashSet<Product> GetAll()
        {
            HashSet<Product> products = new HashSet<Product>();
            foreach(Product product in this._products.Keys)
            {
                products.Add(product);
            }
            return products;
        }

        public void Add(Product product, int quanity)
        {
            if (this._products.ContainsKey(product))
            {
                this._products[product] = this._products[product] + quanity;

            }
            else
            {
                this._products.Add(product, quanity);
            }
        }
    }
}
