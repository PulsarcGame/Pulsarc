using Pulsarc.Utils.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Beatmaps
{
    class DifficultyCalculation
    {
        private const double baseDiff = 1;
        private const double divider = 10;

        private const double kpsFingerSlope = 1.8;
        private const double maxKpsDiff = 40;
        private const double strainDecay = 2;

        private const int sectionLength = 400;
        private const int minLength = 60;

        private const double weightSlope = 0.9;
        private const double weightBase = 0.6;

        static readonly double[] keyDiff = {
            // Left
            1,
            // Up
            1.15,
            // Down
            1.15,
            // Right
            1,
        };
        
        const double maxSections = minLength / (sectionLength / 1000f);

        /// <summary>
        /// Find the difficulty of as map by splitting a 2D List representation
        /// of the beatmap into sections.
        /// </summary>
        /// <param name="columns">A 2D List that represents the beatmap.</param>
        /// <param name="previousStrain"></param>
        /// <returns></returns>
        static public double GetSectionDifficulty(List<List<Arc>> columns, double previousStrain)
        {
            double diff = 0;
            int key = 0;

            List<double> diffs = new List<double>();

            foreach (List<Arc> arcs in columns)
            {
                if (arcs.Count > 0)
                {
                    List<int> times = new List<int>();

                    foreach (Arc arc in arcs)
                        times.Add(arc.Time);

                    double stdDiff = PulsarcMath.CalcStdDeviation(times);

                    double c = Math.Min((Math.Pow(arcs.Count,kpsFingerSlope) * (sectionLength / 1000f)), maxKpsDiff) * keyDiff[key];
                    diff += c;
                    diffs.Add(c);
                }
                else
                    diffs.Add(0);

                key++;
            }

            var std = diffs.Count > 0 ? PulsarcMath.CalcStdDeviation(diffs) : 0;

            return diff + std + (previousStrain / strainDecay);
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
            double current_strain = 0;

            while (!done)
            {
                List<List<Arc>> current_section = new List<List<Arc>>();

                for (int i = 0; i < beatmap.KeyCount; i++)
                    current_section.Add(new List<Arc>());

                currentTime += sectionLength;
                current_strain /= strainDecay;

                for (int k = 0; k < columns.Count; k++)
                {
                    List<Arc> column = columns[k];

                    while (column.Count > 0 && column[0].Time <= currentTime)
                    {
                        current_section[k].Add(column[0]);
                        column.RemoveAt(0);
                    }
                }

                done = true;

                for (int k = 0; k < columns.Count; k++)
                    if (columns[k].Count != 0)
                        done = false;

                if (!done)
                {
                    current_strain = GetSectionDifficulty(current_section, current_strain);

                    diffs.Add(new KeyValuePair<double, int>(current_strain, currentTime));
                }
            }

            diffs.Sort((y, x) => x.Key.CompareTo(y.Key));

            double difficulty = 0;
            double weight = weightBase;

            for (int i = 0; i < maxSections && i < diffs.Count; i++)
            {
                difficulty += diffs[i].Key * weight;
                weight *= weightSlope;
            }

            difficulty /= divider;

            return baseDiff + difficulty;
        }

        /// <summary>
        /// Find the difficulty of the provided beatmap.
        /// </summary>
        /// <param name="beatmap">The beatmap to find the difficulty of.</param>
        /// <returns></returns>
        static public double GetDifficulty(Beatmap beatmap)
        {
            double difficulty = 0;

            difficulty += GetDensityDifficulty(beatmap);
            
            return difficulty;
        }
    }
}
