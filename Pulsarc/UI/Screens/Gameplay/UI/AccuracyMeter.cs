using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class AccuracyMeter : Drawable
    {
        int errorLifetime = 5000;
        int errorBarWidth = 2;

        List<KeyValuePair<long, int>> errors;
        List<KeyValuePair<JudgementValue, Texture2D>> judges;
        Vector2 size;

        long lastTime;

        public AccuracyMeter(Vector2 position, Vector2 size) : base(Skin.defaultTexture, position, size)
        {
            lastTime = 0;
            errors = new List<KeyValuePair<long, int>>();
            this.size = size;
            this.position.X -= size.X / 2;
            this.position.Y -= size.Y;

            judges = new List<KeyValuePair<JudgementValue, Texture2D>>();
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

            texture = new Texture2D(Pulsarc.graphics.GraphicsDevice, (int) size.X, (int) size.Y);
            Color[] bar = new Color[(int) (size.X * size.Y)];


            // Build the bar according to judgement's colors and given size
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

            texture.SetData(bar);
        }

        private int getJudgePixelLength()
        {
            return (int)(size.X / 2 / (Judgement.judgements.Count - 1));
        }

        private Vector2 getErrorPixelPosition(int error)
        {
            JudgementValue judgement = Judgement.getErrorJudgementValue(Math.Abs(error));
            
            int baseX = (int)(position.X + size.X / 2);
            int sectionLength = getJudgePixelLength();
            int sectionX = Judgement.judgements.IndexOf(judgement) * sectionLength;
            int timeSize = Judgement.getNextJudgementValue(judgement).judge - judgement.judge;

            float judgePos = 0;
            if(timeSize > 0)
            {
                judgePos = (judgement.judge - Math.Abs(error)) / (float) timeSize;
            }

            int errorX = (int) ((sectionX + judgePos * sectionLength) * Math.Sign(error));

            return new Vector2(baseX - errorX, position.Y);
        }

        public void addError(long time, int error)
        {
            errors.Add(new KeyValuePair<long, int>(time, error));
        }

        public float LifeLeft(long time, long currentTime)
        {
            return (time - currentTime + errorLifetime) / (float) errorLifetime;
        }

        public void Update(long time)
        {
            lastTime = time;
            for (int i = 0; i < errors.Count; i++)
            {
                if(LifeLeft(errors[i].Key, time) <= 0)
                {
                    errors.RemoveAt(i);
                    i--;
                }
            }
        }

        public KeyValuePair<JudgementValue, Texture2D> getJudge(JudgementValue judge)
        {
            KeyValuePair<JudgementValue, Texture2D > judgePair = new KeyValuePair<JudgementValue, Texture2D>();

            foreach(KeyValuePair<JudgementValue, Texture2D> pair in judges)
            {
                if(pair.Key.judge == judge.judge)
                {
                    judgePair = pair;
                    break;
                }
            }

            return judgePair;
        }

        public override void Draw()
        {
            Pulsarc.spriteBatch.Draw(texture, position: position, rotation: rotation, origin: origin, color: Color.White * 0.3f);

            foreach(KeyValuePair<long,int> error in errors)
            {
                KeyValuePair<JudgementValue, Texture2D> judgeBar = getJudge(Judgement.getErrorJudgementValue(Math.Abs(error.Value)));

                Pulsarc.spriteBatch.Draw(judgeBar.Value, position: getErrorPixelPosition(error.Value), rotation: rotation, origin: origin, color: judgeBar.Key.color * LifeLeft(error.Key, lastTime));
            }
        }
    }
}
