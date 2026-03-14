using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLord.Core;

namespace PurpleLord.Entities
{
    public class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 48;
        public bool IsActive { get; set; } = true;
        
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        
        public GameObject(Vector2 position)
        {
            Position = position;
        }
        
        public virtual void Update(GameTime gameTime)
        {
            Position += Velocity;
        }
        
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            spriteBatch.Draw(rect, Color.Purple);
        }
    }
}
