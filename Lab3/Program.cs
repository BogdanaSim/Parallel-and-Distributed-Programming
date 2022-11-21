using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{

    public class Program
    {
        public static Matrix<double> m1 = new Matrix<double>(4,2);
        public static Matrix<double> m2 = new Matrix<double>(2,4);


        public static void Create()
        {
            //m1[0, 0] = 1;
            //m1[0, 1] = 2;
            //m1[0, 2] = 3;
            //m1[1, 0] = 4;
            //m1[1, 1] = 5;
            //m1[1, 2] = 6;

            //m2[0, 0] = 7;
            //m2[0, 1] = 8;
            //m2[1, 0] = 9;
            //m2[1, 1] = 10;
            //m2[2, 0] = 11;
            //m2[2, 1] = 12;
            //m1[0, 0] = 3;
            //m1[0, 1] = 4;
            //m1[0, 2] = 2;

            //m2[0,0] = 13;
            //m2[0, 1] = 9;
            //m2[0, 2] = 7;
            //m2[0, 3] = 15;
            //m2[1, 0] = 8;
            //m2[1, 1] = 7;
            //m2[1, 2] = 4;
            //m2[1, 3] = 6;
            //m2[2, 0] = 6;
            //m2[2, 1] = 4;
            //m2[2, 2] = 0;
            //m2[2, 3] = 3;
            //m1[0, 0] = 2;
            //m1[0, 1] = -1;
            //m1[0, 2] = -2;
            //m1[1, 0] = 3;
            //m1[1, 1] = 6;
            //m1[1, 2] = -4;

            //m2[0, 0] = 1;
            //m2[0, 1] = 2;
            //m2[0, 2] = 4;
            //m2[0, 3] = 7;
            //m2[1, 0] = -1;
            //m2[1, 1] = 6;
            //m2[1, 2] = -3;
            //m2[1, 3] = 5;
            //m2[2, 0] = 3;
            //m2[2, 1] = 8;
            //m2[2, 2] = 9;
            //m2[2, 3] = 10;

            m1[0, 0] = 3;
            m1[0, 1] = 1;
            m1[1, 0] = 4;
            m1[1, 1] = 5;
            m1[2, 0] = 6;
            m1[2, 1] = 2;
            m1[3, 0] = 1;
            m1[3, 1] = 2;


            m2[0, 0] = 7;
            m2[0, 1] = 8;
            m2[0, 2] = 9;
            m2[0, 3] = 4;
            m2[1, 0] = 1;
            m2[1, 1] = 3;
            m2[1, 2] = 3;
            m2[1, 3] = 7;


        }
        public static void Main(string[] args)
        {
            Create();
            MultiplyMatrices multiplyMatrices = new MultiplyMatrices(m1, m2, 3);
            multiplyMatrices.GetResult1();
            Matrix<double> res = multiplyMatrices.result;
            Console.WriteLine(res.ToString());
        }
    }
}
