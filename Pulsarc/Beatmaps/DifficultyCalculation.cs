using System;
using System.Collections.Generic;
using System.Linq;
using Pulsarc.Utils.Maths;

namespace Pulsarc.Beatmaps
{
    class DifficultyCalculation
    {
        private const double BaseDiff = 1;
        private const double Divider = 10;

        private const double KpsFingerSlope = 1.8;
        private const double MaxKpsDiff = 40;
        private const double StrainDecay = 2;

        private const int SectionLength = 400;
        private const int MinLength = 60;

        private const double WeightSlope = 0.9;
        private const double WeightBase = 0.6;
        
        const double MaxSections = MinLength / (SectionLength / 1000f);

        /// <summary>
        /// Find the difficulty of as map by splitting a 2D List representation
        /// of the beatmap into sections.
        /// </summary>
        /// <param name="columns">A 2D List that represents the beatmap.</param>
        /// <param name="previousStrain"></param>
        /// <returns></returns>
        private static double GetSectionDifficulty(List<List<Arc>> columns, double previousStrain)
        {
            double diff = 0;

            List<double> diffs = new List<double>();

            foreach (List<Arc> arcs in columns)
            {
                if (arcs.Count > 0)
                {
                    List<int> times = arcs.Select(arc => arc.Time).ToList();

                    double stdDiff = PulsarcMath.CalcStdDeviation(times);

                    double c = Math.Min(Math.Pow(arcs.Count,KpsFingerSlope) * (SectionLength / 1000f), MaxKpsDiff);
                    diff += c;
                    diffs.Add(c);
                }
                else
                    diffs.Add(0);
            }

            var std = diffs.Count > 0 ? PulsarcMath.CalcStdDeviation(diffs) : 0;

            return diff + std + previousStrain / StrainDecay;
        }

        /// <summary>
        /// Turn a Beatmap into a 2D List of Arcs
        /// </summary>
        /// <param name="beatmap">The beatmap to convert.</param>
        /// <returns></returns>
        static public List<List<Arc>> GetColumns(Beatmap beatmap)
        {
            List<List<Arc>> columns = new List<List<Arc>>();

            for (int k = 0; k < beatmap.KeyCount; k++)
                columns.Add(new List<Arc>());

            foreach (Arc arc in beatmap.Arcs)
                for (int k = 0; k < beatmap.KeyCount; k++)
                    if (BeatmapHelper.IsColumn(arc, k))
                        columns[k].Add(arc);

            return columns;
        }

        /// <summary>
        /// Find the difficulty of a map by splitting it into sections and
        /// measuring the density between sections.
        /// TODO: Comments
        /// TODO: This method could probably be split up into multiple parts.
        /// Note: Tried to split this into multiple methods and ended up causing
        /// an overflow, I'm guessing the columns didn't get split into sections properly,
        /// I'll try again later -FRUP
        /// </summary>
        /// <param name="beatmap">The beatmap to find the difficulty of.</param>
        /// <returns></returns>
        static public double GetDensityDifficulty(Beatmap beatmap)
        {
            List<List<Arc>> columns = GetColumns(beatmap);
            List<KeyValuePair<double, int>> diffs = new List<KeyValuePair<double, int>>();

            bool done = false;
            int currentTime = 0;
            double currentStrain = 0;

            while (!done)
            {
                List<List<Arc>> currentSection = new List<List<Arc>>();

                for (int i = 0; i < beatmap.KeyCount; i++)
                    currentSection.Add(new List<Arc>());

                currentTime += SectionLength;
                currentStrain /= StrainDecay;

                for (int k = 0; k < columns.Count; k++)
                {
                    List<Arc> column = columns[k];

                    while (column.Count > 0 && column[0].Time <= currentTime)
                    {
                        currentSection[k].Add(column[0]);
                        column.RemoveAt(0);
                    }
                }

                done = true;

                foreach (var unused in columns.Where(t => t.Count != 0))
                    done = false;

                if (done) continue;
                currentStrain = GetSectionDifficulty(currentSection, currentStrain);

                diffs.Add(new KeyValuePair<double, int>(currentStrain, currentTime));
            }

            diffs.Sort((y, x) => x.Key.CompareTo(y.Key));

            double difficulty = 0;
            double weight = WeightBase;

            for (int i = 0; i < MaxSections && i < diffs.Count; i++)
            {
                difficulty += diffs[i].Key * weight;
                weight *= WeightSlope;
            }

            difficulty /= Divider;

            return BaseDiff + difficulty;
        }

        /// <summary>
        /// Find the difficulty of the provided beatmap.
        /// </summary>
        /// <param name="beatmap">The beatmap to find the difficulty of.</param>
        /// <returns></returns>
        public static double GetDifficulty(Beatmap beatmap)
        {
            double difficulty = 0;

            difficulty += GetDensityDifficulty(beatmap);
            
            return difficulty;
        }
    }
}
