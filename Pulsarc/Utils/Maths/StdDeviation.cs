using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsarc.Utils.Maths
{
    static class StdDeviation
    {
        public static double calc(List<double> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }
        public static double calc(List<int> numberSet)
        {
            double mean = numberSet.Sum() / numberSet.Count;

            return Math.Sqrt(numberSet.Sum(x => Math.Pow(x - mean, 2)) / (numberSet.Count - 1));
        }
    }
}
