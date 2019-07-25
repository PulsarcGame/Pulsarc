using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Judgement
    {
        static public List<JudgementValue> judgements = new List<JudgementValue>()
        {
            //////////////// Judge equal to Stepmania J4 /////////////
            new JudgementValue(  1,      22,    320,    2,  new Color(216,   0, 255)),      // MAX
            new JudgementValue(  1,      45,    300,    1,  new Color(240, 198,  35)),      // Perfect
            new JudgementValue(2/3,      90,    200,    0,  new Color( 52, 237,  92)),       // Great
            new JudgementValue(1/3,     135,    100,   -4,  new Color( 60, 143, 222)),        // Good
            new JudgementValue(1/6,     180,     50,  -20,  new Color( 98,  97,  97)),        // Bad
            new JudgementValue(  0,     200,      0,  -50,  new Color(158,  28,  28)),         // Miss
        };

        static public JudgementValue getMiss()
        {
            return judgements.Last();
        }

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

    class JudgementValue {

        public double acc;
        public int judge;
        public int score;
        public int combo_addition;
        public Color color;

        public JudgementValue(double acc, int judge, int score, int combo_addition, Color color)
        {
            this.acc = acc;
            this.judge = judge;
            this.score = score;
            this.color = color;
        }
    }
}
