using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Hamiltonian
    {
       
        private readonly DirectedGraph DirectedGraph;
        private readonly string StartingVertex;
        public bool AllowPrint = false;
        public DirectedGraph Graph { get { return DirectedGraph; } }
        public SynchronizedCollection<string> visited = new();
        public SynchronizedCollection<string> HamiltonianCycle = new();
     

        public Hamiltonian(DirectedGraph directedGraph, string startingVertex)
        {

            DirectedGraph = directedGraph;
            StartingVertex = startingVertex;
        }

        public Hamiltonian(DirectedGraph directedGraph,string startingVertex, SynchronizedCollection<string> visited, SynchronizedCollection<string> hamiltonianCycle)
        {
            DirectedGraph = directedGraph;
            StartingVertex = startingVertex;
            this.visited = visited;
            HamiltonianCycle = hamiltonianCycle;
        }

        public SynchronizedCollection<string> FindHamiltonianCycle()
        {
            //HamiltonianCycle.Clear();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            GoToVertex(StartingVertex);
            stopwatch1.Stop();
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for FindHamiltonianCycle: " + stopwatch1.Elapsed.ToString());
              //  Console.WriteLine("Result for FindHamiltonianCycle: " + PrintCycle());
            }
            return HamiltonianCycle;
        }

        public void ResetResult()
        {
            HamiltonianCycle.Clear();
            visited.Clear();
        }

        public async Task<(SynchronizedCollection<string>,DirectedGraph)> FindHamiltonianCycleParallel()
        {
            //HamiltonianCycle.Clear();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            await GoToVertexParallel(StartingVertex,0);
            stopwatch1.Stop();
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for FindHamiltonianCycleParallel: " + stopwatch1.Elapsed.ToString());
               // Console.WriteLine("Result for FindHamiltonianCycleParallel: "+PrintCycle());
            }
      
            return (HamiltonianCycle,Graph);
        }

        public Hamiltonian GenerateHamiltonian()
        {
            SynchronizedCollection<string> vistitedNew = new SynchronizedCollection<string>();
            foreach(string v in visited)
            {
                vistitedNew.Add(v);
            }
            return new Hamiltonian(Graph, StartingVertex, vistitedNew, HamiltonianCycle);
        }

        public void GoToVertex(string vertex)
        {
            if (HamiltonianCycle.Count > 0)
            {
                return;
            }
            visited.Add(vertex);
            if (visited.Count != Graph.Size)
            {
                SynchronizedCollection<string> EdgesOutVertex = Graph.GetEdgesForVertex(vertex);


                foreach (string v in EdgesOutVertex)
                {
                    if (!visited.Contains(v))
                    {
                        GoToVertex(v);
                    }
                }

            }
            else
            {
                if (Graph.CheckIfEdgeExists(vertex, StartingVertex))
                {
                    foreach (string v in visited)
                    {
                        HamiltonianCycle.Add(v);
                    }
                    HamiltonianCycle.Add(StartingVertex);
                }
            }
            visited.Remove(vertex);
        }

        public async Task GoToVertexParallel(string vertex,int steps)
        {
            lock (HamiltonianCycle)
            {

                if (HamiltonianCycle.Count > 0)
                {
                    return;
                }

            }
            if (steps < 2)
            {
                visited.Add(vertex);
                if (visited.Count != Graph.Size)
                {
                    SynchronizedCollection<string> EdgesOutVertex = Graph.GetEdgesForVertex(vertex);

                    List<Task> tasks = new List<Task>();
                    foreach (string v in EdgesOutVertex)
                    {
                   

                        if (!visited.Contains(v))
                        {

                            tasks.Add(Task.Run(() => this.GenerateHamiltonian().GoToVertexParallel(v,steps+1)));

                        }
                    }
                    await Task.WhenAll(tasks);

                }
                else
                {
                    if (Graph.CheckIfEdgeExists(vertex, StartingVertex))
                    {

                        lock (HamiltonianCycle)
                        {

                            foreach (string v in visited)
                            {
                                HamiltonianCycle.Add(v);
                            }
                            HamiltonianCycle.Add(StartingVertex);
                        }

                    }

                }
            }
            else
            {
                this.GenerateHamiltonian().GoToVertexWithLock(vertex);
            }
          //  visited.Remove(vertex);
        }
        public void GoToVertexWithLock(string vertex)
        {
            lock (HamiltonianCycle)
            {
                if (HamiltonianCycle.Count > 0)
                {
                    return;
                }
            }
            visited.Add(vertex);
            if (visited.Count != Graph.Size)
            {
                SynchronizedCollection<string> EdgesOutVertex = Graph.GetEdgesForVertex(vertex);


                foreach (string v in EdgesOutVertex)
                {
                    if (!visited.Contains(v))
                    {
                        GoToVertexWithLock(v);
                    }
                }

            }
            else
            {
                if (Graph.CheckIfEdgeExists(vertex,StartingVertex))
                {
                    lock (HamiltonianCycle)
                    {
                        foreach (string v in visited)
                        {
                            HamiltonianCycle.Add(v);
                        }
                        HamiltonianCycle.Add(StartingVertex);
                    }
                }
            }
            visited.Remove(vertex);
        }
        public string PrintCycle()
        {
            StringBuilder stringBuilder = new StringBuilder(" ");
            if (HamiltonianCycle.Count > 0)
            {
               // stringBuilder.Append("This is the Hamiltonian Cycle: ");
                foreach (string v in HamiltonianCycle)
                {
                    stringBuilder.Append(v+" ");
                }
                return stringBuilder.ToString();
            }
            return "There is no Hamiltonian Cycle!";
        }
    }
}
