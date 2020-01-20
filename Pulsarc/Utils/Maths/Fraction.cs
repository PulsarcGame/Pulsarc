using System;

namespace Pulsarc.Utils.Maths
{
    // https://rosettacode.org/wiki/Convert_decimal_number_to_rational#C.23
    public class Fraction
    {
        private readonly Int64 _numerator;
        private readonly Int64 _denominator;
        public Fraction(double f, Int64 maximumDenominator = 4096)
        {
            /* Translated from the C version. */
            /*  a: continued fraction coefficients. */
            Int64 a;
            var h = new Int64[] { 0, 1, 0 };
            var k = new Int64[] { 1, 0, 0 };
            Int64 x, d, n = 1;
            int i, neg = 0;

            if (maximumDenominator <= 1)
            {
                _denominator = 1;
                _numerator = (Int64)f;
                return;
            }

            if (f < 0) { neg = 1; f = -f; }

            while (f != Math.Floor(f)) { n <<= 1; f *= 2; }
            d = (Int64)f;

            /* continued fraction and check denominator each step */
            for (i = 0; i < 64; i++)
            {
                a = n != 0 ? d / n : 0;
                if (i != 0 && a == 0) break;

                x = d; d = n; n = x % n;

                x = a;
                if (k[1] * a + k[0] >= maximumDenominator)
                {
                    x = (maximumDenominator - k[0]) / k[1];
                    if (x * 2 >= a || k[1] >= maximumDenominator)
                        i = 65;
                    else
                        break;
                }

                h[2] = x * h[1] + h[0]; h[0] = h[1]; h[1] = h[2];
                k[2] = x * k[1] + k[0]; k[0] = k[1]; k[1] = k[2];
            }
            _denominator = k[1];
            _numerator = neg != 0 ? -h[1] : h[1];
        }
        public override string ToString()
        {
            return $"{_numerator} / {_denominator}";
        }
    }
}
