// See https://aka.ms/new-console-template for more information
using Lab6;

Dictionary<string, SynchronizedCollection<string>> edgesOut = new() {
    ["1"] = new SynchronizedCollection<string> { "2" },
    ["2"] = new SynchronizedCollection<string> { "3"},
    ["3"] = new SynchronizedCollection<string> { "4"},
    ["4"] = new SynchronizedCollection<string> { "5"},
    ["5"] = new SynchronizedCollection<string> { "1"}

    };
Dictionary<string, SynchronizedCollection<string>> edgesIn = new()
{
    ["1"] = new SynchronizedCollection<string> { "5" },
    ["2"] = new SynchronizedCollection<string> { "1" },
    ["3"] = new SynchronizedCollection<string> { "2" },
    ["4"] = new SynchronizedCollection<string> { "3" },
    ["5"] = new SynchronizedCollection<string> { "4" }

};
DirectedGraph directedGraph = new DirectedGraph(5, edgesIn, edgesOut);
Hamiltonian hamiltonian = new Hamiltonian(directedGraph);
hamiltonian.FindHamiltonianCycle();
Console.WriteLine(hamiltonian.printCycle());
