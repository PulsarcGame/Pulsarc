using System;
using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.Utils.Maths
{
    static class StdDeviation
    {
        public static double Calc(List<double> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }
        public static double Calc(List<int> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }
    }
}