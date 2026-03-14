// ============================================================================
// KnowledgeItem.cs - Предмет знания / Knowledge item
// Собираемые предметы - технологии, свитки, книги
// Collectible items - technologies, scrolls, books
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Entities.Objects
{
    public enum KnowledgeType
    {
        Frontend,     // HTML, CSS, JS, React
        Backend,      // Go, Python, Node.js
        Database,     // SQL, NoSQL, Redis
        DevOps,       // Docker, Kubernetes, CI/CD
        NeuralNet,    // ML, AI, TensorFlow
        Other         // Разное / Miscellaneous
    }

    public class KnowledgeItem : GameObject
    {
        private Texture2D _texture;
        private KnowledgeType _type;
        private string _technologyName;
        private string _description;
        private float _rotationSpeed = 2f;
        private float _bobSpeed = 1.5f;
        private float _bobAmplitude = 10f;
        private float _initialY;
        private float _timer;
        private bool _isCollected = false;

        public KnowledgeItem(Vector2 position, KnowledgeType type, 
            string technologyName, string description) : base(position)
        {
            _type = type;
            _technologyName = technologyName;
            _description = description;
            Width = 32;
            Height = 32;
            _initialY = position.Y;
            Tag = "Knowledge";
        }

        public override void Update(GameTime gameTime)
        {
            if (_isCollected) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += delta;

            // Анимация парения / Floating animation
            Position.Y = _initialY + (float)Math.Sin(_timer * _bobSpeed) * _bobAmplitude;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_isCollected) return;

            Color color = GetTypeColor(_type);
            
            if (_texture != null)
            {
                spriteBatch.Draw(
                    _texture,
                    Position,
                    null,
                    color,
                    _timer * _rotationSpeed,
                    new Vector2(Width / 2, Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                // Placeholder - кристалл / Placeholder - crystal
                DrawCrystalPlaceholder(spriteBatch, color);
            }
        }

        private void DrawCrystalPlaceholder(SpriteBatch spriteBatch, Color color)
        {
            Vector2 center = Position;
            float size = Width / 2;

            // Рисуем ромб / Draw diamond
            Vector2[] points = new Vector2[]
            {
                new Vector2(center.X, center.Y - size),      // Top
                new Vector2(center.X + size * 0.7f, center.Y),  // Right
                new Vector2(center.X, center.Y + size),      // Bottom
                new Vector2(center.X - size * 0.7f, center.Y)   // Left
            };

            // Простая отрисовка заполнением / Simple fill drawing
            for (int y = (int)(center.Y - size); y <= center.Y; y++)
            {
                float progress = (y - (center.Y - size)) / size;
                float halfWidth = size * 0.7f * progress;
                Rectangle rect = new Rectangle(
                    (int)(center.X - halfWidth),
                    y,
                    (int)(halfWidth * 2),
                    1);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, color * (1 - progress * 0.5f));
            }

            for (int y = (int)center.Y; y <= center.Y + size; y++)
            {
                float progress = (y - center.Y) / size;
                float halfWidth = size * 0.7f * (1 - progress);
                Rectangle rect = new Rectangle(
                    (int)(center.X - halfWidth),
                    y,
                    (int)(halfWidth * 2),
                    1);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, color * (0.5f + progress * 0.5f));
            }
        }

        private Color GetTypeColor(KnowledgeType type)
        {
            switch (type)
            {
                case KnowledgeType.Frontend:
                    return new Color(255, 165, 0);    // Orange
                case KnowledgeType.Backend:
                    return new Color(0, 150, 255);    // Blue
                case KnowledgeType.Database:
                    return new Color(0, 200, 100);    // Green
                case KnowledgeType.DevOps:
                    return new Color(150, 50, 200);   // Purple
                case KnowledgeType.NeuralNet:
                    return new Color(255, 50, 150);   // Pink
                default:
                    return Color.Gold;
            }
        }

        public void Collect()
        {
            _isCollected = true;
            OnCollect?.Invoke(this);
        }

        public event Action<KnowledgeItem> OnCollect;

        public bool IsCollected => _isCollected;
        public KnowledgeType Type => _type;
        public string TechnologyName => _technologyName;
        public string Description => _description;
    }
}
