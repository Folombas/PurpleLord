// ============================================================================
// MainMenuScene.cs - Главное меню / Main menu
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Scenes;
using PurpleLordPlatformer.Core;
using PurpleLordPlatformer.Managers;
using PurpleLordPlatformer.UI;
using PurpleLordPlatformer.Levels;

namespace PurpleLordPlatformer.UI.Menus
{
    public class MainMenuScene : Scene
    {
        private enum MenuState
        {
            Main,
            LevelSelect,
            Options,
            Credits
        }

        private MenuState _currentState = MenuState.Main;
        private int _selectedItem = 0;
        private string[] _mainMenuItems = { "Start Game", "Level Select", "Options", "Credits", "Exit" };
        private float _animationTimer = 0f;
        private Vector2 _lordPosition = Vector2.Zero;
        private Vector2[] _techIconPositions = new Vector2[8];
        private float[] _techIconAngles = new float[8];

        public override string SceneId => "main_menu";
        public override string SceneName => "Main Menu";

        public override void Initialize(Game game, ContentManager contentManager,
            InputManager inputManager, AudioManager audioManager,
            EffectManager effectManager, UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            InitializeTechIcons();
        }

        private void InitializeTechIcons()
        {
            for (int i = 0; i < 8; i++)
            {
                _techIconAngles[i] = i * MathHelper.Pi / 4;
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            AudioManager?.PlayMusic("menu_theme");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _animationTimer += delta;

            // Анимация иконок технологий / Animate tech icons
            for (int i = 0; i < 8; i++)
            {
                _techIconAngles[i] += delta * 0.5f;
            }

            // Обновление позиции Лорда / Update Lord position
            _lordPosition = new Vector2(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height / 2f - 100);

            HandleInput(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            // Навигация меню / Menu navigation
            if (InputManager.IsKeyJustPressed(Keys.W) || InputManager.IsKeyJustPressed(Keys.Up))
            {
                _selectedItem--;
                if (_selectedItem < 0) _selectedItem = _mainMenuItems.Length - 1;
                AudioManager?.PlaySound("menu_hover");
            }

            if (InputManager.IsKeyJustPressed(Keys.S) || InputManager.IsKeyJustPressed(Keys.Down))
            {
                _selectedItem++;
                if (_selectedItem >= _mainMenuItems.Length) _selectedItem = 0;
                AudioManager?.PlaySound("menu_hover");
            }

            if (InputManager.IsKeyJustPressed(Keys.Enter) || 
                InputManager.IsKeyJustPressed(Keys.Space) ||
                InputManager.IsGamePadButtonJustPressed(Buttons.A))
            {
                AudioManager?.PlaySound("menu_select");
                SelectItem();
            }
        }

        private void SelectItem()
        {
            switch (_currentState)
            {
                case MenuState.Main:
                    switch (_selectedItem)
                    {
                        case 0: // Start Game
                            StartGame();
                            break;
                        case 1: // Level Select
                            _currentState = MenuState.LevelSelect;
                            break;
                        case 2: // Options
                            _currentState = MenuState.Options;
                            break;
                        case 3: // Credits
                            _currentState = MenuState.Credits;
                            break;
                        case 4: // Exit
                            Game.Exit();
                            break;
                    }
                    break;

                case MenuState.LevelSelect:
                    // Выбор уровня / Level selection
                    break;

                case MenuState.Options:
                case MenuState.Credits:
                    _currentState = MenuState.Main;
                    break;
            }
        }

        private void StartGame()
        {
            // Запуск первого уровня / Start first level
            SceneManager.LoadScene<Level1_FrontendForest>();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Отрисовка фона / Draw background
            DrawBackground(spriteBatch, gameTime);

            // Отрисовка Фиолетового Лорда / Draw Purple Lord
            DrawPurpleLord(spriteBatch, gameTime);

            // Отрисовка иконок технологий / Draw tech icons
            DrawTechIcons(spriteBatch, gameTime);

            // Отрисовка меню / Draw menu
            DrawMenu(spriteBatch, gameTime);
        }

        protected override void DrawBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Градиентный фон / Gradient background
            Rectangle screen = new Rectangle(0, 0, 
                GraphicsDevice.Viewport.Width, 
                GraphicsDevice.Viewport.Height);

            // Верх / Top
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, 
                new Color(30, 20, 50, 255));

            // Низ / Bottom (overlay)
            Rectangle bottom = new Rectangle(0, GraphicsDevice.Viewport.Height / 2,
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bottom,
                new Color(50, 30, 80, 200));
        }

        private void DrawPurpleLord(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Placeholder - фиолетовый силуэт / Placeholder - purple silhouette
            float bobOffset = (float)Math.Sin(_animationTimer * 2) * 10;
            
            // Тело / Body
            Rectangle body = new Rectangle(
                (int)_lordPosition.X - 30,
                (int)(_lordPosition.Y + bobOffset) - 40,
                60, 80);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, Color.Purple);

            // Голова / Head
            Rectangle head = new Rectangle(
                (int)_lordPosition.X - 20,
                (int)(_lordPosition.Y + bobOffset) - 70,
                40, 40);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, head, Color.DarkPurple);

            // Чемодан / Suitcase
            Rectangle suitcase = new Rectangle(
                (int)_lordPosition.X + 35,
                (int)(_lordPosition.Y + bobOffset) + 10,
                40, 30);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, suitcase, new Color(100, 50, 30));
        }

        private void DrawTechIcons(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float radius = 250f;
            Color[] colors = {
                new Color(255, 165, 0),   // Frontend - Orange
                new Color(0, 150, 255),   // Backend - Blue
                new Color(0, 200, 100),   // Database - Green
                new Color(150, 50, 200),  // DevOps - Purple
                new Color(255, 50, 150),  // ML/AI - Pink
                Color.Gold,               // Other - Gold
                Color.Cyan,               // Cloud - Cyan
                Color.Lime                // Security - Lime
            };

            for (int i = 0; i < 8; i++)
            {
                float x = _lordPosition.X + (float)Math.Cos(_techIconAngles[i]) * radius;
                float y = _lordPosition.Y + (float)Math.Sin(_techIconAngles[i]) * radius;

                // Иконка - круг / Icon - circle
                Rectangle iconRect = new Rectangle(
                    (int)x - 15,
                    (int)y - 15,
                    30, 30);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, iconRect, colors[i]);
            }
        }

        private void DrawMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float menuX = GraphicsDevice.Viewport.Width / 2f;
            float menuY = GraphicsDevice.Viewport.Height - 200;
            float itemHeight = 50f;
            float spacing = 10f;

            // Заголовок / Title
            string title = "PURPLE LORD: PATH OF CHOICES";
            Vector2 titleSize = UIManager.DefaultFont?.MeasureString(title) ?? new Vector2(400, 40);
            spriteBatch.DrawString(
                UIManager.DefaultFont ?? CreateDefaultFont(),
                title,
                new Vector2(menuX - titleSize.X / 2, 100),
                Color.White,
                0f,
                Vector2.Zero,
                1.5f,
                SpriteEffects.None,
                0f);

            // Пункты меню / Menu items
            for (int i = 0; i < _mainMenuItems.Length; i++)
            {
                Color color = (i == _selectedItem) ? Color.Yellow : Color.White;
                float scale = (i == _selectedItem) ? 1.2f : 1f;
                
                Vector2 pos = new Vector2(
                    menuX,
                    menuY + i * (itemHeight + spacing));

                // Подложка для выбранного пункта / Background for selected item
                if (i == _selectedItem)
                {
                    Vector2 size = UIManager.DefaultFont?.MeasureString(_mainMenuItems[i]) ?? new Vector2(200, 30);
                    Rectangle bgRect = new Rectangle(
                        (int)(pos.X - size.X / 2 - 20),
                        (int)(pos.Y - size.Y / 2),
                        (int)size.X + 40,
                        (int)size.Y);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(255, 255, 0, 50));
                }

                spriteBatch.DrawString(
                    UIManager.DefaultFont ?? CreateDefaultFont(),
                    _mainMenuItems[i],
                    pos,
                    color,
                    0f,
                    UIManager.DefaultFont?.MeasureString(_mainMenuItems[i]) / 2 ?? Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            // Подзаголовок / Subtitle
            string subtitle = "A Journey Through IT Technologies";
            Vector2 subSize = UIManager.DefaultFont?.MeasureString(subtitle) ?? new Vector2(300, 20);
            spriteBatch.DrawString(
                UIManager.DefaultFont ?? CreateDefaultFont(),
                subtitle,
                new Vector2(menuX - subSize.X / 2, 160),
                new Color(200, 200, 200, 255),
                0f,
                Vector2.Zero,
                0.8f,
                SpriteEffects.None,
                0f);
        }

        private SpriteFont CreateDefaultFont()
        {
            // Временный шрифт / Temporary font
            return null;
        }

        public override void Pause()
        {
            base.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            AudioManager?.ResumeAll();
        }
    }
}
