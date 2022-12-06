using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public class Multiply
    {
        public Polynomial polynomial1 { get; set; }
        public Polynomial polynomial2 { get; set; }

        public Polynomial result { get; set; }

        public Stopwatch stopwatch = new Stopwatch();
        public Multiply(Polynomial polynomial1, Polynomial polynomial2)
        {
            this.polynomial1 = polynomial1;
            this.polynomial2 = polynomial2;
            SynchronizedCollection<float> resultList = new SynchronizedCollection<float>();
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(0);
            }
            this.result = new Polynomial(resultList);
        }

        public void ResetResult()
        {
            this.result.coefficients.Clear();
            SynchronizedCollection<float> resultList = this.result.coefficients;
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(0);
            }
        }

        public void MultiplyRegularSequential()
        {
            this.ResetResult();
            this.stopwatch = Stopwatch.StartNew();
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            SynchronizedCollection<float> resultList = this.result.coefficients;
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    resultList[i + j] += this.polynomial1.coefficients[i] * this.polynomial2.coefficients[j];
                }
            stopwatch.Stop();
            Console.WriteLine("Elapsed time for MultiplyRegularSequential: " + stopwatch.ElapsedMilliseconds.ToString() +" milliseconds");
            Console.WriteLine("Result for MultiplyRegularSequential: " + result.ToString());
        }

        public void MultiplyRegularParallel(int noTasks)
        {
            this.ResetResult();
            this.stopwatch = Stopwatch.StartNew();
            int noCoefficients = this.result.GetDegreePolynomial() / noTasks;
            int extra = this.result.GetDegreePolynomial() % noTasks;
            List<Task> tasks = new List<Task>();
            int i0 = 0, j0 = 0;
            for (int i = 0; i < noTasks; i++)
            {
                j0 = i0 + noCoefficients;
                if (i == noTasks - 1)
                {
                    j0 += extra;
                   
                }
                int copyI = i0, copyJ = j0;
                tasks.Add(Task.Run(() =>
                MultiplyTaskRegularParallel(copyI, copyJ)));
                i0 = j0;
            }
            Task.WhenAll(tasks).Wait();
            stopwatch.Stop();
            Console.WriteLine("Elapsed time for MultiplyRegularParallel: " + stopwatch.ElapsedMilliseconds.ToString() + " milliseconds");
            Console.WriteLine("Result for MultiplyRegularParallel: "+result.ToString());
        }

        public void MultiplyTaskRegularParallel(int i0, int j0)
        {
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            SynchronizedCollection<float> resultList = this.result.coefficients;

            for (int k = i0; k < j0; k++)
                for (int i = 0; i < m; i++)
                {
                    int j = k - i;
                    if (j >= 0 && j < n)
                    {
                        resultList[i + j] += this.polynomial1.coefficients[i] * this.polynomial2.coefficients[j];
                    }
                }
        }

        public List<Tuple<int, int>> GetValidPositions(int i0, int j0, int m, int n)
        {
            List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
            for (int k = i0; k < j0; k++)
                for (int i = 0; i < m; i++)
                {
                    int j = k-i;
                    if(j>0 && j< n)
                    {
                        positions.Add(new Tuple<int, int>(i, j));   
                    }
                }
            return positions;

        }
    }
}
