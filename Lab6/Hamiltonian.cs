using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Hamiltonian
    {
        private readonly DirectedGraph DirectedGraph;

        private readonly string StartingVertex;
        public DirectedGraph Graph { get { return DirectedGraph; } }
        public SynchronizedCollection<string> visited = new();
        public SynchronizedCollection<string> HamiltonianCycle = new();

        public Hamiltonian(DirectedGraph directedGraph)
        {
            
            DirectedGraph = directedGraph;
            StartingVertex = Graph.EdgesOut.First().Key;
        }

        public SynchronizedCollection<string> FindHamiltonianCycle()
        {


            GoToVertex(StartingVertex);
            return HamiltonianCycle;
        }

        public void GoToVertex(string vertex)
        {
            visited.Add(vertex);
            if(visited.Count!=Graph.Size)
            {
                SynchronizedCollection<string> EdgesOutVertex = Graph.GetEdgesOutForVertex(vertex);

             
                foreach(string v in EdgesOutVertex)
                {
                    if(!visited.Contains(v)) {
                        GoToVertex(v);
                    }
                }

            }
            else
            {
                if (Graph.CheckIfEdgeInExists(StartingVertex, vertex))
                {
                    foreach(string v in visited)
                    {
                        HamiltonianCycle.Add(v);
                    }
                    HamiltonianCycle.Add(StartingVertex);
                }
            }
        }

        public string printCycle()
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
