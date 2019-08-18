using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class JudgeBox : Drawable
    {
        // A list of Judges to be drawn.
        List<KeyValuePair<double, int>> toDraw;

        // A list of each judge type.
        Dictionary<int, Judge> judges;

        /// <summary>
        /// Container for displaying the obtained Judges in gameplay
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

        /// <summary>
        /// Add a new Judgement to the "toDraw" list.
        /// </summary>
        /// <param name="time">The time of the judgement.</param>
        /// <param name="judgeKey">The base score of the judgement.</param>
        public void Add(double time, int judgeKey)
        {
            toDraw.Add(new KeyValuePair<double, int>(time, judgeKey));
        }

        /// <summary>
        /// Remove outdated judges.
        /// </summary>
        /// <param name="time">Current time.</param>
        public void Update(double time)
        {
            // TODO: Make this customizeable by the user
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

        /// <summary>
        /// Draw judges.
        /// </summary>
        public override void Draw()
        {
            foreach(KeyValuePair<double, int> judge in toDraw)
            {
                judges[judge.Value].Draw();
            }
        }
    }
}
