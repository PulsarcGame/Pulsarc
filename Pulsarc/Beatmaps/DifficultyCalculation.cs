using Pulsarc.Utils.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Beatmaps
{
    class DifficultyCalculation
    {
        const double baseDiff = 1;
        const double divider = 10;

        const double kpsFingerSlope = 1.8;
        const double maxKpsDiff = 40;
        const double strainDecay = 2;

        const int sectionLength = 400;
        const int minLength = 60;

        const double weightSlope = 0.9;
        const double weightBase = 0.6;

        static readonly double[] keyDiff = {
            1, // Left
            1.15, // Up
            1.15, // Down
            1, // Right
        };
        
        const double maxSections = minLength / (sectionLength / 1000f);

        static public double GetSectionDifficulty(List<List<Arc>> columns, double previousStrain)
        {
            double diff = 0;
            int key = 0;

            List<double> diffs = new List<double>();

            foreach(List<Arc> arcs in columns) {
                if (arcs.Count > 0)
                {
                    List<int> times = new List<int>();
                    foreach(Arc arc in arcs)
                    {
                        times.Add(arc.time);
                    }
                    double stdDiff = StdDeviation.calc(times);

                    double c = Math.Min((Math.Pow(arcs.Count,kpsFingerSlope) * (sectionLength / 1000f)), maxKpsDiff) * keyDiff[key];
                    diff += c;
                    diffs.Add(c);
                }
                else
                {
                    diffs.Add(0);
                }
                key++;
            }

            var std = diffs.Count > 0 ? StdDeviation.calc(diffs) : 0;

            return diff + std + (previousStrain / strainDecay);
        }

        static public List<List<Arc>> GetColumns(Beatmap beatmap)
        {
            List<List<Arc>> columns = new List<List<Arc>>();

            for(int k = 0; k < beatmap.KeyCount; k++)
            {
                columns.Add(new List<Arc>());
            }

            foreach(Arc arc in beatmap.arcs)
            {
                for (int k = 0; k < beatmap.KeyCount; k++)
                {
                    if (BeatmapHelper.isColumn(arc, k))
                    {
                        columns[k].Add(arc);
                    }
                }
            }

            return columns;
        }

        static public double GetDensityDifficulty(Beatmap beatmap)
        {
            List<List<Arc>> columns = GetColumns(beatmap);
            List<KeyValuePair<double, int>> diffs = new List<KeyValuePair<double, int>>();

            bool done = false;
            int t = 0;
            double current_strain = 0;

            while (!done)
            {
                List<List<Arc>> current_section = new List<List<Arc>>();
                for (int i = 0; i < beatmap.KeyCount; i++)
                {
                    current_section.Add(new List<Arc>());
                }

                t += sectionLength;
                current_strain /= strainDecay;

                for (int k = 0; k < columns.Count; k++)
                {
                    List<Arc> column = columns[k];
                    while (column.Count > 0 && column[0].time <= t)
                    {
                        current_section[k].Add(column[0]);
                        column.RemoveAt(0);
                    }
                };

                done = true;

                for (int k = 0; k < columns.Count; k++)
                {
                    if (columns[k].Count != 0)
                    {
                        done = false;
                    }
                };

                if (!done)
                {
                    current_strain = GetSectionDifficulty(current_section, current_strain);

                    diffs.Add(new KeyValuePair<double, int>(current_strain, t));
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

        static public double GetDifficulty(Beatmap beatmap)
        {
            double difficulty = 0;

            difficulty += GetDensityDifficulty(beatmap);
            
            return difficulty;
        }
    }
}
