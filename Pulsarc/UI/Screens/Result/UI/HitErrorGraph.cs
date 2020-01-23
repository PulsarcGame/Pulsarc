using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.UI.Screens.Result.UI
{
    class HitErrorGraph : Drawable
    {
        // Dimensions of the graph
        protected int Width { get; private set; }
        private int Height { get; set; }
        
        // Data on the graph
        private List<KeyValuePair<double, int>> Hits { get; set; }

        // The time of the last hit
        public readonly double MaxTime;

        /// <summary>
        /// A graph that shows all the hits of a play on the Result Screen.
        /// </summary>
        /// <param name="position">The position of the Graph</param>
        /// <param name="width">The width of the Graph</param>
        /// <param name="height">The height of the Graph</param>
        /// <param name="hits">A list of all hits, KVP[time, judgescore]</param>
        /// <param name="anchor"></param>
        public HitErrorGraph(Vector2 position, int width, int height, List<KeyValuePair<double, int>> hits, Anchor anchor = Anchor.TopLeft) : base(Skin.DefaultTexture, position, anchor: anchor)
        {
            Width = width;
            Height = height;
            Hits = hits;

            Texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, width, height);
            Color[] graphBg = new Color[width * height];

            JudgementValue miss = Judgement.GetMiss();

            // Draw the graph
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    graphBg[y * width + x] = Judgement.GetJudgementValueByError((int) Math.Abs((y - height / 2) * miss.Judge / (float) height * 2)).Color * 0.3f;

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
                MaxTime = 0;

                foreach (KeyValuePair<double, int> hit in hits)
                    MaxTime = MaxTime < hit.Key ? hit.Key : MaxTime;

                MaxTime += 4000;

                foreach (var info in hits.Select(hit => GetHitInfo(hit)))
                {
                    for (int yp = -1; yp < 2; yp++)
                    {
                        for (int xp = -1; xp < 2; xp++)
                        {
                            int pos = (int)((int)(info.Key.Y + yp) * width + (info.Key.X + xp));

                            if (pos >= 0 && pos < width * height)
                                graphBg[pos] = info.Value;
                        }
                    }
                }
            }

            Texture.SetData(graphBg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private KeyValuePair<Vector2, Color> GetHitInfo(KeyValuePair<double, int> hit)
        {
            return new KeyValuePair<Vector2, Color>(
                new Vector2
                (
                    (float) ((2000+ hit.Key) / MaxTime * Width),
                    -hit.Value / (float) Judgement.GetMiss().Judge * Height / 2f + Height / 2
                ),
                Judgement.GetJudgementValueByError(Math.Abs(hit.Value)).Color);
        }

        public override void Draw()
        {
            Pulsarc.SpriteBatch.Draw(Texture, position: TruePosition, rotation: Rotation, origin: Origin, color: Color.White);

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
