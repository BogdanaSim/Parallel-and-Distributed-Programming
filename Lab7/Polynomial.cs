using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections;

namespace Lab7
{
    [Serializable]
    public class Polynomial
    {

        public List<float> Coefficients { get; set; } // used List instead of lists since order matters

        public Polynomial(List<float> coefficients)
        {
            this.Coefficients = coefficients;
        }

        public static Polynomial GenerateRandomPolynomial(int degree)
        {
            Random rnd = new();
            List<float> _coefficients = new(degree);
            for (int i = 0; i < degree; i++)
            {
                _coefficients.Add((float)rnd.Next(1, 100));
            }
            return new Polynomial(_coefficients);
        }

        public bool CheckIfZeroPolynomial()
        {
            return Coefficients.All(item => item.Equals(0));
        }

        public int GetDegreePolynomial()
        {
            return Coefficients.Count;
        }

        public List<float> GetCopyCoefficients()
        {
            List<float> _coefficients = new(Coefficients.Count);
            for (int i = 0; i < Coefficients.Count; i++)
            {
                _coefficients.Add(Coefficients[i]);
            }
            return _coefficients;
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
        public static Polynomial CombineResults(List<Polynomial> polynomials)
        {
            int size = polynomials[0].GetDegreePolynomial();
            Polynomial result = new Polynomial(GenerateZeroPolynomial(size + 1));
            foreach (Polynomial polynomial in polynomials)
            {
                result = AddPolynomials(result, polynomial);
            }

            return result;
        }

        public static Polynomial AddPolynomials(Polynomial p1, Polynomial p2)
        {
            int minDegree = Math.Min(p1.GetDegreePolynomial(), p2.GetDegreePolynomial());
            int maxDegree = Math.Max(p1.GetDegreePolynomial(), p2.GetDegreePolynomial());
            minDegree--;
            maxDegree--;
            List<float> coefficients = new List<float>();
            //Console.WriteLine("min = " + minDegree);
            //Console.WriteLine("max = "+maxDegree);
            //Console.WriteLine("p1 = [{0}]", string.Join(", ", p1.Coefficients));
            //Console.WriteLine("p2 = [{0}]", string.Join(", ", p2.Coefficients));
            //Add the 2 polynomials
            for (int i = 0; i <= minDegree; i++)
            {
                coefficients.Add(p1.Coefficients[i] + p2.Coefficients[i]);
            }

            if (minDegree != maxDegree)
            {
                if (maxDegree == p1.GetDegreePolynomial()-1)
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(p1.Coefficients[i]);
                    }
                }
                else
                {
                    for (int i = minDegree + 1; i <= maxDegree; i++)
                    {
                        coefficients.Add(p2.Coefficients[i]);
                    }
                }
            }

            return new Polynomial(coefficients);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            if (CheckIfZeroPolynomial())
            {
                return "0";
            }
            for (int i = 0; i < Coefficients.Count; i++)
            {
                if (i == 0 && Coefficients[i] != 0)
                    stringBuilder.Append(Coefficients[i]);
                else if (Coefficients[i] > 0)
                {
                    stringBuilder.Append("+" + Coefficients[i].ToString() + "*x^" + i.ToString());
                }
                else if (Coefficients[i] < 0)
                {
                    stringBuilder.Append(Coefficients[i].ToString() + "*x^" + i.ToString());
                }
            }
            return stringBuilder.ToString();
        }
    }
}
