// ============================================================================
// PauseScene.cs - Сцена паузы / Pause scene
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Scenes;

namespace PurpleLordPlatformer.UI.Menus
{
    public class PauseScene : Scene
    {
        private int _selectedItem = 0;
        private string[] _menuItems = { "Resume", "Restart Level", "Options", "Main Menu" };
        private float _timer = 0f;

        public override string SceneId => "pause";
        public override string SceneName => "Pause Menu";
        public override Color BackgroundColor => new Color(0, 0, 0, 180);

        public override void LoadContent()
        {
            base.LoadContent();
            AudioManager?.PauseAll();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput();
        }

        private void HandleInput()
        {
            if (InputManager.IsKeyJustPressed(Keys.W) || InputManager.IsKeyJustPressed(Keys.Up))
            {
                _selectedItem--;
                if (_selectedItem < 0) _selectedItem = _menuItems.Length - 1;
                AudioManager?.PlaySound("menu_hover");
            }

            if (InputManager.IsKeyJustPressed(Keys.S) || InputManager.IsKeyJustPressed(Keys.Down))
            {
                _selectedItem++;
                if (_selectedItem >= _menuItems.Length) _selectedItem = 0;
                AudioManager?.PlaySound("menu_hover");
            }

            if (InputManager.IsKeyJustPressed(Keys.Enter) || 
                InputManager.IsKeyJustPressed(Keys.Space) ||
                InputManager.IsKeyJustPressed(Keys.Escape) ||
                InputManager.IsGamePadButtonJustPressed(Buttons.Start))
            {
                AudioManager?.PlaySound("menu_select");
                SelectItem();
            }
        }

        private void SelectItem()
        {
            switch (_selectedItem)
            {
                case 0: // Resume
                    SceneManager.PopScene();
                    break;
                case 1: // Restart Level
                    RestartLevel();
                    break;
                case 2: // Options
                    // TODO: Open options
                    break;
                case 3: // Main Menu
                    SceneManager.LoadScene<MainMenuScene>();
                    break;
            }
        }

        private void RestartLevel()
        {
            // Перезапуск текущего уровня / Restart current level
            var currentScene = SceneManager.CurrentScene;
            if (currentScene is Levels.Level level)
            {
                string levelId = level.SceneId;
                SceneManager.PopScene(); // Close pause
                SceneManager.LoadScene<Levels.Level1_FrontendForest>(); // TODO: Load correct level
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Затемнение фона / Background dimming
            Rectangle screen = new Rectangle(0, 0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, new Color(0, 0, 0, 150));

            // Меню паузы / Pause menu
            float menuX = GraphicsDevice.Viewport.Width / 2f;
            float menuY = GraphicsDevice.Viewport.Height / 2f - 50;
            float itemHeight = 60f;

            // Заголовок / Title
            string title = "PAUSED";
            Vector2 titleSize = UIManager.DefaultFont?.MeasureString(title) ?? new Vector2(200, 50);
            spriteBatch.DrawString(
                UIManager.DefaultFont ?? CreateDefaultFont(),
                title,
                new Vector2(menuX - titleSize.X / 2, menuY - 80),
                Color.White,
                0f,
                Vector2.Zero,
                2f,
                SpriteEffects.None,
                0f);

            // Пункты меню / Menu items
            for (int i = 0; i < _menuItems.Length; i++)
            {
                Color color = (i == _selectedItem) ? Color.Yellow : Color.White;
                float scale = (i == _selectedItem) ? 1.3f : 1f;
                
                // Пульсация для выбранного пункта / Pulse for selected item
                if (i == _selectedItem)
                {
                    float pulse = (float)System.Math.Sin(_timer * 5) * 0.2f + 1f;
                    scale *= pulse;
                }

                Vector2 pos = new Vector2(menuX, menuY + i * itemHeight);
                Vector2 origin = UIManager.DefaultFont?.MeasureString(_menuItems[i]) / 2 ?? Vector2.Zero;

                spriteBatch.DrawString(
                    UIManager.DefaultFont ?? CreateDefaultFont(),
                    _menuItems[i],
                    pos,
                    color,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f);
            }
        }

        private SpriteFont CreateDefaultFont() => null;

        public override void Unload()
        {
            base.Unload();
            AudioManager?.ResumeAll();
        }
    }
}
