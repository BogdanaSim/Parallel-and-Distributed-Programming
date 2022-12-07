using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
            //this.stopwatch = Stopwatch.StartNew();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            SynchronizedCollection<float> resultList = this.result.coefficients;
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    resultList[i + j] += this.polynomial1.coefficients[i] * this.polynomial2.coefficients[j];
                }
            //stopwatch.Stop();
            stopwatch1.Stop();
            Console.WriteLine("Elapsed time for MultiplyRegularSequential: " + stopwatch1.ElapsedMilliseconds.ToString() +" milliseconds");
            Console.WriteLine("Result for MultiplyRegularSequential: " + result.ToString());
        }

        public void MultiplyRegularParallel(int noTasks)
        {
            this.ResetResult();
            //this.stopwatch = Stopwatch.StartNew();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
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
            //stopwatch.Stop();
            stopwatch1.Stop();
            Console.WriteLine("Elapsed time for MultiplyRegularParallel: " + stopwatch1.ElapsedMilliseconds.ToString() + " milliseconds");
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

        public void MultiplyKaratsubaSequential()
        {
            //TODO: check this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            //this.ResetResult();
            //this.stopwatch = Stopwatch.StartNew();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            SynchronizedCollection<float> resultList = this.result.coefficients;
            
            resultList.Clear();
            SynchronizedCollection<float> resultRecursive = MultiplyKaratsubaSequentialRecursive(this.polynomial1.coefficients, this.polynomial2.coefficients);
            //stopwatch.Stop();
            stopwatch1.Stop();
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(resultRecursive[i]);
            }
            
            Console.WriteLine("Elapsed time for MultiplyKaratsubaSequential: " + stopwatch1.ElapsedMilliseconds.ToString() + " milliseconds");
            Console.WriteLine("Result for MultiplyKaratsubaSequential: " + result.ToString());
            


        }
        public SynchronizedCollection<float> MultiplyKaratsubaSequentialRecursive(SynchronizedCollection<float> A, SynchronizedCollection<float> B)
        {
           
            SynchronizedCollection<float> result = new SynchronizedCollection<float>();
            A = FixSize(A);
            B = FixSize(B);
            int m = A.Count;
            int n = B.Count;
            int maxDegree = Math.Max(A.Count, B.Count);
            if (maxDegree == 1)
            {
                if (A.Count == 0)
                    result.Add(B[0]);
                else if (B.Count == 0)
                    result.Add(A[0]);
                else
                {
                    result.Add(A[0] * B[0]);
                }
                return result;
            }
            int middle1 = A.Count/ 2;
            int middle2 = B.Count / 2;

            SynchronizedCollection<float> polynomial1L = GeneratePolynomialFromRange(A,0, middle1-1);
            SynchronizedCollection<float> polynomial1H = GeneratePolynomialFromRange(A, middle1,m-1 );
            SynchronizedCollection<float> polynomial1M = GenerateZeroPolynomial(middle1);
            SynchronizedCollection<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2-1);
            SynchronizedCollection<float> polynomial2H=GeneratePolynomialFromRange(B,middle2, n-1);
            SynchronizedCollection<float> polynomial2M = GenerateZeroPolynomial(middle2);
            for(int i =0; i < middle1; i++)
            {
                polynomial1M[i] = polynomial1L[i] + polynomial1H[i];
            }
            for (int i = 0; i < middle2; i++)
            {
                polynomial2M[i] = polynomial2L[i] + polynomial2H[i];
            }

            var productL = MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L);
            var productH = MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H);
            var productM = MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M);

            SynchronizedCollection<float> polynomialMiddle = GenerateZeroPolynomial((A.Count+B.Count)/2);
            for (int i = 0; i < (A.Count + B.Count) / 2-1; ++i)
            {
                polynomialMiddle[i] = productM[i] - productL[i] - productH[i];
            }
           
            result = GenerateZeroPolynomial(A.Count + B.Count-1);
            for (int i = 0; i < (A.Count + B.Count) / 2-1; ++i)
            {
                result[i] += productL[i];
                result[i + (A.Count + B.Count) / 2] += productH[i];
                result[i + (A.Count + B.Count) / 4] += polynomialMiddle[i];
            }
            return result;
        }
        public async Task MultiplyKaratsubaParallel()
        {
            //TODO: check this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            //this.ResetResult();
            //this.stopwatch = Stopwatch.StartNew();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            SynchronizedCollection<float> resultList = this.result.coefficients;

            resultList.Clear();
            SynchronizedCollection<float> resultRecursive = await Task.Run(() => MultiplyKaratsubaParallelRecursive(this.polynomial1.coefficients, this.polynomial2.coefficients));

            //stopwatch.Stop();
            stopwatch1.Stop();
            int m = this.polynomial1.GetDegreePolynomial();
            int n = this.polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(resultRecursive[i]);
            }

            Console.WriteLine("Elapsed time for MultiplyKaratsubaParallel: " + stopwatch1.ElapsedMilliseconds.ToString() + " milliseconds");
            Console.WriteLine("Result for MultiplyKaratsubaParallel: " + result.ToString());



        }
        public async Task<SynchronizedCollection<float>> MultiplyKaratsubaParallelRecursive(SynchronizedCollection<float> A, SynchronizedCollection<float> B)
        {

            SynchronizedCollection<float> result = new SynchronizedCollection<float>();
            A = FixSize(A);
            B = FixSize(B);
            int m = A.Count;
            int n = B.Count;
            int maxDegree = Math.Max(A.Count, B.Count);
            if (maxDegree == 1)
            {
                if (A.Count == 0)
                    result.Add(B[0]);
                else if (B.Count == 0)
                    result.Add(A[0]);
                else
                {
                    result.Add(A[0] * B[0]);
                }
                return result;
            }
            int middle1 = A.Count / 2;
            int middle2 = B.Count / 2;

            SynchronizedCollection<float> polynomial1L = GeneratePolynomialFromRange(A, 0, middle1 - 1);
            SynchronizedCollection<float> polynomial1H = GeneratePolynomialFromRange(A, middle1, m - 1);
            SynchronizedCollection<float> polynomial1M = GenerateZeroPolynomial(middle1);
            SynchronizedCollection<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2 - 1);
            SynchronizedCollection<float> polynomial2H = GeneratePolynomialFromRange(B, middle2, n - 1);
            SynchronizedCollection<float> polynomial2M = GenerateZeroPolynomial(middle2);
            for (int i = 0; i < middle1; i++)
            {
                polynomial1M[i] = polynomial1L[i] + polynomial1H[i];
            }
            for (int i = 0; i < middle2; i++)
            {
                polynomial2M[i] = polynomial2L[i] + polynomial2H[i];
            }


            //var productL = MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L);
            //var productH = MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H);
            //var productM = MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M);
          //  var tcs = new TaskCompletionSource<SynchronizedCollection<float>>();
            //Task productLRes = Task.Run(() =>
            //     MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L));
            var productL = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L));
            var productH = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H));
            var productM = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M));


            //var productH = MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H);
           // var productM = MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M);

            SynchronizedCollection<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                polynomialMiddle[i] = productM[i] - productL[i] - productH[i];
            }

            result = GenerateZeroPolynomial(A.Count + B.Count - 1);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                result[i] += productL[i];
                result[i + (A.Count + B.Count) / 2] += productH[i];
                result[i + (A.Count + B.Count) / 4] += polynomialMiddle[i];
            }
            return result;
        }

        public SynchronizedCollection<float> GeneratePolynomialFromRange(SynchronizedCollection<float> polynomial,int start, int end)
        {
            List<float> values = polynomial.ToList().GetRange(start, end-start+1);
            SynchronizedCollection<float> polynomialSection = new SynchronizedCollection<float>();
            for(int i = 0; i < end - start+1 ; i++)
            {
                polynomialSection.Add(values[i]);
            }
            return polynomialSection;
        }
        public SynchronizedCollection<float> GenerateZeroPolynomial(int size)
        {
            SynchronizedCollection<float> polynomial = new SynchronizedCollection<float>();
            for (int i = 0; i < size; i++)
            {
                polynomial.Add(0);
            }
            return polynomial;
        }


        public SynchronizedCollection<float> FixSize(SynchronizedCollection<float> A)
        {
            int size = A.Count();
            if(!((size != 0) && ((size & (size - 1)) == 0)))
            {

            }
            int newSize = size;
            newSize--;
            newSize |= newSize >> 1;   // Divide by 2^k for consecutive doublings of k up to 32,
            newSize |= newSize >> 2;   // and then or the results.
            newSize |= newSize >> 4;
            newSize |= newSize >> 8;
            newSize |= newSize >> 16;
            newSize++;
            for(int i = 0; i < newSize - size; i++)
            {
                A.Add(0);
            }
            return A;

        }
    }
}
