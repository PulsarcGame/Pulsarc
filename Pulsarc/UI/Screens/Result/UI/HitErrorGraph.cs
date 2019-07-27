using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Result.UI
{
    class HitErrorGraph : Drawable
    {
        int width;
        int height;
        List<KeyValuePair<double, int>> hits;
        double max_time;

        public HitErrorGraph(Vector2 position, int width, int height, List<KeyValuePair<double, int>> hits) : base(Skin.defaultTexture, position)
        {
            this.width = width;
            this.height = height;
            this.hits = hits;

            texture = new Texture2D(Pulsarc.graphics.GraphicsDevice, width, height);
            Color[] graphBG = new Color[width * height];

            JudgementValue miss = Judgement.getMiss();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    graphBG[(y * width) + x] = Judgement.getErrorJudgementValue((int) Math.Abs(((y - (height / 2)) * miss.judge / (float) height * 2))).color * 0.3f;
                }
            }

            /*
            hits = new List<KeyValuePair<long, int>>();
            int t = 1;
            for (int j = -Judgement.getMiss().judge; j <= Judgement.getMiss().judge; j++)
            {
                hits.Add(new KeyValuePair<long, int>(t++, j));
            }
            for (int j = Judgement.getMiss().judge; j >= -Judgement.getMiss().judge; j--)
            {
                hits.Add(new KeyValuePair<long, int>(t++, j));
            }
            */

            if (hits.Count > 0)
            {
                max_time = hits[hits.Count - 1].Key;
                foreach (KeyValuePair<double, int> hit in hits)
                {
                    KeyValuePair<Vector2, Color> info = getHitInfo(hit);
                    for (int yp = -1; yp < 2; yp++)
                    {
                        for (int xp = -1; xp < 2; xp++)
                        {
                            int pos = (int)((info.Key.Y + yp) * width + (info.Key.X + xp));
                            if (pos >= 0 && pos < width * height)
                            {
                                graphBG[pos] = info.Value;
                            }
                        }
                    }
                }
            }

            texture.SetData(graphBG);
        }

        private KeyValuePair<Vector2, Color> getHitInfo(KeyValuePair<double, int> hit)
        {
            return new KeyValuePair<Vector2, Color>(new Vector2((float) (hit.Key / max_time * width), (hit.Value / (float) Judgement.getMiss().judge * height / 2f) + height / 2), Judgement.getErrorJudgementValue(Math.Abs(hit.Value)).color);
        }

        public override void Draw()
        {
            Pulsarc.spriteBatch.Draw(texture, position: position, rotation: rotation, origin: origin, color: Color.White);

            /*
            foreach (KeyValuePair<long, Double> hit error in hits)
            {
                KeyValuePair<JudgementValue, Texture2D> judgeBar = getJudge(Judgement.getErrorJudgementValue(Math.Abs(error.Value)));

                Pulsarc.spriteBatch.Draw(judgeBar.Value, position: getErrorPixelPosition(error.Value), rotation: rotation, origin: origin, color: judgeBar.Key.color * LifeLeft(error.Key, lastTime));
            }
            */
        }
    }
}
