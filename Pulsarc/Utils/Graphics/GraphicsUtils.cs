using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System.IO;
using System.Linq;
using Wobble.Logging;

namespace Pulsarc.Utils.Graphics
{
    class GraphicsUtils
    {
        public static Texture2D CreateSolidColorTexture(int width, int height, Color paint)
        {
            Texture2D texture = new Texture2D(Pulsarc.Graphics.GraphicsDevice, width, height);

            Color[] data = new Color[width * height];

            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                data[pixel] = paint;
            }

            texture.SetData(data);

            return texture;
        }

        public static Texture2D LoadFileTexture(string path)
        {
            Texture2D texture;

            try
            {
                texture = Texture2D.FromStream(Pulsarc.Graphics.GraphicsDevice, File.Open(path, FileMode.Open));
            }
            catch
            {
                PulsarcLogger.Error($"Failed to load {path}", LogType.Runtime);
                texture = Skin.DefaultTexture;
            }

            return texture;
        }
    }
}
