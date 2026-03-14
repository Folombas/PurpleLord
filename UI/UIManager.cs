// ============================================================================
// UIManager.cs - Менеджер интерфейса / UI manager
// Отрисовка HUD, меню и других UI элементов
// HUD, menus and other UI elements rendering
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.UI
{
    public class UIManager : IDisposable
    {
        private Game _game;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private SpriteFont _defaultFont;
        private bool _isVisible = true;

        public UIManager(Game game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _defaultFont = content.Load<SpriteFont>("Fonts/defaultFont");
        }

        public void Update(GameTime gameTime)
        {
            // Update UI logic
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!_isVisible) return;
            // Draw UI elements
        }

        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }

        public void DrawText(string text, Vector2 position, Color color, float scale = 1f)
        {
            // Helper for drawing text
        }

        public void Dispose()
        {
            // Cleanup
        }

        public SpriteFont DefaultFont => _defaultFont;
        public GraphicsDevice GraphicsDevice => _graphicsDevice;
    }
}
