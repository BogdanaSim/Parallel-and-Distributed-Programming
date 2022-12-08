using System.Diagnostics;

namespace Lab5
{
    public class Multiply
    {
        public Polynomial Polynomial1 { get; set; }
        public Polynomial Polynomial2 { get; set; }
        public Polynomial Result { get; set; }

        public Dictionary<int, List<(int, int)>> validPairs = new();

        public bool AllowPrint { get; set; } = false;

        public Multiply(Polynomial polynomial1, Polynomial polynomial2)
        {
            this.Polynomial1 = polynomial1;
            this.Polynomial2 = polynomial2;
            SynchronizedCollection<float> resultList = new();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(0);
                validPairs.Add(i, new List<(int, int)>());
                for (int k = 0; k < m; k++)
                    for (int q = 0; q < n; q++)
                    {

                        if (k + q == i)
                        {
                            validPairs[i].Add((k, q));
                        }
                    }
            }
            this.Result = new Polynomial(resultList);

  
        }

        public void ResetResult()
        {
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            this.Result.Coefficients = GenerateZeroPolynomial(m + n - 1);
        }

        public void MultiplyRegularSequential()
        {
            this.ResetResult();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            SynchronizedCollection<float> resultList = this.Result.Coefficients;
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    resultList[i + j] += this.Polynomial1.Coefficients[i] * this.Polynomial2.Coefficients[j];
                }
            stopwatch1.Stop();
            if (AllowPrint) //used to avoid the results for the first execution of this function
            {
                Console.WriteLine("Elapsed time for MultiplyRegularSequential: " + stopwatch1.Elapsed.ToString());
                Console.WriteLine("Result for MultiplyRegularSequential: " + Result.ToString());
            }
        }

        public void MultiplyRegularParallel(int noTasks)
        {
            this.ResetResult();
            
            int noCoefficients = this.Result.GetDegreePolynomial() / noTasks;
            int extra = this.Result.GetDegreePolynomial() % noTasks;
            List<Task> tasks = new();
            int i0 = 0, j0 = 0;
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            for (int i = 0; i < noTasks; i++)
            {
                j0 = i0 + noCoefficients;
                if (i == noTasks - 1) //give the last task to compute the remaining coefficients
                {
                    j0 += extra;
                }
                int copyI = i0, copyJ = j0; //the degree of the coefficients which the task will compute
                tasks.Add(Task.Run(() => MultiplyTaskRegularParallel(copyI, copyJ)));
                i0 = j0;
            }
            Task.WhenAll(tasks).Wait();
            stopwatch1.Stop();
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for MultiplyRegularParallel: " + stopwatch1.Elapsed.ToString());
                Console.WriteLine("Result for MultiplyRegularParallel: " + Result.ToString());
            }
        }

        public void MultiplyTaskRegularParallel(int i0, int j0)
        {
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            SynchronizedCollection<float> resultList = this.Result.Coefficients;
            for (int k = i0; k < j0; k++)
                //for (int i = 0; i < m; i++)
                //{
                    //int j = k - i;
                    //if (j >= 0 && j < n) //check if the sum of the indexes is a valid degree for the result polynomial
                    //{
                    //    resultList[i + j] += this.Polynomial1.Coefficients[i] * this.Polynomial2.Coefficients[j];
                    //}
                    foreach (var pair in validPairs[k])
                    {
                        int p0 = pair.Item1, p1 = pair.Item2;
                        resultList[p0 + p1] += this.Polynomial1.Coefficients[p0] * this.Polynomial2.Coefficients[p1];

                    }
               // }
        }

        public void MultiplyKaratsubaSequential()
        {
            //used this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            SynchronizedCollection<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            SynchronizedCollection<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            SynchronizedCollection<float> resultList = this.Result.Coefficients;
            resultList.Clear();
            SynchronizedCollection<float> resultRecursive = MultiplyKaratsubaSequentialRecursive(A, B);
            stopwatch1.Stop();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(resultRecursive[i]);
            }
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for MultiplyKaratsubaSequential: " + stopwatch1.Elapsed.ToString());
                Console.WriteLine("Result for MultiplyKaratsubaSequential: " + Result.ToString());
            }
        }

        public SynchronizedCollection<float> MultiplyKaratsubaSequentialRecursive(SynchronizedCollection<float> A, SynchronizedCollection<float> B)
        {
            SynchronizedCollection<float> result = new();
            //A = FixSize(A);
            //B = FixSize(B);
            int m = A.Count;
            int n = B.Count;
            int maxDegree = Math.Max(A.Count - 1, B.Count - 1) + 1;
            if (maxDegree == 1) // return the remaining coefficient if the degree is 1
            {

                result.Add(A[0] * B[0]);

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

            var productL = MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L);
            var productH = MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H);
            var productM = MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M);

            SynchronizedCollection<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                polynomialMiddle[i] = productM[i] - productL[i] - productH[i];
            }

            result = GenerateZeroPolynomial(A.Count + B.Count - 1);
            int N = Math.Max(A.Count, B.Count);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                result[i] += productL[i];
                result[i + N] += productH[i];
                result[i + N / 2] += polynomialMiddle[i];
            }
            return result;
        }

        public async Task MultiplyKaratsubaParallel()
        {
            //used this idea https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            SynchronizedCollection<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            SynchronizedCollection<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            SynchronizedCollection<float> resultList = this.Result.Coefficients;

            resultList.Clear();
            SynchronizedCollection<float> resultRecursive = await Task.Run(() => MultiplyKaratsubaParallelRecursive(A, B));

            stopwatch1.Stop();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(resultRecursive[i]);
            }
            if (AllowPrint)
            {
                Console.WriteLine("Elapsed time for MultiplyKaratsubaParallel: " + stopwatch1.Elapsed.ToString());
                Console.WriteLine("Result for MultiplyKaratsubaParallel: " + Result.ToString());
            }
        }

        public async Task<SynchronizedCollection<float>> MultiplyKaratsubaParallelRecursive(SynchronizedCollection<float> A, SynchronizedCollection<float> B)
        {
            SynchronizedCollection<float> result = new();
            // A = FixSize(A);
            // B = FixSize(B);
            int m = A.Count;
            int n = B.Count;
            int maxDegree = Math.Max(A.Count - 1, B.Count - 1) + 1;
            if (maxDegree == 1) // return the remaining coefficient if the degree is 1
            {

                result.Add(A[0] * B[0]);

                return result;
            }
            int middle1 = A.Count / 2;
            int middle2 = B.Count / 2;

            SynchronizedCollection<float> polynomial1L = GeneratePolynomialFromRange(A, 0, middle1 - 1); //lower half of the first polynomial
            SynchronizedCollection<float> polynomial1H = GeneratePolynomialFromRange(A, middle1, m - 1);//upper half of the first polynomial
            SynchronizedCollection<float> polynomial1S = GenerateZeroPolynomial(middle1); //will contain the sums between the lower and the higher half of the first polynomial
            SynchronizedCollection<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2 - 1); //lower half of the second polynomial
            SynchronizedCollection<float> polynomial2H = GeneratePolynomialFromRange(B, middle2, n - 1); //upper half of the second polynomial
            SynchronizedCollection<float> polynomial2S = GenerateZeroPolynomial(middle2);//will contain the sums between the lower and the higher half of the second polynomial
            for (int i = 0; i < middle1; i++)
            {
                polynomial1S[i] = polynomial1L[i] + polynomial1H[i]; //compute the sum between the halves of the first polynomial
            }
            for (int i = 0; i < middle2; i++)
            {
                polynomial2S[i] = polynomial2L[i] + polynomial2H[i];//compute the sum between the halves of the second polynomial
            }

            var productL = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L)); //get the lower half of the result recursively using the lower halves of the two polynomials
            var productH = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H)); //get the upper half of the result recursively using the lower halves of the two polynomials
            var productM = await Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1S, polynomial2S)); //get the upper half of the result recursively using the lower halves of the two polynomials

            SynchronizedCollection<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                polynomialMiddle[i] = productM[i] - productL[i] - productH[i]; //compute the middle part of the result polynomial
            }

            result = GenerateZeroPolynomial(A.Count + B.Count - 1);
            int N = Math.Max(A.Count, B.Count);
            for (int i = 0; i < (A.Count + B.Count) / 2 - 1; ++i)
            {
                result[i] += productL[i]; //add the lower part to the result polynomial
                result[i + N] += productH[i]; //add the upper part to the result polynomial
                result[i + N / 2] += polynomialMiddle[i]; //add the middle part to the result polynomial
            }
            return result;
        }

        public static SynchronizedCollection<float> GeneratePolynomialFromRange(SynchronizedCollection<float> polynomial, int start, int end)
        {
            List<float> values = polynomial.ToList().GetRange(start, end - start + 1);
            SynchronizedCollection<float> polynomialSection = new();
            for (int i = 0; i < end - start + 1; i++)
            {
                polynomialSection.Add(values[i]);
            }
            return polynomialSection;
        }

        public static SynchronizedCollection<float> GenerateZeroPolynomial(int size)
        {
            SynchronizedCollection<float> polynomial = new();
            for (int i = 0; i < size; i++)
            {
                polynomial.Add(0);
            }
            return polynomial;
        }

        public SynchronizedCollection<float> FixSize(SynchronizedCollection<float> A)
        {
            int size = A.Count;
            if (!((size != 0) && ((size & (size - 1)) == 0)))
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
            for (int i = 0; i < newSize - size; i++)
            {
                A.Add(0);
            }
            return A;
        }
    }
}