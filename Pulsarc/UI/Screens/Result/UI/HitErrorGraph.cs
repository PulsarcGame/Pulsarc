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
        // Dimensions of the graph
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        // Data on the graph
        public List<KeyValuePair<double, int>> Hits { get; private set; }

        // The time of the last hit
        private double maxTime;

        /// <summary>
        /// A graph that shows all the hits of a play on the Result Screen.
        /// </summary>
        /// <param name="position">The position of the Graph</param>
        /// <param name="width">The width of the Graph</param>
        /// <param name="height">The height of the Graph</param>
        /// <param name="hits">A list of all hits, KVP[time, judgescore]</param>
        public HitErrorGraph(Vector2 position, int width, int height, List<KeyValuePair<double, int>> hits, Anchor anchor = Anchor.TopLeft) : base(Skin.DefaultTexture, position, anchor: anchor)
        {
            Width = width;
            Height = height;
            Hits = hits;

            Texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, width, height);
            Color[] graphBG = new Color[width * height];

            JudgementValue miss = Judgement.GetMiss();

            // Draw the graph
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    graphBG[(y * width) + x] = Judgement.GetJudgementValueByError((int) Math.Abs(((y - (height / 2)) * miss.Judge / (float) height * 2))).Color * 0.3f;

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

            // Draw the hits
            if (hits.Count > 0)
            {
                maxTime = 0;

                foreach (KeyValuePair<double, int> hit in hits)
                    maxTime = maxTime < hit.Key ? hit.Key : maxTime;

                maxTime += 4000;

                foreach (KeyValuePair<double, int> hit in hits)
                {
                    KeyValuePair<Vector2, Color> info = getHitInfo(hit);

                    for (int yp = -1; yp < 2; yp++)
                    {
                        for (int xp = -1; xp < 2; xp++)
                        {
                            int pos = (int)((int)(info.Key.Y + yp) * width + (info.Key.X + xp));

                            if (pos >= 0 && pos < width * height)
                                graphBG[pos] = info.Value;
                        }
                    }
                }
            }

            Texture.SetData(graphBG);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private KeyValuePair<Vector2, Color> getHitInfo(KeyValuePair<double, int> hit)
        {
            return new KeyValuePair<Vector2, Color>(
                new Vector2
                (
                    (float) ((2000+ hit.Key) / maxTime * Width),
                    (-hit.Value / (float) Judgement.GetMiss().Judge * Height / 2f) + Height / 2
                ),
                Judgement.GetJudgementValueByError(Math.Abs(hit.Value)).Color);
        }

        public override void Draw()
        {
            Pulsarc.SpriteBatch.Draw(Texture, position: TruePosition, rotation: Rotation, origin: origin, color: Color.White);

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
