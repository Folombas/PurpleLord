// ============================================================================
// LoadingScene.cs - Экран загрузки / Loading screen
// Показывает прогресс загрузки контента
// Shows content loading progress
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Scenes;

namespace PurpleLordPlatformer.UI.Menus
{
    public class LoadingScene : Scene
    {
        private Scene _nextScene;
        private float _loadProgress = 0f;
        private bool _isLoading = true;
        private string _loadingText = "Loading...";
        private string _tipText = "";
        private List<string> _tips = new List<string>();
        private int _currentTipIndex = 0;
        private float _tipTimer = 0f;
        private float _tipChangeTime = 2f;

        public override string SceneId => "loading";
        public override string SceneName => "Loading";
        public override Color BackgroundColor => new Color(10, 10, 20);

        public LoadingScene()
        {
            InitializeTips();
        }

        public void SetNextScene(Scene scene)
        {
            _nextScene = scene;
        }

        private void InitializeTips()
        {
            _tips = new List<string>
            {
                "Совет: Используйте фокус для замедления времени",
                "Tip: Double jump helps reach higher platforms",
                "Не собирайте всё подряд — это путь к расфокусировке",
                "Focus mode slows down time for precise platforming",
                "Каждый уровень — это новая IT-технология",
                "Each level represents a different IT sphere",
                "Смерть — это часть обучения. Попробуйте снова!",
                "Death is part of learning. Try again!",
                "Фиолетовый Лорд выбирает свой путь в IT",
                "Purple Lord chooses his path in the tech world"
            };
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _loadProgress = 0f;
            _isLoading = true;
            _currentTipIndex = 0;
            _tipTimer = 0f;
            
            // Загрузка следующей сцены / Load next scene
            if (_nextScene != null)
            {
                _nextScene.Initialize(Game, ContentManager, InputManager, 
                    AudioManager, EffectManager, UIManager);
                _nextScene.LoadContent();
            }
            
            _loadProgress = 100f;
            _isLoading = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Смена подсказок / Change tips
            if (!_isLoading)
            {
                _tipTimer += delta;
                if (_tipTimer >= _tipChangeTime)
                {
                    _tipTimer = 0f;
                    _currentTipIndex = (_currentTipIndex + 1) % _tips.Count;
                    _tipText = _tips[_currentTipIndex];
                }
            }

            // Переход к следующей сцене / Transition to next scene
            if (!_isLoading && _loadProgress >= 100f)
            {
                // Небольшая задержка перед переходом / Small delay before transition
                _tipTimer += delta;
                if (_tipTimer > 1f)
                {
                    SceneManager.LoadScene(_nextScene?.GetType());
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Фон / Background
            Rectangle screen = new Rectangle(0, 0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, BackgroundColor);

            // Заголовок / Title
            string title = "LOADING";
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            Vector2 titleSize = font.MeasureString(title);
            spriteBatch.DrawString(font, title,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - titleSize.X / 2, 100),
                Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            // Прогресс бар / Progress bar
            float barWidth = 400;
            float barHeight = 30;
            Vector2 barPos = new Vector2(
                GraphicsDevice.Viewport.Width / 2f - barWidth / 2,
                250);

            // Фон полоски / Bar background
            Rectangle bgRect = new Rectangle((int)barPos.X, (int)barPos.Y, 
                (int)barWidth, (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(50, 50, 50));

            // Заполнение / Fill
            float fillWidth = barWidth * (_loadProgress / 100f);
            Rectangle fillRect = new Rectangle((int)barPos.X, (int)barPos.Y, 
                (int)fillWidth, (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, fillRect, Color.Cyan);

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bgRect, Color.White, 2);

            // Процент / Percentage
            string percentText = $"{(int)_loadProgress}%";
            Vector2 percentSize = font.MeasureString(percentText);
            spriteBatch.DrawString(font, percentText,
                new Vector2(barPos.X + barWidth / 2 - percentSize.X / 2, barPos.Y + 5),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Подсказка / Tip
            if (!string.IsNullOrEmpty(_tipText))
            {
                Vector2 tipSize = font.MeasureString(_tipText);
                spriteBatch.DrawString(font, _tipText,
                    new Vector2(GraphicsDevice.Viewport.Width / 2f - tipSize.X / 2, 400),
                    new Color(150, 150, 150), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            // Индикатор загрузки контента / Content loading indicator
            if (_isLoading)
            {
                DrawLoadingIndicator(spriteBatch, gameTime);
            }
        }

        private void DrawLoadingIndicator(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            Vector2 center = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                350);

            // Вращающиеся сегменты / Rotating segments
            int segments = 8;
            for (int i = 0; i < segments; i++)
            {
                float angle = time * 3f + i * MathHelper.Pi * 2 / segments;
                float radius = 40f;
                Vector2 pos = center + new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);

                float alpha = (float)Math.Sin(time * 5f + i) * 0.5f + 0.5f;
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, pos, null,
                    new Color(255, 255, 255, (int)(alpha * 255)), 0f,
                    Vector2.One * 5f, 1f, SpriteEffects.None, 0f);
            }
        }

        private SpriteFont CreateDefaultFont() => null;
    }
}
