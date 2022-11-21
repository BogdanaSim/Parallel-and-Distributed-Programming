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
        public Dictionary<int, SynchronizedCollection<int>> elementsTasks = new Dictionary<int, SynchronizedCollection<int>>();

        public MultiplyMatrices(Matrix<double> m1, Matrix<double> m2, int noTasks)
        {
            this.m1 = m1;
            this.m2 = m2;
            NoTasks = noTasks;
            result = new Matrix<double>(m1.n, m2.m);
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

        public void GetResult1()
        {
            int pos = 0;
            if (m1.n * m2.m / NoTasks == 0)
            {
                NoTasks = m1.n * m2.m % NoTasks;
            }
            int k = 0, k1 = 0;
            if (m1.n * m2.m % NoTasks != 0)
            {
                k = m1.n * m2.m / NoTasks;
                k1 = m1.n * m2.m / NoTasks + m1.n * m2.m % NoTasks;
            }
            else
            {
                k = k1 = m1.n * m2.m / NoTasks;
            }

            int i0 = 0, j0 = 0;
            int[] result, final = { i0, j0 };
            for (int i = 0; i < NoTasks; i++)
            {
                if (i == NoTasks - 1)
                {
                    pos += k1;
                }
                else
                {
                    pos += k;
                }
                result = GetPostion(pos-1 );

                var z = new int[result.Length + final.Length];
                result.CopyTo(z, 0);
                final.CopyTo(z, result.Length);
                elementsTasks.Add(i, new SynchronizedCollection<int>() { result[0], result[1], final[0], final[1] });
                i0 = result[0];
                j0 = result[1];

                if (result[1] < m2.m - 1)
                {
                    j0++;
                }
                else if (result[0] < m1.n - 1)
                {
                    j0 = 0;
                    i0++;
                }

                final = new int[] { i0, j0 };
            }
            //Console.WriteLine(elementsTasks.ToString());
            for (int i = 0; i < NoTasks; i++)
            {
                int a = i;

                //ThreadStart thread = () => this.Multiply(a);
                ThreadStart thread = () => this.Multiply2(elementsTasks[a][0], elementsTasks[a][1], elementsTasks[a][2], elementsTasks[a][3], a);
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

        public void Multiply2(int i1, int j1, int i0, int j0, int taskNo)
        {

            int i = i0, j = j0;
            while (i!=i1 || j!=j1)
            {
                this.result[i, j] = ComputeElem1(i, j);
                Console.WriteLine("Task " + taskNo + " computed " + "[" + i + "," + j + "] = " + this.result[i, j]);
                if (j < m2.m - 1)
                {
                    j++;
                }
                else if (i < m1.n - 1)
                {
                    j = 0;
                    i++;
                }
               
         
            }
            this.result[i, j] = ComputeElem1(i, j);
            Console.WriteLine("Task " + taskNo + " computed " + "[" + i + "," + j + "] = " + this.result[i, j]);


        }

        public int[] GetPostion(int n)
        {
            int rowIndex = n / m1.n;
            int columnIndex = n % m1.n;
            int[] result = new int[2] { rowIndex, columnIndex };
            return result;
        }

        public int[] GetPositionAfterElements(int i0, int j0, int n)
        {
            int k = 0, i = 0, j = 0;
            int[] result = new int[2];
            for (i = i0; i < m1.n; i++)
                for (j = j0; j < m2.m; j++)
                {
                    if (k == n - 1)
                    {
                        result[0] = i;
                        result[1] = j;
                        return result;
                    }
                    k++;
                }

            result[0] = m1.n - 1;
            result[1] = m2.m - 1;
            return result;
        }

        public void Multiply(int column)
        {
            //for(int i =0;i<NoTasks;i++)
            //{
            int columnElem = column;
            int k = 0;
            if (columnElem < m1.n * m2.m)
            {
                if (columnElem > m2.m)
                {
                    columnElem = columnElem - m2.m;
                    k = m1.n - 1;
                }
                for (int j = k; j < m1.n; j++)
                {
                    while (columnElem < m2.m)
                    {
                        this.result[j, columnElem] = this.ComputeElem(j, columnElem);
                        Console.WriteLine("Task " + column + " computed " + "[" + j + "," + columnElem + "] = " + this.result[j, columnElem]);
                        columnElem += NoTasks;
                    }
                    columnElem = columnElem - m2.m;
                }
            }
            // }
            // return result;
        }

        public double ComputeElem(int i, int j)
        {
            double sum = 0;
            for (int k = 0; k < m1.m; k++)
                sum += m1[i, k] * m2[k, j];

            return sum;
        }

        public double ComputeElem1(int i, int j)
        {
            double sum = 0;
            for (int k = 0; k < m1.m; k++)
                sum += m1[i, k] * m2[k, j];

            return sum;
        }
    }
}