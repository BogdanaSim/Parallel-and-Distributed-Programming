using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{

    public class Bill : Inventory
    {
        private readonly object locker = new object();
        public Bill(Dictionary<Product, int> products) : base(products)
        {
        }
        public static double Add(ref double location1, double value)
        {
            double newCurrentValue = location1;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue.Equals(currentValue))
                    return newValue;
            }
        }
        public double GetTotalPrice()
        {
            lock (locker)
            {
                double Total = 0;
                foreach (Product product in this.GetAll())
                {

                    //Total += product.Price * this.GetQuantiyProduct(product);
                    Total = Add(ref Total,product.Price * this.GetQuantiyProduct(product));
                }
                return Total;
            }
        }

    }
}
