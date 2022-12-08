using System.Text;

namespace Lab5
{
    public class Polynomial
    {
        public SynchronizedCollection<float> Coefficients { get; set; } // used SynchronizedCollection instead of lists since order matters

        public Polynomial(SynchronizedCollection<float> coefficients)
        {
            this.Coefficients = coefficients;
        }

        public static Polynomial GenerateRandomPolynomial(int degree)
        {
            Random rnd = new();
            SynchronizedCollection<float> _coefficients = new(degree);
            for (int i = 0; i < degree; i++)
            {
                _coefficients.Add((float)rnd.Next(1, 100));
            }
            return new Polynomial(_coefficients);
        }

        public int GetDegreePolynomial()
        {
            return Coefficients.Count;
        }

        public SynchronizedCollection<float> GetCopyCoefficients()
        {
            SynchronizedCollection<float> _coefficients = new(Coefficients.Count);
            for (int i = 0; i < Coefficients.Count; i++)
            {
                _coefficients.Add(Coefficients[i]);
            }
            return _coefficients;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
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