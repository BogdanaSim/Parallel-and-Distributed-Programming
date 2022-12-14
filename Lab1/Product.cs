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

        public override string ToString()
        {
            return "Product name: " + Name + "; Product price: " + Math.Round(Price, 5);
        }
    }
}