// See https://aka.ms/new-console-template for more information

using System.Threading.Tasks;

namespace Lab6
{
    public class Program
    {

        public static async Task Main()
        {

            Dictionary<string, SynchronizedCollection<string>> edges = new()
            {
                ["1"] = new SynchronizedCollection<string> { "3", "2", "5" },
                ["2"] = new SynchronizedCollection<string> { "4", "3" },
                ["3"] = new SynchronizedCollection<string> { "6", "1", "4" },
                ["4"] = new SynchronizedCollection<string> { "5", "6" },
                ["5"] = new SynchronizedCollection<string> { "3", "2", "1", "6" },
                ["6"] = new SynchronizedCollection<string> { "1", "5", "4" }

            };
            DirectedGraph directedGraph = new DirectedGraph(5, edges);
            Hamiltonian hamiltonian = new Hamiltonian(DirectedGraph.CreateHamiltonianGraph(30), "0");


            for (int i = 0; i < 15; i++)
            {
                if (i > 3)
                {
                    hamiltonian.AllowPrint = true;

                }
                Console.WriteLine("\n---------------------------------------------");
                hamiltonian.ResetResult();
                hamiltonian.FindHamiltonianCycle();
                hamiltonian.ResetResult();
                await hamiltonian.FindHamiltonianCycleParallel();
                
            }
            //Console.WriteLine(hamiltonian.PrintCycle());
            //Console.WriteLine(DirectedGraph.CreateHamiltonianGraph(10).ToString());
        }
    }
}