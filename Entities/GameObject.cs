// ============================================================================
// GameObject.cs - Базовый класс игрового объекта / Base game object class
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord.Entities
{
    /// <summary>
    /// Базовый класс для всех игровых объектов.
    /// Base class for all game objects.
    /// </summary>
    public abstract class GameObject
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public bool IsActive { get; set; } = true;
        public string Tag { get; set; } = "";
        
        /// <summary>
        /// Получает прямоугольник границ объекта.
        /// Gets the bounding rectangle of the object.
        /// </summary>
        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - Width / 2),
            (int)(Position.Y - Height / 2),
            (int)Width,
            (int)Height);
        
        /// <summary>
        /// Конструктор по умолчанию.
        /// Default constructor.
        /// </summary>
        protected GameObject()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Width = 32;
            Height = 32;
        }
        
        /// <summary>
        /// Конструктор с позицией.
        /// Constructor with position.
        /// </summary>
        protected GameObject(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Width = 32;
            Height = 32;
        }
        
        /// <summary>
        /// Обновление объекта. Вызывается каждый кадр.
        /// Update object. Called every frame.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        /// <summary>
        /// Отрисовка объекта.
        /// Draw object.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsActive) return;
        }
        
        /// <summary>
        /// Проверяет пересечение с другим объектом.
        /// Checks intersection with another object.
        /// </summary>
        public bool Intersects(GameObject other)
        {
            return Bounds.Intersects(other.Bounds);
        }
        
        /// <summary>
        /// Событие при смерти объекта.
        /// Event when object dies.
        /// </summary>
        public event Action<GameObject> OnDestroyed;
        
        /// <summary>
        /// Уничтожить объект.
        /// Destroy the object.
        /// </summary>
        public void Destroy()
        {
            IsActive = false;
            OnDestroyed?.Invoke(this);
        }
    }
}
