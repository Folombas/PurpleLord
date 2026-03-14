// ============================================================================
// Platform.cs - Платформа / Platform
// Базовый класс для всех платформ в игре
// Base class for all platforms in the game
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Entities.Objects
{
    public enum PlatformType
    {
        Solid,          // Твёрдая платформа / Solid platform
        OneWay,         // Односторонняя (можно прыгнуть снизу) / One-way (can jump from bottom)
        Moving,         // Движущаяся платформа / Moving platform
        Disappearing,   // Исчезающая платформа / Disappearing platform
        Hazard          // Опасная платформа (шипы, лава) / Hazard platform
    }

    public class Platform : GameObject
    {
        protected Texture2D _texture;
        protected PlatformType _type = PlatformType.Solid;
        protected Color _tintColor = Color.White;
        protected bool _isTrigger = false;

        public Platform(Vector2 position, float width, float height, PlatformType type = PlatformType.Solid)
            : base(position)
        {
            Width = width;
            Height = height;
            _type = type;
            Tag = "Platform";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_texture != null)
            {
                // Отрисовка с тайлингом / Tiled drawing
                DrawTiled(spriteBatch);
            }
            else
            {
                // Placeholder - прямоугольник / Placeholder - rectangle
                Rectangle rect = new Rectangle(
                    (int)(Position.X - Width / 2),
                    (int)(Position.Y - Height / 2),
                    (int)Width,
                    (int)Height);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, _tintColor);
            }
        }

        protected virtual void DrawTiled(SpriteBatch spriteBatch)
        {
            int tileWidth = _texture.Width;
            int tileHeight = _texture.Height;
            
            int startX = (int)(Position.X - Width / 2);
            int startY = (int)(Position.Y - Height / 2);
            
            for (int x = 0; x < Width; x += tileWidth)
            {
                for (int y = 0; y < Height; y += tileHeight)
                {
                    Rectangle destRect = new Rectangle(
                        startX + x,
                        startY + y,
                        Mathf.Min(tileWidth, (int)Width - x),
                        Mathf.Min(tileHeight, (int)Height - y));
                    
                    spriteBatch.Draw(_texture, destRect, _tintColor);
                }
            }
        }

        public bool IsOneWay => _type == PlatformType.OneWay;
        public bool IsHazard => _type == PlatformType.Hazard;
        public bool IsMoving => _type == PlatformType.Moving;
        
        public PlatformType Type => _type;
        
        public Color TintColor
        {
            get => _tintColor;
            set => _tintColor = value;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
    }

    // Вспомогательный класс для математики / Math helper class
    public static class Mathf
    {
        public static int Min(int a, int b) => a < b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        public static float Max(float a, float b) => a > b ? a : b;
    }
}
