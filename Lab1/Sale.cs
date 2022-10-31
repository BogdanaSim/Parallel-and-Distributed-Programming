namespace Lab1
{
    public class Sale
    {
        private int id;
        private bool IsLocked = false;
        private Inventory Inventory;
        private Object mutex = new Object();

        public Sale(Inventory inventory, int id)
        {
            Inventory = inventory;
            this.id = id;
        }

        public void Run()
        {
            foreach (Product product in Inventory.GetAll())
            {
                try
                {
                    Monitor.Enter(Inventory, ref IsLocked);
                    try
                    {
                        Inventory.Remove(product, Inventory.GetQuantiyProduct(product));
                        Console.WriteLine("Sale " + id + ": purchased " + product.Name + "; quantity -  " + Inventory.GetQuantiyProduct(product));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                finally
                {
                    if (IsLocked)
                    {
                        Monitor.Exit(Inventory);
                    }
                }
            }
        }

        public void AddToInventory(Product product, int quantity)
        {
            Inventory.Add(product, quantity);
        }

        public void RemoveFromInventory(Product product, int quantity)
        {
            try
            {
                Inventory.Remove(product, quantity);
                Console.WriteLine("Sale " + id + ": purchased " + product.Name + "; quantity -  " + quantity + " (remaining quantity: " + Inventory.GetQuantiyProduct(product) + ")");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}