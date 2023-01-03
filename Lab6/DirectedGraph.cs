using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class DirectedGraph
    {
        private readonly int size;
        private readonly Dictionary<string, SynchronizedCollection<string>> edges = new();

        public Dictionary<string, SynchronizedCollection<string>> Edges { get { return edges; } }
        public int Size { get { return size; } }

        public DirectedGraph(int size, Dictionary<string, SynchronizedCollection<string>> edges)
        {
            this.size = size;
            this.edges = edges;
        }
        public DirectedGraph ShallowCopy()
        {
            return (DirectedGraph)this.MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            foreach (string v in edges.Keys)
            {
                stringBuilder.Append(v + " -> { ");
                foreach (string value in edges[v])
                {
                    stringBuilder.Append(value + ", ");
                }
                stringBuilder.Length -= 2;
                stringBuilder.Append(" }\n");
            }

            return stringBuilder.ToString();
        }

        public bool CheckIfEdgeExists(string v1, string v2)
        {
            if (edges.ContainsKey(v1))
            {
                if (edges[v1].Contains(v2)) return true;
            }
            return false;
        }

        public SynchronizedCollection<string> GetEdgesForVertex(string v1)
        {
            if (edges.ContainsKey(v1))
            {
                return edges[v1];
            }
            return new SynchronizedCollection<string>();
        }

        public static DirectedGraph CreateHamiltonianGraph(int size)
        {
            Dictionary<string, SynchronizedCollection<string>> edges = new();
            for (int i = 0; i < size - 1; i++)
            {

                edges[i.ToString()] = new SynchronizedCollection<string>() { (i + 1).ToString() };


            }
            edges[(size - 1).ToString()] = new SynchronizedCollection<string>() { "0" };
            Random random = new Random();
            for (int i = 0; i < (size-1)*(size-1)/2; i++)
            {
                int node1 = random.Next(size - 1);
                int node2 = random.Next(size - 1);
                while (edges[node1.ToString()].Contains(node2.ToString()) || node2 == node1)
                {
                    node2 = random.Next(size - 1);
                }
                if (node2 < (size - 1) * (size - 1) / 4)
                    edges[node1.ToString()].Insert(0, node2.ToString());
                else
                {
                    edges[node1.ToString()].Add(node2.ToString());
                }
                //edges[node1.ToString()].Append( node2.ToString());

            }
            return new DirectedGraph(size, edges);



        }
        public static DirectedGraph CreateRandomGraph(int size, int NoEdges)
        {
            Dictionary<string, SynchronizedCollection<string>> edges = new();
            for (int i = 0; i < size - 1; i++)
            {

                edges[i.ToString()] = new SynchronizedCollection<string>() { };


            }

            Random random = new Random();
            for (int i = 0; i < NoEdges; i++)
            {
                int node1 = random.Next(size - 1);
                int node2 = random.Next(size - 1);
                while (edges[node1.ToString()].Contains(node2.ToString()) || node2 == node1)
                {
                    node1 = random.Next(size - 1);
                    node2 = random.Next(size - 1);
                }
                edges[node1.ToString()].Add(node2.ToString());

            }
            return new DirectedGraph(size, edges);



        }



    }
}
