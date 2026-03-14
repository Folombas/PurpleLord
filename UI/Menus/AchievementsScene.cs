// ============================================================================
// AchievementsScene.cs - Экран достижений / Achievements screen
// Показывает все достижения и прогресс
// Shows all achievements and progress
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Scenes;

namespace PurpleLordPlatformer.UI.Menus
{
    public class AchievementsScene : Scene
    {
        private List<Achievement> _achievements;
        private int _selectedIndex = 0;
        private float _scrollOffset = 0f;
        private const float ItemHeight = 80f;

        public override string SceneId => "achievements";
        public override string SceneName => "Achievements";
        public override Color BackgroundColor => new Color(20, 20, 40);

        public override void LoadContent()
        {
            base.LoadContent();
            
            var achievementSystem = new AchievementSystem();
            _achievements = achievementSystem.GetAllAchievements();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleInput();
        }

        private void HandleInput()
        {
            if (InputManager.IsKeyJustPressed(Keys.W) || InputManager.IsKeyJustPressed(Keys.Up))
            {
                _selectedIndex--;
                if (_selectedIndex < 0) _selectedIndex = _achievements.Count - 1;
            }

            if (InputManager.IsKeyJustPressed(Keys.S) || InputManager.IsKeyJustPressed(Keys.Down))
            {
                _selectedIndex++;
                if (_selectedIndex >= _achievements.Count) _selectedIndex = 0;
            }

            if (InputManager.IsKeyJustPressed(Keys.Escape) ||
                InputManager.IsKeyJustPressed(Keys.Backspace))
            {
                SceneManager.PopScene();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Заголовок / Title
            string title = "ACHIEVEMENTS";
            Vector2 titleSize = UIManager.DefaultFont?.MeasureString(title) ?? new Vector2(300, 50);
            spriteBatch.DrawString(
                UIManager.DefaultFont ?? CreateDefaultFont(),
                title,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - titleSize.X / 2, 30),
                Color.Gold,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f);

            // Общий прогресс / Total progress
            int unlocked = 0;
            int totalPoints = 0;
            foreach (var achievement in _achievements)
            {
                if (achievement.IsUnlocked)
                {
                    unlocked++;
                    totalPoints += achievement.Points;
                }
            }

            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            string progressText = $"Unlocked: {unlocked}/{_achievements.Count} | Points: {totalPoints}";
            Vector2 progressSize = font?.MeasureString(progressText) ?? new Vector2(400, 20);
            spriteBatch.DrawString(font, progressText,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - progressSize.X / 2, 70),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Список достижений / Achievements list
            DrawAchievementsList(spriteBatch);

            // Подсказка / Hint
            string hint = "Press ESC to go back";
            Vector2 hintSize = font?.MeasureString(hint) ?? new Vector2(200, 20);
            spriteBatch.DrawString(font, hint,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - hintSize.X / 2, 
                    GraphicsDevice.Viewport.Height - 40),
                Color.Gray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void DrawAchievementsList(SpriteBatch spriteBatch)
        {
            float startX = GraphicsDevice.Viewport.Width / 2f - 350;
            float startY = 120f - _scrollOffset;

            for (int i = 0; i < _achievements.Count; i++)
            {
                var achievement = _achievements[i];
                float y = startY + i * ItemHeight;

                // Пропуск если за пределами экрана / Skip if out of screen
                if (y < -ItemHeight || y > GraphicsDevice.Viewport.Height) continue;

                // Фон / Background
                Rectangle bgRect = new Rectangle((int)startX, (int)y, 700, (int)ItemHeight - 10);
                Color bgColor = achievement.IsUnlocked ? 
                    new Color(50, 50, 80, 200) : new Color(30, 30, 40, 150);
                
                if (i == _selectedIndex)
                {
                    bgColor = achievement.IsUnlocked ? 
                        new Color(80, 80, 120, 255) : new Color(50, 50, 60, 200);
                }

                spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, bgColor);

                // Иконка / Icon
                Rectangle iconRect = new Rectangle((int)startX + 10, (int)y + 10, 60, 60);
                Color iconColor = achievement.IsUnlocked ? Color.Gold : Color.Gray;
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, iconRect, iconColor);

                // Название / Name
                SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
                Vector2 namePos = new Vector2(startX + 80, y + 15);
                Color nameColor = achievement.IsUnlocked ? Color.White : Color.DarkGray;
                spriteBatch.DrawString(font, achievement.Name, namePos, nameColor, 
                    0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

                // Описание / Description
                Vector2 descPos = new Vector2(startX + 80, y + 40);
                spriteBatch.DrawString(font, achievement.Description, descPos, 
                    achievement.IsUnlocked ? Color.LightGray : Color.DarkGray,
                    0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                // Прогресс / Progress
                if (!achievement.IsUnlocked)
                {
                    string progressText = $"{achievement.Progress}/{achievement.RequiredProgress}";
                    Vector2 progressPos = new Vector2(startX + 650, y + 30);
                    spriteBatch.DrawString(font, progressText, progressPos, Color.Yellow,
                        0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                    // Полоска прогресса / Progress bar
                    float barWidth = 100;
                    float barHeight = 10;
                    Rectangle barBg = new Rectangle((int)startX + 600, (int)y + 55, 
                        (int)barWidth, (int)barHeight);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, barBg, new Color(50, 50, 50));

                    float fillWidth = barWidth * achievement.ProgressPercent;
                    Rectangle barFill = new Rectangle((int)startX + 600, (int)y + 55, 
                        (int)fillWidth, (int)barHeight);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, barFill, Color.Green);
                }
                else
                {
                    // Значок разблокировано / Unlocked icon
                    string unlockedText = $"✓ {achievement.Points} pts";
                    Vector2 unlockedPos = new Vector2(startX + 620, y + 30);
                    spriteBatch.DrawString(font, unlockedText, unlockedPos, Color.Gold,
                        0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        private SpriteFont CreateDefaultFont() => null;
    }
}
