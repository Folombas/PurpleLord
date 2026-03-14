// ============================================================================
// GraphicsUtils.cs - Графические утилиты / Graphics utilities
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Core
{
    public static class GraphicsUtils
    {
        private static Texture2D _whiteTexture;

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

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 1)
        {
            DrawRectangle(spriteBatch, rect.X, rect.Y, rect.Width, rect.Height, color, thickness);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, int thickness = 1)
        {
            spriteBatch.Draw(WhiteTexture, new Rectangle((int)x, (int)y, (int)width, thickness), color);
            spriteBatch.Draw(WhiteTexture, new Rectangle((int)x, (int)(y + height - thickness), (int)width, thickness), color);
            spriteBatch.Draw(WhiteTexture, new Rectangle((int)x, (int)y, thickness, (int)height), color);
            spriteBatch.Draw(WhiteTexture, new Rectangle((int)(x + width - thickness), (int)y, thickness, (int)height), color);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
        {
            Vector2 edge = end - start;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(WhiteTexture, start, null, color, angle, Vector2.Zero, new Vector2(edge.Length(), thickness), SpriteEffects.None, 0);
        }
    }

    public static class GameInstance
    {
        public static GraphicsDevice GraphicsDevice { get; set; }
    }
}
