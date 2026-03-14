using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord.Core
{
    public class Camera2D
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1f;
        
        private Viewport _viewport;
        
        public Camera2D(Viewport viewport)
        {
            _viewport = viewport;
            Position = Vector2.Zero;
        }
        
        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));
        }
    }
}
