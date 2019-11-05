using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Wobble.Logging;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class AccuracyMeter : Drawable
    {
        // Duration (in ms) each displayed hiterror bar stays on the meter
        private int errorLifetime = 5000;

        // Width (in pixels) of a hiterror bar
        private int errorBarWidth = 2;

        public Vector2 Size { get; private set; }

        // A list of every error, the time it occured at and its judgement type
        private List<KeyValuePair<double, int>> errors;

        // A list containing all of the HitError bar textures
        private List<KeyValuePair<JudgementValue, Texture2D>> judges;

        // Used for keeping track of the time
        private double lastTime;

        /// <summary>
        /// Create an AccuracyMeter that will display all recent errors during gameplay.
        /// </summary>
        /// <param name="position">Where this meter will be on the screen.</param>
        /// <param name="size">The width and height of this meter.</param>
        /// <param name="anchor">The anchor for this Meter</param>
        public AccuracyMeter(Vector2 position, Vector2 size, Anchor anchor = Anchor.CenterBottom)
            : base(Skin.DefaultTexture, position, -1, anchor)
        {
            Size = size;

            lastTime = 0;
            errors = new List<KeyValuePair<double, int>>();

            judges = new List<KeyValuePair<JudgementValue, Texture2D>>();

            CreateHitErrorTextures();

            BuildMeter();

            Resize(Size);
            ChangePosition(position);
        }

        /// <summary>
        /// Create the HitError textures for each judgements
        /// </summary>
        private void CreateHitErrorTextures()
        {
            foreach (JudgementValue judgement in Judgement.Judgements)
            {
                Texture2D judgeTexture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, errorBarWidth, (int)Size.Y);
                Color[] judgeColors = new Color[errorBarWidth * (int)Size.Y];

                for (int h = 0; h < (int)Size.Y; h++)
                    for (int l = 0; l < errorBarWidth; l++)
                        judgeColors[h * errorBarWidth + l] = judgement.Color;

                judgeTexture.SetData(judgeColors);
                judges.Add(new KeyValuePair<JudgementValue, Texture2D>(judgement, judgeTexture));
            }
        }
        /// <summary>
        /// Build the meter according to judgement's colors and given size
        /// </summary>
        private void BuildMeter()
        {
            Texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, (int)Size.X, (int)Size.Y);
            Color[] bar = new Color[(int)(Size.X * Size.Y)];

            int lastX = -1;
            int x = 0;
            int y = 0;

            for (y = (int)Size.Y / 3; y < 2 * Size.Y / 3; y++)
            {
                lastX = 0;
                x = 0;

                for (int i = Judgement.Judgements.Count - 2; i >= 0; i--)
                {
                    int judgePixelLength = getJudgePixelLength();

                    for (x = lastX; x - lastX < judgePixelLength; x++)
                        bar[(int)(y * Size.X) + x] = Judgement.Judgements[i].Color;

                    lastX = x;
                }

                for (int i = 0; i < Judgement.Judgements.Count - 1; i++)
                {
                    int judgePixelLength = getJudgePixelLength();

                    for (x = lastX; x - lastX < judgePixelLength; x++)
                        bar[(int)(y * Size.X) + x] = Judgement.Judgements[i].Color;

                    lastX = x;
                }
            }

            Texture.SetData(bar);
        }

        /// <summary>
        /// The size of each Judge section on the AccuracyMeter
        /// </summary>
        private int getJudgePixelLength()
        {
            return (int)(Size.X / 2 / (Judgement.Judgements.Count - 1));
        }

        /// <summary>
        /// Used to determine where to draw each HitError on the bar.
        /// </summary>
        /// <param name="error">The error time of the hit.</param>
        /// <returns></returns>
        private Vector2 GetErrorPixelPosition(int error)
        {
            JudgementValue judgement = Judgement.GetJudgementValueByError(Math.Abs(error));
         
            int baseX = (int)(truePosition.X + Size.X / 2);
            int sectionLength = getJudgePixelLength();
            int sectionX = Judgement.Judgements.IndexOf(judgement) * sectionLength;
            int timeSize = judgement.Judge - Judgement.GetPreviousJudgementValue(judgement).Judge;

            float judgePos = 0;

            if (timeSize > 0)
                judgePos = (judgement.Judge - Math.Abs(error)) / (float) timeSize;

            int errorX = (int) ((sectionX + (1-judgePos) * sectionLength) * Math.Sign(error));

            return new Vector2(baseX - errorX, truePosition.Y);
        }

        /// <summary>
        /// Add a new error to the errors KVP
        /// </summary>
        /// <param name="time">The time the hit happened.</param> 
        /// <param name="error">The error time of the hit.</param>
        public void AddError(double time, int error)
        {
            errors.Add(new KeyValuePair<double, int>(time, error));
        }

        /// <summary>
        /// How much of a HitError bar's "life" has it gone through so far.
        /// </summary>
        /// <param name="time">The time a HitError bar was added.</param>
        /// <param name="currentTime">The current time.</param>
        /// <returns>The percentage of time left until the HitError bar would be removed.</returns>
        public double LifeLeft(double time, double currentTime)
        {
            return (time - currentTime + errorLifetime) / errorLifetime;
        }

        /// <summary>
        /// Clear all expired HitErrors.
        /// </summary>
        /// <param name="currentTime">The Current Time.</param>
        public void Update(double currentTime)
        {
            lastTime = currentTime;

            for (int i = 0; i < errors.Count; i++)
                if (LifeLeft(errors[i].Key, currentTime) <= 0)
                {
                    errors.RemoveAt(i);
                    i--;
                }
        }

        /// <summary>
        /// Get the JudgementValue:Texture KVP for the HitError bar given the current JudgementValue
        /// </summary>
        /// <param name="judgeVal">The JudgementValue used to find the KVP.</param>
        /// <returns>The JudgementValue:Texture KVP for a HitError bar.</returns>
        public KeyValuePair<JudgementValue, Texture2D> GetJudge(JudgementValue judgeVal)
        {
            KeyValuePair<JudgementValue, Texture2D > judgePair = new KeyValuePair<JudgementValue, Texture2D>();

            foreach (KeyValuePair<JudgementValue, Texture2D> pair in judges)
                if (pair.Key.Judge == judgeVal.Judge)
                {
                    judgePair = pair;
                    break;
                }

            return judgePair;
        }

        /// <summary>
        /// Draw the meter and every HitError bar on the meter. Makes HitError bars transparent depending on how much "Life" they have left.
        /// </summary>
        public override void Draw()
        {
            Pulsarc.SpriteBatch.Draw(Texture, position: truePosition, rotation: Rotation, origin: origin, color: Color.White * 0.3f);

            foreach (KeyValuePair<double, int> error in errors)
            {
                KeyValuePair<JudgementValue, Texture2D> judgeBar = GetJudge(Judgement.GetJudgementValueByError(Math.Abs(error.Value)));

                Pulsarc.SpriteBatch.Draw(judgeBar.Value, position: GetErrorPixelPosition(error.Value), rotation: Rotation, origin: origin, color: judgeBar.Key.Color * (float) LifeLeft(error.Key, lastTime));
            }
        }
    }
}
