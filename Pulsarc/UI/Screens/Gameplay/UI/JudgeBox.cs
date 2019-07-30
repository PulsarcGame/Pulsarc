using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class JudgeBox : Drawable
    {
        List<KeyValuePair<double, int>> toDraw;
        Dictionary<int, Judge> judges;

        /// <summary>
        ///  Container for displaying the obtained Judges in gameplay
        /// </summary>
        /// <param name="position">Where to place the JudgeBox on the screen</param>
        public JudgeBox(Vector2 position) : base(Skin.defaultTexture)
        {
            texture = null;

            this.position = position;

            judges = new Dictionary<int, Judge>();
            foreach (JudgementValue judge in Judgement.judgements)
            {
                judges.Add(judge.score,new Judge(judge.score, new Vector2(position.X, position.Y)));
            }

            toDraw = new List<KeyValuePair<double, int>>();
        }

        public void Add(double time, int judgeKey)
        {
            toDraw.Add(new KeyValuePair<double, int>(time, judgeKey));
        }

        public void Update(double time)
        {
            int judgeDisplayTimeMs = 100;

            for(int i = 0; i < toDraw.Count; i++)
            {
                if(toDraw[i].Key + judgeDisplayTimeMs < time)
                {
                    toDraw.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void Draw()
        {
            foreach(KeyValuePair<double, int> judge in toDraw)
            {
                judges[judge.Value].Draw();
            }
        }
    }
}
