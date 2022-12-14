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
        private readonly Dictionary<string, SynchronizedCollection<string>> edgesIn = new();
        private readonly Dictionary<string, SynchronizedCollection<string>> edgesOut = new();

        public Dictionary<string, SynchronizedCollection<string>> EdgesIn { get { return edgesIn; } }
        public Dictionary<string, SynchronizedCollection<string>> EdgesOut { get { return edgesOut; } }
        public int Size { get { return size; } }    

        public DirectedGraph(int size, Dictionary<string, SynchronizedCollection<string>> edgesIn, Dictionary<string, SynchronizedCollection<string>> edgesOut)
        {
            this.size = size;
            this.edgesIn = edgesIn;
            this.edgesOut = edgesOut;
        }

        public bool CheckIfEdgeInExists(string v1, string v2)
        {
            if (edgesIn.ContainsKey(v1))
            {
                if (edgesIn[v1].Contains(v2)) return true;
            }
            return false;
        }
        public bool CheckIfEdgeOutExists(string v1, string v2)
        {
            if (edgesOut.ContainsKey(v1))
            {
                if (edgesOut[v1].Contains(v2)) return true;
            }
            return false;
        }

        public SynchronizedCollection<string> GetEdgesInForVertex(string v1)
        {
            if(edgesIn.ContainsKey(v1))
            {
                return edgesIn[v1];
            }
            return new SynchronizedCollection<string>();
        }

        public SynchronizedCollection<string> GetEdgesOutForVertex(string v1)
        {
            if (edgesIn.ContainsKey(v1))
            {
                return edgesOut[v1];
            }
            return new SynchronizedCollection<string>();
        }

        
    }


}
