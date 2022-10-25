using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private static Dictionary<Product, int> productsList = new Dictionary<Product, int>();
        private static Inventory inventory = new Inventory(productsList);
        private static SynchronizedCollection<Bill> bills = new SynchronizedCollection<Bill>();
        private static List<Sale> sales = new List<Sale>();
        private static List<Thread> threads = new List<Thread>();
        private static double amountOfMoney = 0;
        private static readonly object locker = new object();



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
                products.Add(new Product(RandomString(), (random.NextDouble() * 80) + 20), random.Next(1, 50));
            }

        }

        //public static List<Bill> CreateBills(Dictionary<Product, int> products)
        //{
        //    List<Bill> bills = new List<Bill>();
        //    Random random = new Random();
        //    foreach (Product product in products.Keys)
        //    {
        //        Dictionary<Product, int> op = new Dictionary<Product, int>();
        //        for (int i = 0; i < No_Operations; i++)
        //        {

        //            if (products[product] <= 0) continue;
        //            int q = random.Next(0, products[product] + 1);
        //            if (q > 0)
        //            {
        //                op.Add(product, q);

        //            }
        //            if (op.Count > 0)
        //            {
        //                bills.Add(new Bill(op));
        //            }
        //        }
        //    }

        //    return bills;
        //}

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

        public static void Start(Sale sale, int delay)
        {
            Dictionary<Product, SynchronizedCollection<int>> op = new Dictionary<Product, SynchronizedCollection<int>>();
            Random random = new Random();
            double sum = 0;
            // lock (inventory) lock (bills) { 
            //lock (bills) lock(locker)
            //{
            for (int i = 0; i < No_Operations; i++)
            {
                int id = random.Next(0, productsList.Count - 1);
                Product product = productsList.ElementAt(id).Key;
                lock (product)
                {
                    if (productsList.ElementAt(id).Value <= 0) continue;
                    int q = random.Next(0, productsList.ElementAt(id).Value + 1);
                    if (q > 0)
                    {

                        //op.Add(productsList.ElementAt(id).Key, q);
                        if (!op.ContainsKey(productsList.ElementAt(id).Key))
                        {
                            op[productsList.ElementAt(id).Key]=new SynchronizedCollection<int>();
                        }
                        op[productsList.ElementAt(id).Key].Add(q);



                        // amountOfMoney += productsList.ElementAt(id).Key.Price * q;
                        sale.RemoveFromInventory(productsList.ElementAt(id).Key, q);
                        sum += productsList.ElementAt(id).Key.Price * q;
                        //lock (bills) lock (locker)
                        //    {
                        //        amountOfMoney = Add(ref amountOfMoney, productsList.ElementAt(id).Key.Price * q);
                        //        //amountOfMoney += productsList.ElementAt(id).Key.Price * q;

                        //    }
                    }

                }
                //}
            }
            lock (bills) 
                {
                    amountOfMoney = Add(ref amountOfMoney, sum);
                    if (op.Count > 0)
                    {

                        bills.Add(new Bill(op));

                    }
                }
            // }

             //Thread.Sleep(delay);

        }

        public static void WriteDataToFile(string FileName)
        {
            
            using (StreamWriter file = new StreamWriter(FileName))
                foreach (var entry in inventory.GetProducts())
                    file.WriteLine("[{0}; quantity: {1}]", entry.Key, entry.Value);
        }

        public static string ResultChecker(int result)
        {
            if (result == 0)
                return "Verified!";
            return "Error Checker!";

        }

        public static void Checker(int delay)
        {
            while (true)
            {
                lock (bills) 
                    {

                        double sum = 0;
                        foreach (Bill bill in bills.ToList())
                        {
                            //sum += bill.GetTotalPrice();
                            sum = Add(ref sum, bill.GetTotalPrice());
                        }
                        Console.WriteLine("am = " + Math.Round(amountOfMoney, 5) + "; " + "sum = " + Math.Round(sum, 5));
                        Console.WriteLine("Checker: " + ResultChecker(Math.Round(amountOfMoney, 5).CompareTo(Math.Round(sum, 5))));

                    }

                Thread.Sleep(delay);
            }
        }

        static void Main(string[] args)
        {
            CreateProducts(productsList);
            inventory.GenerateLocks();
            WriteDataToFile(@"..\..\..\Files\data.txt");
            Task.Run(() => Checker(1));
            for (int i = 0; i < No_Threads; i++)
            {
                Sale sale = new Sale(inventory, i);
                ThreadStart thread = () => Start(sale, 7);
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
            // double sum = 0;
            //foreach (Bill bill in bills.ToList())
            //{
            //    sum += Add(ref sum, bill.GetTotalPrice());
            //}
            //Console.WriteLine("am = " + Math.Round(amountOfMoney, 5) + "; " + "sum = " + Math.Round(sum, 5));
            //Console.WriteLine("Checker: " + ((Math.Round(amountOfMoney, 5).CompareTo(Math.Round(sum, 5)))));

            lock (bills)
            {

                double sum = 0;
                foreach (Bill bill in bills.ToList())
                {
                    //sum += bill.GetTotalPrice();
                    sum = Add(ref sum, bill.GetTotalPrice());
                }
                Console.WriteLine("am = " + Math.Round(amountOfMoney, 5) + "; " + "sum = " + Math.Round(sum, 5));
                Console.WriteLine("Checker: " + ((Math.Round(amountOfMoney, 5).CompareTo(Math.Round(sum, 5)))));

            }
        }
    }
}
