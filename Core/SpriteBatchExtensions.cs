using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord.Core
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D _whiteTexture;

        public static void Draw(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _whiteTexture.SetData(new[] { Color.White });
            }
            
            spriteBatch.Draw(_whiteTexture, rect, color);
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            Rectangle rect,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth)
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _whiteTexture.SetData(new[] { Color.White });
            }
            
            spriteBatch.Draw(_whiteTexture, rect, null, color, rotation, origin, effects, layerDepth);
        }

        public static void Draw(
            this SpriteBatch spriteBatch,
            Rectangle rect,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effects,
            float layerDepth,
            float scale)
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _whiteTexture.SetData(new[] { Color.White });
            }
            
            var scaledRect = new Rectangle(
                rect.X,
                rect.Y,
                (int)(rect.Width * scale),
                (int)(rect.Height * scale)
            );
            
            spriteBatch.Draw(_whiteTexture, scaledRect, null, color, rotation, origin, effects, layerDepth);
        }
    }
}
