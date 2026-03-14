// ============================================================================
// HUD.cs - Интерфейс во время игры / Heads-Up Display
// Шкала осознанности, компас, мини-карта
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities.Player;
using PurpleLordPlatformer.Core;

namespace PurpleLordPlatformer.UI.HUD
{
    public class HUD
    {
        private Player _player;
        private GameState _gameState;
        private SpriteFont _font;
        private Vector2 _compassTarget = Vector2.Zero;
        private bool _showCompass = true;

        public HUD(Player player, GameState gameState, SpriteFont font)
        {
            _player = player;
            _gameState = gameState;
            _font = font;
        }

        public void Update(GameTime gameTime)
        {
            // Обновление компаса / Update compass
            if (_player != null)
            {
                // Компас указывает на ближайший несобранный предмет
                // Compass points to nearest uncollected item
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawHealthBar(spriteBatch);
            DrawAwarenessMeter(spriteBatch);
            DrawCompass(spriteBatch);
            DrawKnowledgeCounter(spriteBatch);
            DrawFocusLossIndicator(spriteBatch);
        }

        private void DrawHealthBar(SpriteBatch spriteBatch)
        {
            // Полоска здоровья в левом верхнем углу (под шкалой фокуса)
            // Health bar in top-left corner (below focus meter)
            if (_player == null) return;

            float barWidth = 200;
            float barHeight = 25;
            Vector2 pos = new Vector2(20, 50);

            // Фон / Background
            Rectangle bgRect = new Rectangle((int)pos.X, (int)pos.Y, (int)barWidth, (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(50, 50, 50));

            // Заполнение / Fill
            float fillPercent = _player.HealthPercent;
            Rectangle fillRect = new Rectangle((int)pos.X, (int)pos.Y, (int)(barWidth * fillPercent), (int)barHeight);
            
            // Цвет зависит от здоровья / Color depends on health
            Color healthColor = fillPercent > 0.6f ? Color.Green : 
                               (fillPercent > 0.3f ? Color.Yellow : Color.Red);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, fillRect, healthColor);

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bgRect, Color.White, 2);

            // Сердца / Hearts
            for (int i = 0; i < _player.MaxHealth; i++)
            {
                float heartX = pos.X + 10 + i * 35;
                float heartY = pos.Y + 3;
                
                if (i < _player.CurrentHealth)
                {
                    // Заполненное сердце / Filled heart
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                        new Rectangle((int)heartX, (int)heartY, 20, 20), Color.Red);
                }
                else
                {
                    // Пустое сердце / Empty heart
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                        new Rectangle((int)heartX, (int)heartY, 20, 20), new Color(80, 80, 80));
                }
            }
        }

        private void DrawAwarenessMeter(SpriteBatch spriteBatch)
        {
            // Шкала осознанности в левом верхнем углу
            // Awareness meter in top-left corner
            float barWidth = 200;
            float barHeight = 20;
            Vector2 pos = new Vector2(20, 85);

            // Фон / Background
            Rectangle bgRect = new Rectangle((int)pos.X, (int)pos.Y, (int)barWidth, (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(50, 50, 50));

            // Заполнение / Fill
            float fillPercent = _gameState != null ? _gameState.AwarenessMeter / _gameState.MaxAwarenessMeter : 1f;
            Rectangle fillRect = new Rectangle((int)pos.X, (int)pos.Y, (int)(barWidth * fillPercent), (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, fillRect, Color.Cyan);

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bgRect, Color.White, 2);

            // Текст / Text
            if (_font != null)
            {
                string text = $"FOCUS: {(int)(fillPercent * 100)}%";
                Vector2 textSize = _font.MeasureString(text);
                spriteBatch.DrawString(_font, text, 
                    new Vector2(pos.X + barWidth / 2 - textSize.X / 2, pos.Y + 2),
                    Color.White);
            }
        }

        private void DrawCompass(SpriteBatch spriteBatch)
        {
            if (!_showCompass) return;

            // Компас в верхней части экрана
            // Compass at top of screen
            Vector2 center = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth / 2f,
                60);
            float radius = 40f;

            // Фон компаса / Compass background
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(center.X - radius), (int)(center.Y - radius),
                    (int)(radius * 2), (int)(radius * 2)),
                new Color(0, 0, 0, 150));

            // Стрелка компаса / Compass needle
            float angle = (float)System.Math.Atan2(
                _compassTarget.Y - center.Y,
                _compassTarget.X - center.X);

            Vector2 needleEnd = center + new Vector2(
                (float)System.Math.Cos(angle) * radius * 0.8f,
                (float)System.Math.Sin(angle) * radius * 0.8f);

            GraphicsUtils.DrawLine(spriteBatch, center, needleEnd, Color.Yellow, 3);

            // Маркер цели / Target marker
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(needleEnd.X - 3), (int)(needleEnd.Y - 3), 6, 6),
                Color.Red);
        }

        private void DrawKnowledgeCounter(SpriteBatch spriteBatch)
        {
            // Счётчик знаний в правом верхнем углу
            // Knowledge counter in top-right corner
            if (_font == null) return;

            string text = $"Knowledge: {_gameState?.GetOrCreateLevelStats("current")?.KnowledgeCollected ?? 0}";
            Vector2 textSize = _font.MeasureString(text);
            Vector2 pos = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth - textSize.X - 20,
                20);

            spriteBatch.DrawString(_font, text, pos, Color.Gold);
        }

        private void DrawFocusLossIndicator(SpriteBatch spriteBatch)
        {
            // Индикатор расфокусировки
            // Focus loss indicator
            if (_gameState == null || _gameState.FocusDrainLevel <= 0) return;

            float alpha = _gameState.FocusDrainLevel / 100f;
            Vector2 center = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth / 2f,
                GraphicsDeviceManager.DefaultBackBufferHeight / 2f);

            // Пульсирующий круг / Pulsing circle
            float pulse = (float)System.Math.Sin(_gameState.FocusDrainLevel * 5) * 0.3f + 0.7f;
            
            for (float r = 100; r > 0; r -= 20)
            {
                spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                    new Rectangle((int)(center.X - r), (int)(center.Y - r),
                        (int)(r * 2), (int)(r * 2)),
                    new Color(255, 0, 0, (int)(50 * alpha * pulse * (r / 100))));
            }
        }

        public void SetCompassTarget(Vector2 target)
        {
            _compassTarget = target;
        }

        public void ShowCompass(bool show)
        {
            _showCompass = show;
        }

        public SpriteFont Font
        {
            get => _font;
            set => _font = value;
        }
    }
}
