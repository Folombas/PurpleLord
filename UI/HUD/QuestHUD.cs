// ============================================================================
// QuestHUD.cs - HUD для квестов / Quest HUD
// Отображение активных заданий на экране
// Display active quests on screen
// ============================================================================

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.UI.HUD
{
    public class QuestHUD
    {
        private QuestSystem _questSystem;
        private Vector2 _position = new Vector2(20, 200);
        private float _panelWidth = 300;
        private float _itemHeight = 70;

        public QuestHUD(QuestSystem questSystem)
        {
            _questSystem = questSystem;
        }

        public void Update(GameTime gameTime)
        {
            // Обновление логики / Update logic
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var activeQuests = _questSystem.GetActiveQuests();
            if (activeQuests.Count == 0) return;

            float y = _position.Y;

            // Заголовок / Title
            SpriteFont font = UIManager.DefaultFont;
            if (font != null)
            {
                Vector2 titleSize = font.MeasureString("QUESTS");
                spriteBatch.DrawString(font, "QUESTS",
                    new Vector2(_position.X + _panelWidth / 2 - titleSize.X / 2, y),
                    Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
                y += 30;
            }

            // Активные квесты / Active quests
            foreach (var quest in activeQuests)
            {
                if (quest.IsTracked)
                {
                    DrawQuestPanel(spriteBatch, font, quest, y);
                    y += _itemHeight + 10;
                }
            }
        }

        private void DrawQuestPanel(SpriteBatch spriteBatch, SpriteFont font, Quest quest, float y)
        {
            // Фон / Background
            Rectangle bgRect = new Rectangle(
                (int)_position.X,
                (int)y,
                (int)_panelWidth,
                (int)_itemHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(0, 0, 0, 150));

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bgRect, Color.Gray, 1);

            float padding = 10f;
            float textX = _position.X + padding;

            // Название / Title
            if (font != null)
            {
                // Wrap title if needed
                string title = quest.Title;
                Vector2 titleSize = font.MeasureString(title);
                float scale = Math.Min(1f, (_panelWidth - padding * 2) / titleSize.X);
                
                spriteBatch.DrawString(font, title,
                    new Vector2(textX, y + 5),
                    Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                // Прогресс / Progress
                string progress = quest.GetProgressString();
                Vector2 progressSize = font.MeasureString(progress);
                spriteBatch.DrawString(font, progress,
                    new Vector2(textX, y + 25),
                    Color.LightGray, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                // Полоска прогресса / Progress bar
                float barWidth = _panelWidth - padding * 2;
                float barHeight = 8;
                float barY = y + 45;

                Rectangle barBg = new Rectangle(
                    (int)(textX),
                    (int)barY,
                    (int)barWidth,
                    (int)barHeight);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, barBg, new Color(50, 50, 50));

                float fillWidth = barWidth * quest.ProgressPercent;
                Rectangle barFill = new Rectangle(
                    (int)(textX),
                    (int)barY,
                    (int)fillWidth,
                    (int)barHeight);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, barFill, Color.Green);
            }
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetPanelWidth(float width)
        {
            _panelWidth = width;
        }
    }
}
