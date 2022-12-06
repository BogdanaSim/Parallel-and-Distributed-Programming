using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class Matrix<T>
    {
        public Dictionary<int,Dictionary<int,T>> elements { get; set; }
        public int n { get; set; }
        public int m { get; set; }

        public Matrix(int n, int m)
        {
            this.n = n;
            this.m = m;
            elements = new Dictionary<int,Dictionary<int,T>>();
            for(int i = 0; i < n; i++)
            {
                
                elements[i] = new Dictionary<int,T>();
            }
        }

        public T this[int i, int j]
        {
            get { return elements[i][j]; }
            set { elements[i][j] = value; } 
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("[ ");
            for(int i=0;i<n;i++)
            {
                for(int j=0;j<m; j++)
                {
                    stringBuilder.Append(elements[i][j]).Append(" ");
                }
                stringBuilder.Append("]\n[ ");
            }
            stringBuilder.Length-=2;
            return stringBuilder.ToString();
            //return $"{{{nameof(elements)}={elements}, {nameof(n)}={n.ToString()}, {nameof(m)}={m.ToString()}, {
            //    nameof(this[])}={this[]}}}";
        }
    }
}
