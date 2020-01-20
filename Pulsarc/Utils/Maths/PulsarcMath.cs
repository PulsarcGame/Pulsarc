using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.Utils.Maths
{
    public static class PulsarcMath
    {
        /// <summary>
        /// Linear Interpolation between two floats.
        /// </summary>
        /// <param name="firstFloat"></param>
        /// <param name="secondFloat"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }

        /// <summary>
        /// Caculate the Standard Deviation from a set of doubles.
        /// </summary>
        /// <param name="numberSet">The list of numbers to calculate
        /// the Standard Deviation of.</param>
        /// <returns>The Standard Deviation of the set.</returns>
        public static double CalcStdDeviation(List<double> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }

        /// <summary>
        /// Caculate the Standard Deviation from a set of ints.
        /// </summary>
        /// <param name="numberSet">The list of numbers to calculate
        /// the Standard Deviation of.</param>
        /// <returns>The Standard Deviation of the set.</returns>
        public static double CalcStdDeviation(List<int> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }
    }
}
