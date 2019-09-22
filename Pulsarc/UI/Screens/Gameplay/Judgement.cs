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
        public static readonly List<JudgementValue> judgements = new List<JudgementValue>()
        {
            //////////////// Judge equal to Stepmania J4 /////////////
            new JudgementValue("max",       1,      22,    320,     2),
            new JudgementValue("perfect",   1,      45,    300,     1),
            new JudgementValue("great",   2/3,      90,    200,    -8),
            new JudgementValue("good",    1/3,     135,    100,   -15),
            new JudgementValue("bad",     1/6,     180,     50,   -45),
            new JudgementValue("miss",      0,     200,      0,  -100),
        };

        /// <summary>
        /// Returns the JudgementValue for a miss.
        /// </summary>
        /// <returns>A "miss" JudgmentValue</returns>
        static public JudgementValue getMiss()
        {
            return judgements.Last();
        }

        /// <summary>
        /// Finds the JudgementValue that has the name provided, then returns it.
        /// </summary>
        /// <param name="name">The JudgementValue to search for</param>
        /// <returns>The JudgementValue that is requested by the string.
        /// Returns null if there is no matching JudgementValue.</returns>
        static public JudgementValue getByName(string name)
        {
            foreach(JudgementValue j in judgements)
            {
                if (j.name == name) return j;
            }
            return null;
        }

        /// <summary>
        /// Finds the JudgementValue using the error provided, then returns it.
        /// </summary>
        /// <param name="error">The error of a hit. error = |arcTime - hitTime|</param>
        /// <returns>Returns the judgement that corresponds to the error amount.
        /// Returns null if the error is larger than all JudgementValues.</returns>
        static public JudgementValue getErrorJudgementValue(int error)
        {
            JudgementValue result = null;
            if (error <= judgements.Last().judge)
            {
                for (int i = 0; i < judgements.Count; i++)
                {
                    JudgementValue judgement = judgements[i];

                    if (error <= judgement.judge)
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
        static public JudgementValue getPreviousJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = null;

            for (int i = 0; i < judgements.Count; i++)
            {
                if (judgements[i].judge >= judgement.judge)
                {
                    break;
                } else
                {
                    result = judgements[i];
                }
            }

            return result;
        }

        static public JudgementValue getNextJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = getMiss();
            for (int i = 0; i < judgements.Count; i++)
            {
                result = judgements[i];

                if (result.judge > judgement.judge)
                {
                    break;
                }
            }

            return result;
        }
    }

    public class JudgementValue {

        public string name;
        public double acc;
        public int judge;
        public int score;
        public int combo_addition;
        public Color color;

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
            this.name = name;
            this.acc = acc;
            this.judge = judge;
            this.score = score;
            this.combo_addition = combo_addition;

            string colorName = char.ToUpper(name[0]) + name.Substring(1) + "Color";
            color = Skin.getConfigColor("judgements", "Colors", colorName);
        }
    }
}
