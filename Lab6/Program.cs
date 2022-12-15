// See https://aka.ms/new-console-template for more information
using Lab6;
//TODO: Manage bter sync between tasks
Dictionary<string, SynchronizedCollection<string>> edges = new() {
    ["1"] = new SynchronizedCollection<string> { "2" },
    ["2"] = new SynchronizedCollection<string> { "3"},
    ["3"] = new SynchronizedCollection<string> { "4"},
    ["4"] = new SynchronizedCollection<string> { "5"},
    ["5"] = new SynchronizedCollection<string> { "1"}

    };
DirectedGraph directedGraph = new DirectedGraph(5, edges);
Hamiltonian hamiltonian = new Hamiltonian(directedGraph,"1");
//hamiltonian.FindHamiltonianCycleParallel();
for(int i = 0; i < 5; i++)
{
    if (i > 1)
    {
        hamiltonian.AllowPrint = true;

    }
    hamiltonian.FindHamiltonianCycle();
    hamiltonian.FindHamiltonianCycleParallel();
}
Console.WriteLine(hamiltonian.PrintCycle());
