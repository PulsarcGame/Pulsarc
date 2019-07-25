using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class JudgeBox : Drawable
    {
        List<KeyValuePair<long, int>> toDraw;
        public JudgeBox(Vector2 position) : base(Skin.defaultTexture)
        {
            texture = null;

            this.position = position;

            toDraw = new List<KeyValuePair<long, int>>();
        }

        public void Add(long time, int judgeKey)
        {
            toDraw.Add(new KeyValuePair<long, int>(time, judgeKey));
        }

        public void Update(long time)
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
            foreach(KeyValuePair<long,int> judge in toDraw)
            {
                Vector2 pos = new Vector2(position.X - Skin.judges[judge.Value].Width / 2, position.Y - Skin.judges[judge.Value].Height / 2);
                Pulsarc.spriteBatch.Draw(Skin.judges[judge.Value], position: pos);
            }
        }
    }
}
