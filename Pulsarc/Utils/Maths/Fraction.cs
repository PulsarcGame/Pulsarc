using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Utils.Maths
{
    // https://rosettacode.org/wiki/Convert_decimal_number_to_rational#C.23
    public class Fraction
    {
        public Int64 Numerator;
        public Int64 Denominator;
        public Fraction(double f, Int64 MaximumDenominator = 4096)
        {
            /* Translated from the C version. */
            /*  a: continued fraction coefficients. */
            Int64 a;
            var h = new Int64[3] { 0, 1, 0 };
            var k = new Int64[3] { 1, 0, 0 };
            Int64 x, d, n = 1;
            int i, neg = 0;

            if (MaximumDenominator <= 1)
            {
                Denominator = 1;
                Numerator = (Int64)f;
                return;
            }

            if (f < 0) { neg = 1; f = -f; }

            while (f != Math.Floor(f)) { n <<= 1; f *= 2; }
            d = (Int64)f;

            /* continued fraction and check denominator each step */
            for (i = 0; i < 64; i++)
            {
                a = (n != 0) ? d / n : 0;
                if ((i != 0) && (a == 0)) break;

                x = d; d = n; n = x % n;

                x = a;
                if (k[1] * a + k[0] >= MaximumDenominator)
                {
                    x = (MaximumDenominator - k[0]) / k[1];
                    if (x * 2 >= a || k[1] >= MaximumDenominator)
                        i = 65;
                    else
                        break;
                }

                h[2] = x * h[1] + h[0]; h[0] = h[1]; h[1] = h[2];
                k[2] = x * k[1] + k[0]; k[0] = k[1]; k[1] = k[2];
            }
            Denominator = k[1];
            Numerator = neg != 0 ? -h[1] : h[1];
        }
        public override string ToString()
        {
            return string.Format("{0} / {1}", Numerator, Denominator);
        }
    }
}
