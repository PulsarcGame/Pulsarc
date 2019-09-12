using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class AccuracyMeter : Drawable
    {
        // Duration (in ms) each displayed hiterror bar stays on the meter
        int errorLifetime = 5000;

        // Width (in pixels) of a hiterror bar
        int errorBarWidth = 2;


        // A list of every error, the time it occured at and its judgement type
        List<KeyValuePair<double, int>> errors;

        // A list containing all of the HitError bar textures
        List<KeyValuePair<JudgementValue, Texture2D>> judges;
        
        // The width and height of this AccuracyMeter
        Vector2 size;

        // Used for keeping track of the time
        double lastTime;

        /// <summary>
        /// Create an AccuracyMeter that will display all recent errors during gameplay.
        /// </summary>
        /// <param name="position">Where this meter will be on the screen.</param>
        /// <param name="size">The width and height of this meter.</param>
        public AccuracyMeter(Vector2 position, Vector2 size) : base(Skin.defaultTexture, position, size, -1, Anchor.CenterBottom)
        {
            lastTime = 0;
            errors = new List<KeyValuePair<double, int>>();

            this.size = size;
            this.truePosition.X -= size.X / 2;
            this.truePosition.Y -= size.Y;

            judges = new List<KeyValuePair<JudgementValue, Texture2D>>();

            // Create the HitError textures for each judgements
            foreach(JudgementValue judgement in Judgement.judgements)
            {
                Texture2D judgeTexture = new Texture2D(Pulsarc.graphics.GraphicsDevice, errorBarWidth, (int) size.Y);
                Color[] judgeColors = new Color[errorBarWidth * (int)size.Y];

                for(int h = 0; h < (int) size.Y; h++)
                {
                    for (int l = 0; l < errorBarWidth; l++)
                    {
                        judgeColors[h * errorBarWidth + l] = judgement.color;
                    }
                }

                judgeTexture.SetData(judgeColors);
                judges.Add(new KeyValuePair<JudgementValue, Texture2D>(judgement,judgeTexture));
            }

            Texture = new Texture2D(Pulsarc.graphics.GraphicsDevice, (int) size.X, (int) size.Y);
            Color[] bar = new Color[(int) (size.X * size.Y)];


            // Build the meter according to judgement's colors and given size
            int lastX = -1;
            int x = 0;
            int y = 0;

            for (y = (int) size.Y/3; y < 2*size.Y / 3; y++)
            {
                lastX = 0;
                x = 0;
                for (int i = Judgement.judgements.Count - 2; i >= 0; i--)
                {
                    int judgePixelLength = getJudgePixelLength();

                    for (x = lastX; x - lastX < judgePixelLength; x++)
                    {
                        bar[(int) (y*size.X)+x] = Judgement.judgements[i].color;
                    }
                    lastX = x;
                }

                for (int i = 0; i < Judgement.judgements.Count - 1; i++)
                {
                    int judgePixelLength = getJudgePixelLength();

                    for (x = lastX; x - lastX < judgePixelLength; x++)
                    {
                        bar[(int)(y * size.X) + x] = Judgement.judgements[i].color;
                    }
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
            return (int)(size.X / 2 / (Judgement.judgements.Count - 1));
        }

        /// <summary>
        /// Used to determine where to draw each HitError on the bar.
        /// </summary>
        /// <param name="error">The error time of the hit.</param>
        /// <returns></returns>
        private Vector2 getErrorPixelPosition(int error)
        {
            JudgementValue judgement = Judgement.getErrorJudgementValue(Math.Abs(error));
         
            int baseX = (int)(truePosition.X + size.X / 2);
            int sectionLength = getJudgePixelLength();
            int sectionX = Judgement.judgements.IndexOf(judgement) * sectionLength;
            int timeSize = judgement.judge - Judgement.getPreviousJudgementValue(judgement).judge;

            float judgePos = 0;
            if(timeSize > 0)
            {
                judgePos = (judgement.judge - Math.Abs(error)) / (float) timeSize;
            }

            int errorX = (int) ((sectionX + (1-judgePos) * sectionLength) * Math.Sign(error));

            return new Vector2(baseX - errorX, truePosition.Y);
        }

        /// <summary>
        /// Add a new error to the errors KVP
        /// </summary>
        /// <param name="time">The time the hit happened.</param> 
        /// <param name="error">The error time of the hit.</param>
        public void addError(double time, int error)
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
            {
                if(LifeLeft(errors[i].Key, currentTime) <= 0)
                {
                    errors.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Get the JudgementValue:Texture KVP for the HitError bar given the current JudgementValue
        /// </summary>
        /// <param name="judgeVal">The JudgementValue used to find the KVP.</param>
        /// <returns>The JudgementValue:Texture KVP for a HitError bar.</returns>
        public KeyValuePair<JudgementValue, Texture2D> getJudge(JudgementValue judgeVal)
        {
            KeyValuePair<JudgementValue, Texture2D > judgePair = new KeyValuePair<JudgementValue, Texture2D>();

            foreach(KeyValuePair<JudgementValue, Texture2D> pair in judges)
            {
                if(pair.Key.judge == judgeVal.judge)
                {
                    judgePair = pair;
                    break;
                }
            }

            return judgePair;
        }

        /// <summary>
        /// Draw the meter and every HitError bar on the meter. Makes HitError bars transparent depending on how much "Life" they have left.
        /// </summary>
        public override void Draw()
        {
            Pulsarc.spriteBatch.Draw(Texture, position: position, rotation: rotation, origin: origin, color: Color.White * 0.3f);

            foreach(KeyValuePair<double, int> error in errors)
            {
                KeyValuePair<JudgementValue, Texture2D> judgeBar = getJudge(Judgement.getErrorJudgementValue(Math.Abs(error.Value)));

                Pulsarc.spriteBatch.Draw(judgeBar.Value, position: getErrorPixelPosition(error.Value), rotation: rotation, origin: origin, color: judgeBar.Key.color * (float) LifeLeft(error.Key, lastTime));
            }
        }
    }
}
