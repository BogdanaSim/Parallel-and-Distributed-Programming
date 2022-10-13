using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class MainClass
    {
        private static readonly int No_Threads = 10;
        private static readonly int No_Operations = 2;
        private static readonly int No_Products = 100;
        private static Dictionary<Product, int> productsList =new Dictionary<Product, int>();
        private static Inventory inventory = new Inventory(productsList);
        private static List<Bill> bills = new List<Bill>();
        private static List<Sale> sales = new List<Sale>();
        private static List<Thread> threads = new List<Thread>();
        private static double amountOfMoney = 0;



        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        public static void CreateProducts(Dictionary<Product, int> products)
        {
            Random random = new Random();
            for (int i = 0; i < No_Products; i++)
            {
                products.Add(new Product(RandomString(), (random.NextDouble() * 80) + 20), random.Next(0, 50));
            }

        }

        public static List<Bill> CreateBills(Dictionary<Product, int> products)
        {
            List<Bill> bills = new List<Bill>();
            Random random = new Random();
            foreach (Product product in products.Keys)
            {
                Dictionary<Product, int> op = new Dictionary<Product, int>();
                for (int i = 0; i < No_Operations; i++)
                {

                    if (products[product] <= 0) continue;
                    int q = random.Next(0, products[product] + 1);
                    if (q > 0)
                    {
                        op.Add(product, q);

                    }
                    if (op.Count > 0)
                    {
                        bills.Add(new Bill(op));
                    }
                }
            }

            return bills;
        }

        public static void Start(Sale sale)
        {
            Dictionary<Product, int> op = new Dictionary<Product, int>();
            Random random = new Random();
            for (int i = 0; i < No_Operations; i++)
            {
                int id = random.Next(0, productsList.Count - 1);
                if (productsList.ElementAt(id).Value <= 0) continue;
                int q = random.Next(0, productsList.ElementAt(id).Value + 1);
                if (q > 0)
                {
                    op.Add(productsList.ElementAt(id).Key, q);
                    amountOfMoney += productsList.ElementAt(id).Key.Price * q;
                    sale.RemoveFromInventory(productsList.ElementAt(id).Key, q);

                }
                if (op.Count > 0)
                {
                    bills.Add(new Bill(op));
                }

            }

        }

        public static void Checker(int delay)
        {
            while (true)
            {
                lock (inventory) lock(bills)
                {
                    double sum = 0;
                    foreach (Bill bill in bills.ToList())
                    {
                        sum += bill.GetTotalPrice();
                    }
                    Console.WriteLine("Checker: " + (amountOfMoney != sum));
                }

                Thread.Sleep(delay);
            }
        }

        static void Main(string[] args)
        {
            CreateProducts(productsList);
            Task.Run(() => Checker(10));
            for (int i = 0; i < No_Threads; i++)
            {
                Sale sale = new Sale(inventory, i);
                ThreadStart thread = () => Start(sale);
                threads.Add(new Thread(thread));

            }
            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                try
                {
                    thread.Join();
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
