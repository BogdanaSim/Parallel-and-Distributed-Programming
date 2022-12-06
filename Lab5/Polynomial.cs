using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public class Polynomial
    {
        public SynchronizedCollection<float> coefficients { get; set; }

        public Polynomial(SynchronizedCollection<float> coefficients)
        {
            this.coefficients = coefficients;
        }

        public int GetDegreePolynomial()
        {
            return coefficients.Count;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();  
            for(int i = 0; i < coefficients.Count; i++)
            {
                if(i==0 && coefficients[i]!=0)
                    stringBuilder.Append(coefficients[i].ToString());
                else if (coefficients[i] > 0)
                {
                    stringBuilder.Append("+"+coefficients[i].ToString() + "*x^"+i.ToString());

                }
                else if (coefficients[i] < 0)
                {
                    stringBuilder.Append(coefficients[i].ToString() + "*x^" + i.ToString());
                }

            }
            return stringBuilder.ToString();
        }
    }
}
