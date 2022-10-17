using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            //try
            //{
            //    Monitor.Enter(Inventory, ref IsLocked);
            //    try
            //    {
            Inventory.Add(product, quantity);

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}
            //finally
            //{
            //    if (IsLocked)
            //    {
            //        Monitor.Exit(Inventory);
            //        IsLocked = false;
            //    }
            // }
        }

        public void RemoveFromInventory(Product product, int quantity)
        {

            //try
            //{
            //    Monitor.Enter(Inventory, ref IsLocked);
            try
            {
                Inventory.Remove(product, quantity);
                Console.WriteLine("Sale " + id + ": purchased " + product.Name + "; quantity -  " + Inventory.GetQuantiyProduct(product));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //}
            //finally
            //{
            //    if (IsLocked)
            //    {
            //        Monitor.Exit(Inventory);
            //        IsLocked = false;
            //    }
            //}
        }


    }
}
