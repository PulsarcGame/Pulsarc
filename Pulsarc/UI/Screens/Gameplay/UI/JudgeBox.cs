using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class JudgeBox : Drawable
    {
        // The stats of the current Judge being drawn
        private int judgeKey = -1;
        private double time = 0;

        // A list of each judge type.
        private Dictionary<int, Judge> judges;

        /// <summary>
        /// Container for displaying the obtained Judges in gameplay
        /// </summary>
        /// <param name="position">Where to place the JudgeBox on the screen</param>
        public JudgeBox(Vector2 position, Anchor anchor = Anchor.Center)
            : base(Skin.DefaultTexture, position, anchor: anchor)
        {
            Texture = null;

            judges = new Dictionary<int, Judge>();

            foreach (JudgementValue judge in Judgement.Judgements)
                judges.Add(judge.Score,new Judge(judge.Score, new Vector2(position.X, position.Y)));
        }

        /// <summary>
        /// Add a new Judgement to be drawn.
        /// </summary>
        /// <param name="time">The time of the judgement.</param>
        /// <param name="judgeKey">The base score of the judgement.</param>
        public void Add(double time, int judgeKey)
        {
            this.time = time;
            this.judgeKey = judgeKey;
        }

        /// <summary>
        /// Remove outdated judges.
        /// </summary>
        /// <param name="time">Current time.</param>
        public void Update(double time)
        {
            // TODO: Make this customizeable by the user
            int judgeDisplayTimeMs = 100;
           
            if(this.time + judgeDisplayTimeMs < time)
                judgeKey = -1;
        }

        /// <summary>
        /// Draw the most recent judge.
        /// </summary>
        public override void Draw()
        {
            if (judgeKey >= 0)
                judges[judgeKey].Draw();
        }
    }
}
