using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Judgement
    {
        /// <summary>
        /// The different judgements that can be obtained during gameplay.
        /// Can be easily edited to change how Judgement works.
        /// </summary>
        public static readonly List<JudgementValue> Judgements = new List<JudgementValue>()
        {
            //////////////// Judge equal to Stepmania J4 /////////////
            new JudgementValue("max",       1,      22,    320,     2),
            new JudgementValue("perfect",   1,      45,    300,     1),
            new JudgementValue("great",  2/3d,      90,    200,    -8),
            new JudgementValue("good",   1/3d,     135,    100,   -15),
            new JudgementValue("bad",    1/6d,     180,     50,   -45),
            new JudgementValue("miss",      0,     200,      0,  -100),
        };

        /// <summary>
        /// Returns the JudgementValue for a miss.
        /// </summary>
        /// <returns>A "miss" JudgmentValue</returns>
        static public JudgementValue GetMiss() => Judgements.Last();

        /// <summary>
        /// Finds the JudgementValue that has the name provided, then returns it.
        /// </summary>
        /// <param name="name">The JudgementValue to search for</param>
        /// <returns>The JudgementValue that is requested by the string.
        /// Returns null if there is no matching JudgementValue.</returns>
        static public JudgementValue GetJudgementValueByName(string name)
        {
            foreach (JudgementValue j in Judgements)
            {
                if (j.Name == name)
                {
                    return j;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the JudgementValue using the error provided, then returns it.
        /// </summary>
        /// <param name="error">The error of a hit. error = |arcTime - hitTime|</param>
        /// <returns>Returns the judgement that corresponds to the error amount.
        /// Returns null if the error is larger than all JudgementValues' Timing
        /// Windows.</returns>
        static public JudgementValue GetJudgementValueByError(int error)
        {
            JudgementValue result = null;

            if (error <= Judgements.Last().Judge)
            {
                for (int i = 0; i < Judgements.Count; i++)
                {
                    JudgementValue judgement = Judgements[i];

                    if (error <= judgement.Judge)
                    {
                        result = judgement;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get the JudgementValue above (better than) the provided JudgementValue.
        /// </summary>
        /// <param name="judgement"></param>
        /// <returns></returns>
        static public JudgementValue GetPreviousJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = JudgementValue.GetBaseJudgementValue();

            for (int i = 0; i < Judgements.Count; i++)
            {
                if (Judgements[i].Judge >= judgement.Judge)
                {
                    break;
                }
                else
                {
                    result = Judgements[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Get the JudgementValue below (worse than) the provided JudgementValue.
        /// </summary>
        /// <param name="judgement"></param>
        /// <returns></returns>
        static public JudgementValue GetNextJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = GetMiss();

            for (int i = 0; i < Judgements.Count; i++)
            {
                result = Judgements[i];

                if (result.Judge > judgement.Judge)
                {
                    break;
                }
            }

            return result;
        }
    }

    public class JudgementValue
    {
        // Judge Name
        public string Name { get; private set; }

        // Accuracy Reward
        public double Acc { get; private set; }

        // Outer range of timing window in ms.
        public int Judge { get; private set; }

        // Base Hidden Score Reward
        public int Score { get; private set; }

        // Hidden Combo Reward
        public int ComboAddition { get; private set; }

        // The color associated with this Judgementvalue
        public Color Color { get; private set; }

        // oooooooooooooooooooooooooooooooooooooooooooooo
        private static JudgementValue baseJudgementValue;

        /// <summary>
        /// A JudgementValue used to determine the judgement of a hit arc.
        /// </summary>
        /// <param name="name">The name of the Judgement. i.e. MAX, Good, Miss, etc.</param>
        /// <param name="acc">The accuracy of the Judgement; this affects the accuracy in game.</param>
        /// <param name="judge">The time (in ms) before or after a HitObject's time where this JudgementValue would apply to.</param>
        /// <param name="score">How much score is added to the total hidden score when this JudgementValue is obtained.</param>
        /// <param name="combo_addition">How much combo is added to the total hidden combo when this JudgementValue is obtained.</param>
        /// <param name="color">The color corresponding to this JudgementValue</param>
        public JudgementValue(string name, double acc, int judge, int score, int combo_addition)
        {
            Name = name;
            Acc = acc;
            Judge = judge;
            Score = score;
            ComboAddition = combo_addition;

            string colorName = char.ToUpper(name[0]) + name.Substring(1) + "Color";
            Color = Skin.GetConfigColor("judgements", "Colors", colorName);
        }

        /// <summary>
        /// Blank JudgementValue with a White color.
        /// </summary>
        public JudgementValue()
        {
            Name = "";
            Acc = 0;
            Judge = 0;
            Score = 0;
            ComboAddition = 0;
            Color = Color.White;
        }

        public static JudgementValue GetBaseJudgementValue()
        {
            if (baseJudgementValue == null)
            {
                baseJudgementValue = new JudgementValue();
            }
            return baseJudgementValue;
        }
    }
}
