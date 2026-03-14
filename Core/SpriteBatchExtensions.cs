using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord
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
    }
}
