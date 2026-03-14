// ============================================================================
// GameObject.cs - Базовый класс игрового объекта / Base game object class
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Entities
{
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool IsActive { get; set; } = true;
        public string Tag { get; set; } = "";

        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - Width / 2),
            (int)(Position.Y - Height / 2),
            (int)Width,
            (int)Height);

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public bool Intersects(GameObject other)
        {
            return Bounds.Intersects(other.Bounds);
        }

        public bool Intersects(Rectangle rect)
        {
            return Bounds.Intersects(rect);
        }
    }
}
