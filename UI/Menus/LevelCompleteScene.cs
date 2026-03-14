// ============================================================================
// LevelCompleteScene.cs - Экран завершения уровня / Level complete screen
// Показывает статистику и процент изученного
// Shows statistics and completion percentage
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Scenes;
using PurpleLordPlatformer.Levels;

namespace PurpleLordPlatformer.UI.Menus
{
    public class LevelCompleteScene : Scene
    {
        private LevelStats _stats;
        private float _timer = 0f;
        private float _percentDisplay = 0f;
        private int _selectedButton = 0;
        private string[] _buttons = { "Next Level", "Collect More", "Main Menu" };

        public LevelCompleteScene()
        {
        }

        public void SetStats(LevelStats stats)
        {
            _stats = stats;
        }

        public override string SceneId => "level_complete";
        public override string SceneName => "Level Complete";
        public override Color BackgroundColor => new Color(20, 30, 50);

        public override void LoadContent()
        {
            base.LoadContent();
            _timer = 0f;
            _percentDisplay = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Анимация процента / Animate percentage
            if (_percentDisplay < _stats?.CompletionPercent)
            {
                _percentDisplay += (float)gameTime.ElapsedGameTime.TotalSeconds * 30f;
                if (_percentDisplay > _stats.CompletionPercent)
                    _percentDisplay = _stats.CompletionPercent;
            }

            HandleInput();
        }

        private void HandleInput()
        {
            if (InputManager.IsKeyJustPressed(Keys.A) || InputManager.IsKeyJustPressed(Keys.Left))
            {
                _selectedButton--;
                if (_selectedButton < 0) _selectedButton = _buttons.Length - 1;
            }

            if (InputManager.IsKeyJustPressed(Keys.D) || InputManager.IsKeyJustPressed(Keys.Right))
            {
                _selectedButton++;
                if (_selectedButton >= _buttons.Length) _selectedButton = 0;
            }

            if (InputManager.IsKeyJustPressed(Keys.Enter) || 
                InputManager.IsKeyJustPressed(Keys.Space) ||
                InputManager.IsGamePadButtonJustPressed(Buttons.A))
            {
                SelectButton();
            }
        }

        private void SelectButton()
        {
            switch (_selectedButton)
            {
                case 0: // Next Level
                    // Переход на следующий уровень / Go to next level
                    SceneManager.LoadScene<Level2_BackendBadlands>();
                    break;
                case 1: // Collect More
                    // Возврат на уровень / Return to level
                    SceneManager.PopScene();
                    break;
                case 2: // Main Menu
                    SceneManager.LoadScene<MainMenuScene>();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Заголовок / Title
            string title = "Level Complete!";
            Vector2 titleSize = UIManager.DefaultFont?.MeasureString(title) ?? new Vector2(300, 50);
            spriteBatch.DrawString(
                UIManager.DefaultFont ?? CreateDefaultFont(),
                title,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - titleSize.X / 2, 100),
                Color.Gold,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f);

            // Статистика / Statistics
            if (_stats != null)
            {
                DrawStatistics(spriteBatch);
            }

            // Процент изученного / Completion percentage
            DrawPercentage(spriteBatch);

            // Кнопки / Buttons
            DrawButtons(spriteBatch);

            // Философская цитата / Philosophical quote
            DrawQuote(spriteBatch);
        }

        private void DrawStatistics(SpriteBatch spriteBatch)
        {
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            if (font == null) return;

            float startX = GraphicsDevice.Viewport.Width / 2f - 150;
            float startY = 200f;
            float lineHeight = 40f;

            // Knowledge collected
            string knowledgeText = $"Knowledge: {_stats.KnowledgeCollected}/{_stats.KnowledgeTotal}";
            spriteBatch.DrawString(font, knowledgeText, 
                new Vector2(startX, startY), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            // Time spent
            string timeText = $"Time: {_stats.TimeSpent.Minutes}:{_stats.TimeSpent.Seconds:D2}";
            spriteBatch.DrawString(font, timeText,
                new Vector2(startX, startY + lineHeight), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            // Deaths
            string deathsText = $"Deaths: {_stats.Deaths}";
            spriteBatch.DrawString(font, deathsText,
                new Vector2(startX, startY + lineHeight * 2), Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
        }

        private void DrawPercentage(SpriteBatch spriteBatch)
        {
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            if (font == null) return;

            float centerX = GraphicsDevice.Viewport.Width / 2f;
            float centerY = 400f;

            // Круговая диаграмма / Circular chart
            DrawCircularChart(spriteBatch, centerX, centerY, _percentDisplay / 100f);

            // Процент / Percentage text
            string percentText = $"{(int)_percentDisplay}%";
            Vector2 percentSize = font.MeasureString(percentText);
            spriteBatch.DrawString(font, percentText,
                new Vector2(centerX - percentSize.X / 2, centerY + 50),
                Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            // Подпись / Label
            string label = "STUDIED";
            Vector2 labelSize = font.MeasureString(label);
            spriteBatch.DrawString(font, label,
                new Vector2(centerX - labelSize.X / 2, centerY + 90),
                Color.Gray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void DrawCircularChart(SpriteBatch spriteBatch, float centerX, float centerY, float percent)
        {
            int radius = 80;
            int thickness = 15;

            // Фон круга / Circle background
            DrawCircle(spriteBatch, centerX, centerY, radius, Color.Gray, thickness);

            // Заполнение / Fill
            if (percent > 0)
            {
                DrawCircleArc(spriteBatch, centerX, centerY, radius, percent, Color.Gold, thickness);
            }
        }

        private void DrawCircle(SpriteBatch spriteBatch, float centerX, float centerY, int radius, Color color, int thickness)
        {
            int segments = 36;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * MathHelper.Pi * 2 / segments;
                float angle2 = (i + 1) * MathHelper.Pi * 2 / segments;

                Vector2 pos1 = new Vector2(
                    centerX + (float)Math.Cos(angle1) * radius,
                    centerY + (float)Math.Sin(angle1) * radius);
                Vector2 pos2 = new Vector2(
                    centerX + (float)Math.Cos(angle2) * radius,
                    centerY + (float)Math.Sin(angle2) * radius);

                GraphicsUtils.DrawLine(spriteBatch, pos1, pos2, color, thickness);
            }
        }

        private void DrawCircleArc(SpriteBatch spriteBatch, float centerX, float centerY, int radius, float percent, Color color, int thickness)
        {
            int segments = (int)(36 * percent);
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * MathHelper.Pi * 2 / 36 - MathHelper.PiOver2;
                float angle2 = (i + 1) * MathHelper.Pi * 2 / 36 - MathHelper.PiOver2;

                Vector2 pos1 = new Vector2(
                    centerX + (float)Math.Cos(angle1) * radius,
                    centerY + (float)Math.Sin(angle1) * radius);
                Vector2 pos2 = new Vector2(
                    centerX + (float)Math.Cos(angle2) * radius,
                    centerY + (float)Math.Sin(angle2) * radius);

                GraphicsUtils.DrawLine(spriteBatch, pos1, pos2, color, thickness);
            }
        }

        private void DrawButtons(SpriteBatch spriteBatch)
        {
            float buttonY = GraphicsDevice.Viewport.Height - 200;
            float buttonSpacing = 120f;
            float startX = GraphicsDevice.Viewport.Width / 2f - buttonSpacing;

            for (int i = 0; i < _buttons.Length; i++)
            {
                float buttonX = startX + i * buttonSpacing;
                bool isSelected = (i == _selectedButton);

                // Фон кнопки / Button background
                Rectangle buttonRect = new Rectangle(
                    (int)(buttonX - 80),
                    (int)buttonY,
                    160, 50);

                Color bgColor = isSelected ? new Color(100, 100, 100, 200) : new Color(50, 50, 50, 150);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, buttonRect, bgColor);

                // Рамка / Border
                GraphicsUtils.DrawRectangle(spriteBatch, buttonRect, isSelected ? Color.White : Color.Gray, 2);

                // Текст / Text
                SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
                Vector2 textSize = font?.MeasureString(_buttons[i]) ?? new Vector2(100, 20);
                spriteBatch.DrawString(
                    font,
                    _buttons[i],
                    new Vector2(buttonX - textSize.X / 2, buttonY + 15),
                    isSelected ? Color.Yellow : Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0f);
            }
        }

        private void DrawQuote(SpriteBatch spriteBatch)
        {
            string[] quotes = {
                "Нельзя объять необъятное. - Козьма Прутков",
                "Less is more. - Ludwig Mies van der Rohe",
                "Choose one path and walk it with purpose.",
                "The expert in anything was once a beginner.",
                "Focus is the key to mastery."
            };

            int quoteIndex = (int)(_timer / 5) % quotes.Length;
            
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            if (font == null) return;

            Vector2 textSize = font.MeasureString(quotes[quoteIndex]);
            spriteBatch.DrawString(font, quotes[quoteIndex],
                new Vector2(GraphicsDevice.Viewport.Width / 2f - textSize.X / 2, 
                    GraphicsDevice.Viewport.Height - 80),
                new Color(150, 150, 150), 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);
        }

        private SpriteFont CreateDefaultFont() => null;
    }
}
