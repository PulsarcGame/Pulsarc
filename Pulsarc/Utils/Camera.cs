using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsarc.Utils
{
    public class Camera
    {
        private const float ZoomUpperLimit = 1.5f;
        private const float ZoomLowerLimit = .5f;

        private Matrix _transform;
        private Vector2 _pos;
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private readonly int _worldWidth;
        private readonly int _worldHeight;

        public Camera(Viewport viewport, int worldWidth,
           int worldHeight, float initialZoom)
        {
            Zoom = initialZoom;
            Rotation = 0.0f;
            _pos = Vector2.Zero;
            _viewportWidth = viewport.Width;
            _viewportHeight = viewport.Height;
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
        }

        #region Properties

        private float Zoom { get; set; }

        private float Rotation { get; set; }

        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        public Vector2 Pos
        {
            get => _pos;
            set
            {
                float leftBarrier = _viewportWidth *
                       .5f / Zoom;
                float rightBarrier = _worldWidth -
                       _viewportWidth * .5f / Zoom;
                float topBarrier = _worldHeight -
                       _viewportHeight * .5f / Zoom;
                float bottomBarrier = _viewportHeight *
                       .5f / Zoom;
                _pos = value;
                if (_pos.X < leftBarrier)
                    _pos.X = leftBarrier;
                if (_pos.X > rightBarrier)
                    _pos.X = rightBarrier;
                if (_pos.Y > topBarrier)
                    _pos.Y = topBarrier;
                if (_pos.Y < bottomBarrier)
                    _pos.Y = bottomBarrier;
            }
        }

        #endregion

        public Matrix GetTransformation()
        {
            _transform =
               Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
               Matrix.CreateRotationZ(Rotation) *
               Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
               Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f,
                   _viewportHeight * 0.5f, 0));

            return _transform;
        }
    }
}
