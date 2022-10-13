using Lab1.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Inventory
        
    {
        private Dictionary<Product, int> _products = new Dictionary<Product, int>();

        public Inventory(Dictionary<Product, int> products)
        {
            _products = products;
        }

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

        public void Add(Product product, int quantity)
        {
            if (this._products.ContainsKey(product))
            {
                this._products[product] = this._products[product] + quantity;

            }
            else
            {
                this._products.Add(product, quantity);
            }
        }

        public void Remove(Product product, int quantity)
        {
            if (this._products.ContainsKey(product))
            {
                if(this.GetQuantiyProduct(product) > quantity)
                    this._products[product] = this._products[product] - quantity;
                else
                {
                    throw new InventoryException("Quantity should be lower or equal than the product's quantity!");
                }
                if (this.GetQuantiyProduct(product) == 0)
                {
                    this._products.Remove(product);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder productsString = new StringBuilder();
            foreach (Product product in this.GetAll())
            {
                productsString.Append("Name = ").Append(product.Name).Append(", Quantity = ").Append(this.GetQuantiyProduct(product)).Append(";\n");
            }

            return productsString.ToString();
        }
    }
}
