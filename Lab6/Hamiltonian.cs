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
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public DirectedGraph Graph { get { return DirectedGraph; } }
        public SynchronizedCollection<string> visited = new();
        public SynchronizedCollection<string> HamiltonianCycle = new();

        public Hamiltonian(DirectedGraph directedGraph, string startingVertex)
        {

            DirectedGraph = directedGraph;
            StartingVertex = startingVertex;
        }

        public SynchronizedCollection<string> FindHamiltonianCycle()
        {
            HamiltonianCycle.Clear();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            GoToVertex(StartingVertex);
            stopwatch1.Stop();
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for FindHamiltonianCycle: " + stopwatch1.Elapsed.ToString());
            }
            return HamiltonianCycle;
        }

        public async Task<SynchronizedCollection<string>> FindHamiltonianCycleParallel()
        {
            HamiltonianCycle.Clear();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            await GoToVertexParallel(StartingVertex);
            stopwatch1.Stop();
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for FindHamiltonianCycleParallel: " + stopwatch1.Elapsed.ToString());
            }
            return HamiltonianCycle;
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

        public async Task GoToVertexParallel(string vertex)
        {
            //lock (HamiltonianCycle)
            //{
          //  await semaphoreSlim.WaitAsync();
           // semaphoreSlim.Wait();
            //try
            //{

                if (HamiltonianCycle.Count > 0)
            {
                return;
            }
            //}
            //finally
            //{
               // semaphoreSlim.Release();
           // }
        //}
        visited.Add(vertex);
            if (visited.Count != Graph.Size)
            {
                SynchronizedCollection<string> EdgesOutVertex = Graph.GetEdgesForVertex(vertex);

                List<Task> tasks = new();
                foreach (string v in EdgesOutVertex)
                {

                    if (!visited.Contains(v))
                    {
                        //await semaphoreSlim.WaitAsync();
                         tasks.Add(Task.Run( () =>  GoToVertexParallel(v))) ;
                        //try
                        //{
                        //    await semaphoreSlim.WaitAsync();
                          // Task.Run(() => GoToVertexParallel(v));
                        //}
                        //finally
                        //{
                        //    semaphoreSlim.Release();
                        //}

                        //Task res = Task.Run(()=>GoToVertexParallel(v));
                        //GoToVertex(v);
                    }
                }
                await Task.WhenAll(tasks);

            }
            else
            {
                if (Graph.CheckIfEdgeExists(vertex, StartingVertex))
                {
                    
                   //  await semaphoreSlim.WaitAsync();
                    // lock (HamiltonianCycle)
                    //{
                    //try
                    //{
                        foreach (string v in visited)
                    {
                        HamiltonianCycle.Add(v);
                    }
                    HamiltonianCycle.Add(StartingVertex);
                   // }
                    //finally
                    //{
                    //    semaphoreSlim.Release();
                    //}
                    // }
                }
            }
            visited.Remove(vertex);
        }

        public string PrintCycle()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (HamiltonianCycle.Count > 0)
            {
                stringBuilder.Append("This is the Hamiltonian Cycle: ");
                foreach (string v in HamiltonianCycle)
                {
                    stringBuilder.Append(v).Append(" ");
                }
                return stringBuilder.ToString();
            }
            return "There is no Hamiltonian Cycle!";
        }
    }
}
