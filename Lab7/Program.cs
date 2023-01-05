namespace Lab7;

public class Program
{
    static void Main(String[] args)

    {

            MPI.Environment.Run(ref args, comm =>
            {
            for (int i = 0; i < 10; i++)
            {
                Polynomial polynomial1 = new(new List<float> { 5, 0, 10, 6, 7, 12 });
                    Polynomial polynomial2 = new(new List<float> { 1, 2, 4, 0, 5, 8 });

                    Multiply multiply = new(polynomial1, polynomial2, comm);

                    if (comm.Rank == 0)
                    {
                        //Polynomial polynomial1 = Polynomial.GenerateRandomPolynomial(1000);
                        //Polynomial polynomial2 = Polynomial.GenerateRandomPolynomial(1000);
                        // program for rank 0
                        multiply.MultiplyRegular();
                        //multiply.MultiplyKaratsubaMain();
                       // multiply.MultiplyKaratsuba();
                    }
                    else // not rank 0
                    {
                        //multiply.KaratsubaPartial(comm.Rank);
                        //Console.WriteLine("See for rank " + comm.Rank.ToString());
                        //multiply.MultiplyKaratsubaRecursivePart(comm.Rank);
                         multiply.MultiplyRegularPart();
                        // program for all other ranks
                    }
                }
            });
        
        // Polynomial polynomial1 = new(new List<float> { 5, 0, 10, 6, 7, 12 });
        //Polynomial polynomial2 = new(new List<float> { 1, 2, 4, 0, 5});
        //Polynomial polynomial1 = Polynomial.GenerateRandomPolynomial(1000);
        //Polynomial polynomial2 = Polynomial.GenerateRandomPolynomial(1000);
        //Multiply multiply = new(polynomial1, polynomial2);
        //for (int i = 0; i < 20; i++) //loop to see if the time lapses are consistent
        //{
        //    if (i > 1)  // avoid printing the first results to get rid of any warmup factors (JIT / caching etc).
        //        multiply.AllowPrint = true;
        //    multiply.MultiplyRegularSequential();
        //    multiply.MultiplyRegularParallel(5);
        //    multiply.MultiplyKaratsubaSequential();
        //    await multiply.MultiplyKaratsubaParallel();
        //    Console.WriteLine("\n--------------------------------------------\n");
        //}
    }
}

