using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay
{
    static class Judgement
    {
        static public List<JudgementValue> judgements = new List<JudgementValue>()
        {
            //////////////// Judge equal to Stepmania J4 /////////////
            new JudgementValue(1,       22,     320,    new Color(216,0,255)),      // MAX
            new JudgementValue(1,       45,     300,    new Color(240,198,35)),      // Perfect
            new JudgementValue(2/3,     90,     200,    new Color(52,237,92)),       // Great
            new JudgementValue(1/3,     135,    100,    new Color(60,143,222)),        // Good
            new JudgementValue(1/6,     180,    50,     new Color(98,97,97)),        // Bad
            new JudgementValue(0,       200,    0,      new Color(158,28,28)),         // Miss
        };


        static public KeyValuePair<double, int> getErrorJudgement(int error)
        {
            KeyValuePair<double, int> result = new KeyValuePair<double, int>(-1, -1);
            if (error < judgements.Last().judge)
            {
                bool notJudged = true;

                for (int i = 0; i < judgements.Count && notJudged; i++)
                {
                    JudgementValue judgement = judgements[i];

                    if (error < judgement.judge)
                    {
                        result = new KeyValuePair<double, int>(judgement.acc, judgement.score);
                        notJudged = false;
                    }
                }

                if (notJudged)
                {
                    result = new KeyValuePair<double, int>(0, 0);
                }
            }

            return result;
        }

        static public JudgementValue getMiss()
        {
            return judgements.Last();
        }

        static public JudgementValue getErrorJudgementValue(int error)
        {
            JudgementValue result = getMiss();
            if (error < judgements.Last().judge)
            {
                for (int i = 0; i < judgements.Count - 1; i++)
                {
                    JudgementValue judgement = judgements[i];

                    if (error < judgement.judge)
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
        public Color color;

        public JudgementValue(double acc, int judge, int score, Color color)
        {
            this.acc = acc;
            this.judge = judge;
            this.score = score;
            this.color = color;
        }
    }
}
