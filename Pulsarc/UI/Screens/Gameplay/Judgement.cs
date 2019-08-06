using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.UI.Screens.Gameplay
{
    static class Judgement
    {
        public static List<JudgementValue> judgements = new List<JudgementValue>()
        {
            //////////////// Judge equal to Stepmania J4 /////////////
            new JudgementValue("max",       1,      22,    320,    2,  new Color(216,   0, 255)),
            new JudgementValue("perfect",   1,      45,    300,    1,  new Color(240, 198,  35)),
            new JudgementValue("great",   2/3,      90,    200,    -8,  new Color( 52, 237,  92)),
            new JudgementValue("good",    1/3,     135,    100,   -15,  new Color( 60, 143, 222)),
            new JudgementValue("bad",     1/6,     180,     50,  -45,  new Color( 98,  97,  97)),
            new JudgementValue("miss",      0,     200,      0,  -100,  new Color(158,  28,  28)),
        };

        public static JudgementValue GetMiss()
        {
            return judgements.Last();
        }

        public static JudgementValue GetByName(string name)
        {
            foreach(JudgementValue j in judgements)
            {
                if (j.name == name) return j;
            }
            return null;
        }

        public static JudgementValue GetErrorJudgementValue(int error)
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

        public static JudgementValue GetPreviousJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = new JudgementValue("0",0,0,0,0,Color.White);

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

        public static JudgementValue GetNextJudgementValue(JudgementValue judgement)
        {
            JudgementValue result = GetMiss();
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

        public JudgementValue(string name, double acc, int judge, int score, int combo_addition, Color color)
        {
            this.name = name;
            this.acc = acc;
            this.judge = judge;
            this.score = score;
            this.color = color;
            this.combo_addition = combo_addition;
        }
    }
}