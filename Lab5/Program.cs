// See https://aka.ms/new-console-template for more information
using Lab5;

Polynomial polynomial1 = new Polynomial(new SynchronizedCollection<float> { 5, 0, 10, 6 });
Polynomial polynomial2 = new Polynomial(new SynchronizedCollection<float> { 1, 2, 4 });
Multiply multiply = new Multiply(polynomial1, polynomial2);
multiply.MultiplyRegularSequential();
multiply.MultiplyRegularParallel(3);
//Console.WriteLine(multiply.result.ToString());
