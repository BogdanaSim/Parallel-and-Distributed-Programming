using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{

    public class Program
    {
        public static Matrix<double> m1 = new Matrix<double>(2,3);
        public static Matrix<double> m2 = new Matrix<double>(3,2);


        public static void Create()
        {
            m1[0, 0] = 1;
            m1[0, 1] = 2;
            m1[0, 2] = 3;
            m1[1, 0] = 4;
            m1[1, 1] = 5;
            m1[1, 2] = 6;

            m2[0, 0] = 7;
            m2[0, 1] = 8;
            m2[1, 0] = 9;
            m2[1, 1] = 10;
            m2[2, 0] = 11;
            m2[2, 1] = 12;
        }
        public static void Main(string[] args)
        {
            Create();
            MultiplyMatrices multiplyMatrices = new MultiplyMatrices(m1, m2, 2);
            multiplyMatrices.GetResult();
            Matrix<double> res = multiplyMatrices.result;
            Console.WriteLine(res.ToString());
        }
    }
}
