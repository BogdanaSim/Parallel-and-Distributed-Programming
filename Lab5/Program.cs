namespace Lab5
{
    public class Program
    {
        private async static Task Main()
        {
            // Polynomial polynomial1 = new(new SynchronizedCollection<float> { 5, 0, 10, 6, 7, 12 });
            //Polynomial polynomial2 = new(new SynchronizedCollection<float> { 1, 2, 4, 0, 5});
            Polynomial polynomial1 = Polynomial.GenerateRandomPolynomial(1000);
            Polynomial polynomial2 = Polynomial.GenerateRandomPolynomial(1000);
            Multiply multiply = new(polynomial1, polynomial2);
            for (int i = 0; i < 20; i++) //loop to see if the time lapses are consistent
            {
                if (i > 1)  // avoid printing the first results to get rid of any warmup factors (JIT / caching etc).
                    multiply.AllowPrint = true;
                multiply.MultiplyRegularSequential();
                multiply.MultiplyRegularParallel(5);
                multiply.MultiplyKaratsubaSequential();
                await multiply.MultiplyKaratsubaParallel();
                Console.WriteLine("\n--------------------------------------------\n");
            }
        }
    }
}