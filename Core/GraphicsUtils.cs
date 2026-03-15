// ============================================================================
// GraphicsUtils.cs - Графические утилиты / Graphics utilities
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord.Core
{
    /// <summary>
    /// Класс с графическими утилитами.
    /// Graphics utilities class.
    /// </summary>
    public static class GraphicsUtils
    {
        private static Texture2D _whiteTexture;
        
        /// <summary>
        /// Белый текстурный пиксель для рисования.
        /// White texture pixel for drawing.
        /// </summary>
        public static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(GameInstance.GraphicsDevice, 1, 1);
                    _whiteTexture.SetData(new[] { Color.White });
                }
                return _whiteTexture;
            }
        }
        
        /// <summary>
        /// Рисует прямоугольник с рамкой.
        /// Draws a rectangle with border.
        /// </summary>
        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, int borderThickness = 1)
        {
            // Заполнение / Fill
            spriteBatch.Draw(WhiteTexture, rect, color);
            
            // Рамка / Border
            if (borderThickness > 0)
            {
                // Верх и низ / Top and bottom
                spriteBatch.Draw(WhiteTexture, new Rectangle(rect.X, rect.Y, rect.Width, borderThickness), Color.White);
                spriteBatch.Draw(WhiteTexture, new Rectangle(rect.X, rect.Bottom - borderThickness, rect.Width, borderThickness), Color.White);
                
                // Лево и право / Left and right
                spriteBatch.Draw(WhiteTexture, new Rectangle(rect.X, rect.Y, borderThickness, rect.Height), Color.White);
                spriteBatch.Draw(WhiteTexture, new Rectangle(rect.Right - borderThickness, rect.Y, borderThickness, rect.Height), Color.White);
            }
        }
        
        /// <summary>
        /// Рисует линию между двумя точками.
        /// Draws a line between two points.
        /// </summary>
        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            
            spriteBatch.Draw(
                WhiteTexture,
                start,
                null,
                color,
                rotation,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f);
        }
        
        /// <summary>
        /// Создаёт текстуру заданного цвета.
        /// Creates a texture of specified color.
        /// </summary>
        public static Texture2D CreateColorTexture(Color color, int width = 1, int height = 1)
        {
            Texture2D texture = new Texture2D(GameInstance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            texture.SetData(data);
            return texture;
        }
        
        /// <summary>
        /// Ссылка на экземпляр игры.
        /// Reference to game instance.
        /// </summary>
        public static Game GameInstance { get; set; }
    }
}
