using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;

namespace Lab7
{
    public class Multiply
    {
        public Polynomial Polynomial1 { get; set; }
        public Polynomial Polynomial2 { get; set; }
        public Polynomial Result { get; set; }

        public Dictionary<int, List<(int, int)>> validPairs = new();

        public Intracommunicator comm;

        List<int> leftProcesses = new() { 0 };

        public bool AllowPrint { get; set; } = true;
        public TimeSpan timeSpan = new(0, 0, 0, 0);
        public bool done = false;
        public Multiply(Polynomial polynomial1, Polynomial polynomial2, Intracommunicator comm)
        {
            this.Polynomial1 = polynomial1;
            this.Polynomial2 = polynomial2;
            this.comm = comm;
            List<float> resultList = new();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(0);
                //validPairs.Add(i, new List<(int, int)>());
                //for (int k = 0; k < m; k++)
                //    for (int q = 0; q < n; q++)
                //    {

                //        if (k + q == i)
                //        {
                //            validPairs[i].Add((k, q));
                //        }
                //    }
            }
            this.Result = new Polynomial(resultList);


        }

        public void ResetResult()
        {
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            this.Result.Coefficients = GenerateZeroPolynomial(m + n - 1);
            this.done = false;
        }

        public void MultiplyRegularSequential()
        {
            this.ResetResult();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            List<float> resultList = this.Result.Coefficients;
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    resultList[i + j] += this.Polynomial1.Coefficients[i] * this.Polynomial2.Coefficients[j];
                }
            stopwatch1.Stop();
            if (AllowPrint) //used to avoid the results for the first execution of this function
            {
                Console.WriteLine("Elapsed time for MultiplyRegularSequential: " + stopwatch1.Elapsed.ToString());
                //Console.WriteLine("Result for MultiplyRegularSequential: " + Result.ToString());
            }
        }

        public void MultiplyRegularParallel(int noTasks)
        {
            this.ResetResult();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int noCoefficients = (this.Result.GetDegreePolynomial() / 2) / noTasks;
            int extra = (this.Result.GetDegreePolynomial() / 2) % noTasks;
            List<Task> tasks = new();
            int i0 = 0, j0 = 0;

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
                //Console.WriteLine("Result for MultiplyRegularParallel: " + Result.ToString());
            }
        }

        public void MultiplyTaskRegularParallel(int i0, int j0)
        {
            //Stopwatch stopwatch1 = Stopwatch.StartNew();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            int M = Math.Min(n, m);
            List<float> resultList = this.Result.Coefficients;
            for (int i = i0; i < j0; i++)
                //for (int i = 0; i < m; i++)
                //{
                //int j = k - i;
                //if (j >= 0 && j < n) //check if the sum of the indexes is a valid degree for the result polynomial
                //{
                //    resultList[i + j] += this.Polynomial1.Coefficients[i] * this.Polynomial2.Coefficients[j];
                //}
                for (int j = 0; j < M; j++)
                {
                    resultList[i + j] += this.Polynomial1.Coefficients[i] * this.Polynomial2.Coefficients[j];
                }
            //foreach (var pair in validPairs[k])
            //{
            //    int p0 = pair.Item1, p1 = pair.Item2;
            //    resultList[p0 + p1] += this.Polynomial1.Coefficients[p0] * this.Polynomial2.Coefficients[p1];

            //}
            // }
            // stopwatch1.Stop();
            // timeSpan = timeSpan.Add(stopwatch1.Elapsed);
        }

        public void MultiplyKaratsubaSequential()
        {
            //used this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            List<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            List<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            List<float> resultList = this.Result.Coefficients;
            resultList.Clear();
            List<float> resultRecursive = MultiplyKaratsubaSequentialRecursive(A, B);
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
                // Console.WriteLine("Result for MultiplyKaratsubaSequential: " + Result.ToString());
            }
        }

        public List<float> MultiplyKaratsubaSequentialRecursive(List<float> A, List<float> B)
        {
            List<float> result = new();
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

            List<float> polynomial1L = GeneratePolynomialFromRange(A, 0, middle1 - 1);
            List<float> polynomial1H = GeneratePolynomialFromRange(A, middle1, m - 1);
            List<float> polynomial1M = GenerateZeroPolynomial(middle1);
            List<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2 - 1);
            List<float> polynomial2H = GeneratePolynomialFromRange(B, middle2, n - 1);
            List<float> polynomial2M = GenerateZeroPolynomial(middle2);
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

            List<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
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
            List<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            List<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            List<float> resultList = this.Result.Coefficients;

            resultList.Clear();
            List<float> resultRecursive = await Task.Run(() => MultiplyKaratsubaParallelRecursive(A, B));

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
                //Console.WriteLine("Result for MultiplyKaratsubaParallel: " + Result.ToString());
            }
        }

        public async Task<List<float>> MultiplyKaratsubaParallelRecursive(List<float> A, List<float> B)
        {
            List<float> result = new();
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

            List<float> polynomial1L = GeneratePolynomialFromRange(A, 0, middle1 - 1); //lower half of the first polynomial
            List<float> polynomial1H = GeneratePolynomialFromRange(A, middle1, m - 1);//upper half of the first polynomial
            List<float> polynomial1S = GenerateZeroPolynomial(middle1); //will contain the sums between the lower and the higher half of the first polynomial
            List<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2 - 1); //lower half of the second polynomial
            List<float> polynomial2H = GeneratePolynomialFromRange(B, middle2, n - 1); //upper half of the second polynomial
            List<float> polynomial2S = GenerateZeroPolynomial(middle2);//will contain the sums between the lower and the higher half of the second polynomial
            for (int i = 0; i < middle1; i++)
            {
                polynomial1S[i] = polynomial1L[i] + polynomial1H[i]; //compute the sum between the halves of the first polynomial
            }
            for (int i = 0; i < middle2; i++)
            {
                polynomial2S[i] = polynomial2L[i] + polynomial2H[i];//compute the sum between the halves of the second polynomial
            }


            var productLTask = Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L)); //get the lower half of the result recursively using the lower halves of the two polynomials
            var productHTask = Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H)); //get the upper half of the result recursively using the lower halves of the two polynomials
            var productMTask = Task.Run(() => MultiplyKaratsubaSequentialRecursive(polynomial1S, polynomial2S)); //get the upper half of the result recursively using the lower halves of the two polynomials
            var productL = productLTask.Result; //get the lower half of the result recursively using the lower halves of the two polynomials
            var productH = productHTask.Result;  //get the upper half of the result recursively using the lower halves of the two polynomials
            var productM = productMTask.Result;  //get the upper half of the result recursively using the lower halves of the two polynomials


            List<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
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

        public static List<float> GeneratePolynomialFromRange(List<float> polynomial, int start, int end)
        {
            List<float> values = polynomial.ToList().GetRange(start, end - start + 1);
            List<float> polynomialSection = new();
            for (int i = 0; i < end - start + 1; i++)
            {
                polynomialSection.Add(values[i]);
            }
            return polynomialSection;
        }

        public static List<float> GenerateZeroPolynomial(int size)
        {
            List<float> polynomial = new();
            for (int i = 0; i < size; i++)
            {
                polynomial.Add(0);
            }
            return polynomial;
        }

        public List<float> FixSize(List<float> A)
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

        public void MultiplyRegular()
        {
            this.ResetResult();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int size = comm.Size;
            var coeff1 = this.Polynomial1.Coefficients;
            var coeff2 = this.Polynomial2.Coefficients;
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            int i0 = 0, j0 = 0;
            int nrCoeff = (m + n) / (2 * (size - 1));
            for (int i = 1; i < size; i++)
            {
                i0 = j0;
                j0 = i0 + nrCoeff;
                if (i == size - 1)
                {
                    j0 = (m + n) / 2;
                }
                int[] buffer1 = new int[1];
                int[] buffer2 = new int[1];
                buffer1[0] = i0;
                buffer2[0] = j0;
                // comm.Send(coeff1.ToArray(), i, 0);
                //comm.Send(coeff2.ToArray(), i, 0);
                comm.Send(buffer1, i, 0);
                comm.Send(buffer2, i, 0);
            }

            List<float> resultList = this.Result.Coefficients;
            for (int i = 1; i < size; i++)
            {
                float[] buffer = new float[m + n - 1];
                comm.Receive(i, 0, ref buffer);
                for (int j = 0; j < m + n - 1; j++)
                {
                    resultList[j] += buffer[j];
                }
            }
            stopwatch1.Stop();
            //if (AllowPrint) //used to avoid the results for the first execution of this function
            //{
            Console.WriteLine("Elapsed time for MultiplyRegularSequential: " + stopwatch1.Elapsed.ToString());
            Console.WriteLine("Result for MultiplyRegularSequential: " + Result.ToString());
            //}
        }

        public void MultiplyRegularPart()
        {
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();

            float[] a = new float[m];
            float[] b = new float[n];
            int[] i0 = new int[1];
            int[] j0 = new int[1];
            float[] resultList = new float[m + n - 1];
            var coeff1 = this.Polynomial1.Coefficients;
            var coeff2 = this.Polynomial2.Coefficients;
            // comm.Receive(0,0, ref a);
            // comm.Receive(0, 0, ref b);
            comm.Receive(0, 0, ref i0);
            comm.Receive(0, 0, ref j0);

            Console.WriteLine("Rank = " + comm.Rank + " i0=" + i0[0] + " j0=" + j0[0]);

            for (int i = i0[0]; i < j0[0]; i++)
            {

                for (int j = 0; j < (m + n) / 2; j++)
                {
                    resultList[i + j] += coeff1[i] * coeff2[j];
                }
            }
            var part = new Polynomial(new List<float>(resultList));
            //Console.WriteLine("-----"+ string.Join(", ", resultList) + "-----");
            // Console.WriteLine(resultList.ToString());
            Console.WriteLine("Partial result of rank " + comm.Rank + " is: " + part.ToString());

            comm.Send(resultList, 0, 0);
        }

        public void MultiplyKaratsuba()
        {
            //used this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            List<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            List<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            List<float> resultList = this.Result.Coefficients;
            resultList.Clear();
            List<float> resultRecursive = MultiplyKaratsubaRecursive(A, B, comm.Size, 0);
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
            int[] final = new int[1];
            final[0] = -1;
            comm.Broadcast(ref final, 0);
            //for (int i = 0; i < comm.Size; i++)
            //{
            //    try {
            //        comm.Send(final, i, 1);
            //    }
            //    catch(Exception e)
            //    {
            //        continue;
            //    }                
            //    }
            //comm.Abort(0);
        }
        public List<float> MultiplyKaratsubaRecursive(List<float> A, List<float> B, int size, int rank)
        {
            List<float> result = new();
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

            List<float> polynomial1L = GeneratePolynomialFromRange(A, 0, middle1 - 1);
            List<float> polynomial1H = GeneratePolynomialFromRange(A, middle1, m - 1);
            List<float> polynomial1M = GenerateZeroPolynomial(middle1);
            List<float> polynomial2L = GeneratePolynomialFromRange(B, 0, middle2 - 1);
            List<float> polynomial2H = GeneratePolynomialFromRange(B, middle2, n - 1);
            List<float> polynomial2M = GenerateZeroPolynomial(middle2);
            for (int i = 0; i < middle1; i++)
            {
                polynomial1M[i] = polynomial1L[i] + polynomial1H[i];
            }
            for (int i = 0; i < middle2; i++)
            {
                polynomial2M[i] = polynomial2L[i] + polynomial2H[i];
            }

            List<float> productL, productH, productM;
            //var productL = MultiplyKaratsubaSequentialRecursive(polynomial1L, polynomial2L);
            //var productH = MultiplyKaratsubaSequentialRecursive(polynomial1H, polynomial2H);
            //var productM = MultiplyKaratsubaSequentialRecursive(polynomial1M, polynomial2M);
            Console.WriteLine("!!--Size for rank " + rank + " is: " + size);
            if (size >= 4)
            {
                int[] sizes = new int[4];

                sizes[0] = size / 4;
                sizes[1] = polynomial1L.Count;
                sizes[2] = polynomial2L.Count;
                sizes[3] = rank;
                Console.WriteLine("!!Nr proc " + sizes[0]);
                comm.Send(sizes, rank + sizes[0], 1);
                Console.WriteLine("!!Sent sizes for low rank " + rank + " dest " + (rank + sizes[0]));
                Console.WriteLine("!!sizes for low rank " + rank + "is: [{0}]", string.Join(", ", sizes));
                comm.Send(polynomial1L.ToArray(), rank + sizes[0], 2);
                Console.WriteLine("!!Sent first for low rank " + rank + " dest " + (rank + sizes[0]));
                comm.Send(polynomial2L.ToArray(), rank + sizes[0], 3);
                Console.WriteLine("!!Sent second for low rank " + rank + " dest " + (rank + sizes[0]));
                int[] productlSize = new int[1], productmSize = new int[1], producthSize = new int[1];
                comm.Receive(rank + sizes[0], 4, ref productlSize);
                Console.WriteLine("!!Received size for low rank " + rank);
                float[] productl = new float[productlSize[0]];
                comm.Receive(rank + sizes[0], 5, ref productl);
                Console.WriteLine("!!Received result for low rank " + rank);

                sizes[1] = polynomial1M.Count;
                sizes[2] = polynomial2M.Count;

                comm.Send(sizes, rank + 2 * sizes[0], 1);
                Console.WriteLine("!!Sent sizes for m rank " + rank + " dest " + (rank + 2 * sizes[0]));
                Console.WriteLine("!!sizes for m rank " + rank + "is: [{0}]", string.Join(", ", sizes));
                comm.Send(polynomial1M.ToArray(), rank + 2 * sizes[0], 2);
                Console.WriteLine("!!Sent first for m rank " + rank + " dest " + (rank + 2 * sizes[0]));
                comm.Send(polynomial2M.ToArray(), rank + 2 * sizes[0], 3);
                Console.WriteLine("!!Sent second for m rank " + rank + " dest " + (rank + 2 * sizes[0]));

                comm.Receive(rank + 2 * sizes[0], 4, ref productmSize);
                Console.WriteLine("!!Received size for m rank " + rank);

                float[] productm = new float[productmSize[0]];



                comm.Receive(rank + 2 * sizes[0], 5, ref productm);
                Console.WriteLine("!!Received result for h rank " + rank);

                sizes[1] = polynomial1H.Count;
                sizes[2] = polynomial2H.Count;

                comm.Send(sizes, rank + 3 * sizes[0], 1);
                Console.WriteLine("!!Sent sizes for h rank " + rank + " dest " + (rank + 3 * sizes[0]));
                Console.WriteLine("!!sizes for m rank " + rank + "is: [{0}]", string.Join(", ", sizes));
                comm.Send(polynomial1H.ToArray(), rank + 3 * sizes[0], 2);
                Console.WriteLine("!!Sent first for h rank " + rank + " dest " + (rank + 3 * sizes[0]));
                comm.Send(polynomial2H.ToArray(), rank + 3 * sizes[0], 3);
                Console.WriteLine("!!Sent second for h rank " + rank + " dest " + (rank + 3 * sizes[0]));
                float[] product = new float[productmSize[0]];

                comm.Receive(rank + 3 * sizes[0], 4, ref producthSize);
                Console.WriteLine("!!Received size for h rank " + rank);

                float[] producth = new float[producthSize[0]];



                comm.Receive(rank + 3 * sizes[0], 5, ref producth);
                Console.WriteLine("!!Received result for h rank " + rank);

                //productH = MultiplyKaratsubaRecursive(polynomial1H, polynomial2H, sizes[0],rank);
                //sizes[1] = polynomial1H.Count;
                //sizes[2] = polynomial2H.Count;
                //comm.Send(sizes, rank + sizes[0], 1);
                //comm.Send(polynomial1M.ToArray(), rank + sizes[0], 2);
                //comm.Send(polynomial2M.ToArray(), rank + sizes[0], 3);

                leftProcesses.Add(rank + 3 * sizes[0]);
                leftProcesses.Add(rank + sizes[0]);
                leftProcesses.Add(rank + 2 * sizes[0]);


                productH = producth.ToList();
                productL = productl.ToList();
                productM = productm.ToList();

            }
            else
            {

                productL = MultiplyKaratsubaRecursive(polynomial1L, polynomial2L, 1, rank);

                productM = MultiplyKaratsubaRecursive(polynomial1M, polynomial2M, 1, rank);

                productH = MultiplyKaratsubaRecursive(polynomial1H, polynomial2H, 1, rank);
                leftProcesses.Add(rank);
            }


            List<float> polynomialMiddle = GenerateZeroPolynomial((A.Count + B.Count) / 2);
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
            var part = new Polynomial(result);
            //Console.WriteLine("-----"+ string.Join(", ", resultList) + "-----");
            // Console.WriteLine(resultList.ToString());
            Console.WriteLine("!!!Partial result of rank " + rank + " is: " + part.ToString());
            leftProcesses.Add(rank);

            return result;

        }

        public void MultiplyKaratsubaRecursivePart(int rank)
        {
            Console.WriteLine("Here is rank " + rank);
            int[] sizes = new int[4];
            comm.Receive(Communicator.anySource, 1, ref sizes);
            Console.WriteLine("~~Received size rank " + rank);
            Console.WriteLine("~~sizes = [{0}]", string.Join(", ", sizes));
            if (sizes[0] != -1)
            {
                Console.WriteLine("Received size rank " + rank);
                Console.WriteLine("sizes = [{0}]", string.Join(", ", sizes));
                float[] coeff1 = new float[sizes[1]];
                float[] coeff2 = new float[sizes[2]];

                int source = sizes[3];
                Console.WriteLine("Source rank " + rank + " is: " + source);
                comm.Receive(source, 2, ref coeff1);
                comm.Receive(source, 3, ref coeff2);
                Console.WriteLine("Received coeffs rank " + source);
                Console.WriteLine("coeff1 = [{0}]", string.Join(", ", coeff1));
                Console.WriteLine("coeff2 = [{0}]", string.Join(", ", coeff2));
                List<float> resultList = MultiplyKaratsubaRecursive(coeff1.ToList(), coeff2.ToList(), sizes[0], rank);

                var part = new Polynomial(resultList);
                //Console.WriteLine("-----"+ string.Join(", ", resultList) + "-----");
                // Console.WriteLine(resultList.ToString());
                Console.WriteLine("Partial result of rank " + rank + " is: " + part.ToString());
                leftProcesses.Add(rank);
                int[] rSize = { resultList.Count };

                comm.Send(rSize, source, 4);
                comm.Send(resultList.ToArray(), source, 5);
                Console.WriteLine("Sent results rank " + source);
            }
        }

        public void KaratsubaPartial(int rank)
        {
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();

            float[] a = new float[m];
            float[] b = new float[n];
            int[] i0 = new int[1];
            int[] j0 = new int[1];
            var coeff1 = this.Polynomial1.Coefficients;
            var coeff2 = this.Polynomial2.Coefficients;
            var copy1 = this.Polynomial1.GetCopyCoefficients();
            var copy2 = this.Polynomial2.GetCopyCoefficients();
            // comm.Receive(0,0, ref a);
            // comm.Receive(0, 0, ref b);
           // Console.WriteLine("Before receive rank "+rank);
            comm.Receive(0, 0, ref i0);
            comm.Receive(0, 0, ref j0);


            Polynomial pol1 = new Polynomial(copy1);
            Polynomial pol2 = new Polynomial(copy2);

            for (int i = 0; i < i0[0]; i++)
            {
                pol1.Coefficients[i] = 0;
            }
            for (int j = j0[0]; j < pol1.Coefficients.Count; j++)
            {
                pol1.Coefficients[j]= 0;
            }
            List<float> A = FixSize(pol1.GetCopyCoefficients());
            List<float> B = FixSize(pol2.GetCopyCoefficients());
            List<float> resultList = this.Result.Coefficients;
            resultList.Clear();
            List<float> result = MultiplyKaratsubaSequentialRecursive(A, B);
            //Console.WriteLine("result rank "+rank+" = [{0}]", string.Join(", ", result));
            int[] sizeR = { result.Count };
            comm.Send(sizeR, 0, 0);
            comm.Send(result.ToArray(),0, 0);
            //Console.WriteLine("Sent result rank "+rank);
        }

        public List<float> MultiplyKaratsubaSequentialPartial()
        {
            //used this https://stackoverflow.com/questions/16502997/karatsuba-multiplication-for-unequal-size-non-power-of-2-operands
            List<float> A = FixSize(this.Polynomial1.GetCopyCoefficients());
            List<float> B = FixSize(this.Polynomial2.GetCopyCoefficients());
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            List<float> resultList = this.Result.Coefficients;
            resultList.Clear();
            List<float> resultRecursive = MultiplyKaratsubaSequentialRecursive(A, B);
            stopwatch1.Stop();
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            for (int i = 0; i < m + n - 1; i++)
            {
                resultList.Add(resultRecursive[i]);
            }

            return resultList;
        }

        public void MultiplyKaratsubaMain()
        {
            this.ResetResult();
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            int size = comm.Size;
            var coeff1 = this.Polynomial1.Coefficients;
            var coeff2 = this.Polynomial2.Coefficients;
            int m = this.Polynomial1.GetDegreePolynomial();
            int n = this.Polynomial2.GetDegreePolynomial();
            int i0 = 0, j0 = 0;
            int nrCoeff = (m + n) / (2 * (size - 1));
            for (int i = 1; i < size; i++)
            {
                i0 = j0;
                j0 = i0 + nrCoeff;
                if (i == size - 1)
                {
                    j0 = (m + n) / 2;
                }
                int[] buffer1 = new int[1];
                int[] buffer2 = new int[1];
                buffer1[0] = i0;
                buffer2[0] = j0;
                // comm.Send(coeff1.ToArray(), i, 0);
                //comm.Send(coeff2.ToArray(), i, 0);
                comm.Send(buffer1, i, 0);
                comm.Send(buffer2, i, 0);
               // Console.WriteLine("Sent buffers");
            }
            List<Polynomial> results = new();
           
            for (int i = 1; i < size; i++)
            {
                int[] sizeR =new int[1];
                comm.Receive(i,0,ref sizeR);
                float[] resultL = new float[sizeR[0]];
                comm.Receive(i, 0, ref resultL);
                results.Add(new Polynomial(resultL.ToList()));
            }

            Polynomial result = Polynomial.CombineResults(results);

            stopwatch1.Stop();
            Console.WriteLine("Elapsed time for MultiplyKaratsubaMain: " + stopwatch1.Elapsed.ToString());
            Console.WriteLine("Result for MultiplyKaratsubaMain: " + result.ToString());

        }
    }


}


