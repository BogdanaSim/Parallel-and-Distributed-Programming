using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class MultiplyMatrices
    {
        public Matrix<double> m1;
        public Matrix<double> m2;
        public Matrix<double> result { get; }
        public int NoTasks;
        public List<Thread> threads = new List<Thread>();
        public List<int> columns = new List<int>();

        public MultiplyMatrices(Matrix<double> m1, Matrix<double> m2, int noTasks)
        {
            this.m1 = m1;
            this.m2 = m2;
            NoTasks = noTasks;
            result = new Matrix<double>(m1.n,m2.m);


        }

        public void GetResult() 
        {
            for (int i = 0; i < NoTasks; i++)
            {
                int a = i;
                ThreadStart thread = () => this.Multiply(a);
                Thread thread1 = new Thread(thread);
                thread1.Start();
                threads.Add(thread1);

            }


            //foreach (Thread thread in threads)
            //{
            //    thread.Start();
            //}

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
            
            
        }

        public void Multiply(int column)
        {
            //for(int i =0;i<NoTasks;i++)
            //{
                
                int columnElem = column;
                if(columnElem > m2.m)
            {
                columnElem = columnElem - m2.m;
            }
                for(int j = 0; j < m1.n; j++)
                {
                while (columnElem < m2.m)
                {

                    this.result[j, columnElem] = this.ComputeElem(j, columnElem);
                    Console.WriteLine("[" + j + "," + columnElem + "] = " + this.result[j, columnElem]);
                    columnElem += NoTasks;
                    }
                    columnElem = columnElem - m2.m;
                }
           // }
           // return result;
        }

        public double ComputeElem(int i, int j)
        {
            double sum = 0;
            for (int k = 0; k < m1.m; k++)
                sum += m1[i,k] * m2[k,j];
            return sum;
        }




    }
}
