namespace Lab1
{
    public class Bill
    {
        private readonly object locker = new object();
        private Dictionary<Product, SynchronizedCollection<int>> products = new Dictionary<Product, SynchronizedCollection<int>>();

        public Bill(Dictionary<Product, SynchronizedCollection<int>> products)
        {
            this.products = products;
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
                foreach (Product product in this.products.Keys)
                {
                    foreach (int quantity in this.products[product])
                        //Total += product.Price * this.GetQuantiyProduct(product);
                        Total = Add(ref Total, product.Price * quantity);
                }
                return Total;
            }
        }
    }
}