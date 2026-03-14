// ============================================================================
// OptionsScene.cs - Экран настроек / Options screen
// Настройки графики, звука, управления
// Graphics, audio, controls settings
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Scenes;

namespace PurpleLordPlatformer.UI.Menus
{
    public class OptionsScene : Scene
    {
        private enum OptionsTab
        {
            Audio,
            Video,
            Controls
        }

        private OptionsTab _currentTab = OptionsTab.Audio;
        private int _selectedItem = 0;
        private string[] _audioItems = { "Master Volume", "Music Volume", "SFX Volume", "Back" };
        private string[] _videoItems = { "Fullscreen", "Resolution", "VSync", "Back" };
        private string[] _controlsItems = { "Jump", "Focus", "Move Left", "Move Right", "Back" };
        
        private float _masterVolume = 1f;
        private float _musicVolume = 0.7f;
        private float _sfxVolume = 1f;
        private bool _fullscreen = true;
        private int _resolutionIndex = 0;
        private string[] _resolutions = { "1920x1080", "1600x900", "1280x720" };
        private bool _vsync = true;

        public override string SceneId => "options";
        public override string SceneName => "Options";
        public override Color BackgroundColor => new Color(20, 20, 40);

        public override void LoadContent()
        {
            base.LoadContent();
            // Загрузка настроек из SaveManager / Load settings from SaveManager
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleInput();
        }

        private void HandleInput()
        {
            // Переключение табов / Switch tabs
            if (InputManager.IsKeyJustPressed(Keys.Q))
            {
                _currentTab = OptionsTab.Audio;
                _selectedItem = 0;
            }
            else if (InputManager.IsKeyJustPressed(Keys.W))
            {
                _currentTab = OptionsTab.Video;
                _selectedItem = 0;
            }
            else if (InputManager.IsKeyJustPressed(Keys.E))
            {
                _currentTab = OptionsTab.Controls;
                _selectedItem = 0;
            }

            // Навигация / Navigation
            string[] currentItems = GetCurrentItems();
            
            if (InputManager.IsKeyJustPressed(Keys.Up))
            {
                _selectedItem--;
                if (_selectedItem < 0) _selectedItem = currentItems.Length - 1;
            }

            if (InputManager.IsKeyJustPressed(Keys.Down))
            {
                _selectedItem++;
                if (_selectedItem >= currentItems.Length) _selectedItem = 0;
            }

            // Изменение значений / Change values
            if (InputManager.IsKeyJustPressed(Keys.Left))
            {
                DecreaseValue();
            }

            if (InputManager.IsKeyJustPressed(Keys.Right))
            {
                IncreaseValue();
            }

            // Назад / Back
            if (InputManager.IsKeyJustPressed(Keys.Escape) ||
                InputManager.IsKeyJustPressed(Keys.Backspace))
            {
                SaveSettings();
                SceneManager.PopScene();
            }
        }

        private string[] GetCurrentItems()
        {
            switch (_currentTab)
            {
                case OptionsTab.Audio: return _audioItems;
                case OptionsTab.Video: return _videoItems;
                case OptionsTab.Controls: return _controlsItems;
                default: return _audioItems;
            }
        }

        private void IncreaseValue()
        {
            switch (_currentTab)
            {
                case OptionsTab.Audio:
                    switch (_selectedItem)
                    {
                        case 0: _masterVolume = MathHelper.Min(_masterVolume + 0.1f, 1f); break;
                        case 1: _musicVolume = MathHelper.Min(_musicVolume + 0.1f, 1f); break;
                        case 2: _sfxVolume = MathHelper.Min(_sfxVolume + 0.1f, 1f); break;
                    }
                    break;
                case OptionsTab.Video:
                    switch (_selectedItem)
                    {
                        case 0: _fullscreen = !_fullscreen; break;
                        case 1: _resolutionIndex = (_resolutionIndex + 1) % _resolutions.Length; break;
                        case 2: _vsync = !_vsync; break;
                    }
                    break;
            }
        }

        private void DecreaseValue()
        {
            switch (_currentTab)
            {
                case OptionsTab.Audio:
                    switch (_selectedItem)
                    {
                        case 0: _masterVolume = MathHelper.Max(_masterVolume - 0.1f, 0f); break;
                        case 1: _musicVolume = MathHelper.Max(_musicVolume - 0.1f, 0f); break;
                        case 2: _sfxVolume = MathHelper.Max(_sfxVolume - 0.1f, 0f); break;
                    }
                    break;
                case OptionsTab.Video:
                    switch (_selectedItem)
                    {
                        case 0: _fullscreen = !_fullscreen; break;
                        case 1: _resolutionIndex = (_resolutionIndex - 1 + _resolutions.Length) % _resolutions.Length; break;
                        case 2: _vsync = !_vsync; break;
                    }
                    break;
            }
        }

        private void SaveSettings()
        {
            AudioManager?.SetMasterVolume(_masterVolume);
            AudioManager?.SetMusicVolume(_musicVolume);
            AudioManager?.SetSfxVolume(_sfxVolume);
            
            // Применение настроек графики / Apply graphics settings
            if (Game is Game1 game)
            {
                game.GraphicsDeviceManager.IsFullScreen = _fullscreen;
                // TODO: Apply resolution
                game.GraphicsDeviceManager.ApplyChanges();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Заголовок / Title
            string title = "OPTIONS";
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            Vector2 titleSize = font.MeasureString(title);
            spriteBatch.DrawString(font, title,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - titleSize.X / 2, 50),
                Color.Gold, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            // Табы / Tabs
            DrawTabs(spriteBatch);

            // Элементы настроек / Settings items
            DrawSettingsItems(spriteBatch);

            // Подсказка / Hint
            string hint = "Left/Right: Change | ESC: Back";
            Vector2 hintSize = font.MeasureString(hint);
            spriteBatch.DrawString(font, hint,
                new Vector2(GraphicsDevice.Viewport.Width / 2f - hintSize.X / 2, 
                    GraphicsDevice.Viewport.Height - 50),
                Color.Gray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        private void DrawTabs(SpriteBatch spriteBatch)
        {
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            float tabY = 100f;
            float tabSpacing = 150f;
            float startX = GraphicsDevice.Viewport.Width / 2f - tabSpacing;

            string[] tabs = { "AUDIO (Q)", "VIDEO (W)", "CONTROLS (E)" };
            for (int i = 0; i < tabs.Length; i++)
            {
                float x = startX + i * tabSpacing;
                bool isSelected = (int)_currentTab == i;
                Color color = isSelected ? Color.Gold : Color.White;
                
                Vector2 tabSize = font.MeasureString(tabs[i]);
                spriteBatch.DrawString(font, tabs[i],
                    new Vector2(x - tabSize.X / 2, tabY),
                    color, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

                if (isSelected)
                {
                    Rectangle underline = new Rectangle(
                        (int)(x - tabSize.X / 2),
                        (int)(tabY + 25),
                        (int)tabSize.X,
                        3);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, underline, Color.Gold);
                }
            }
        }

        private void DrawSettingsItems(SpriteBatch spriteBatch)
        {
            SpriteFont font = UIManager.DefaultFont ?? CreateDefaultFont();
            float itemHeight = 50f;
            float startY = 180f;
            float centerX = GraphicsDevice.Viewport.Width / 2f;

            string[] items = GetCurrentItems();
            for (int i = 0; i < items.Length; i++)
            {
                float y = startY + i * itemHeight;
                bool isSelected = (i == _selectedItem);
                
                // Фон выбранного элемента / Selected item background
                if (isSelected)
                {
                    Rectangle bgRect = new Rectangle(
                        (int)(centerX - 200),
                        (int)(y - 5),
                        400, 40);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, 
                        new Color(255, 255, 255, 50));
                }

                // Название / Name
                Color nameColor = isSelected ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, items[i],
                    new Vector2(centerX - 150, y),
                    nameColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                // Значение / Value
                string value = GetValueString(i);
                Vector2 valueSize = font.MeasureString(value);
                spriteBatch.DrawString(font, value,
                    new Vector2(centerX + 150 - valueSize.X, y),
                    Color.Cyan, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        private string GetValueString(int index)
        {
            switch (_currentTab)
            {
                case OptionsTab.Audio:
                    switch (index)
                    {
                        case 0: return $"{(int)(_masterVolume * 100)}%";
                        case 1: return $"{(int)(_musicVolume * 100)}%";
                        case 2: return $"{(int)(_sfxVolume * 100)}%";
                        case 3: return "";
                    }
                    break;
                case OptionsTab.Video:
                    switch (index)
                    {
                        case 0: return _fullscreen ? "ON" : "OFF";
                        case 1: return _resolutions[_resolutionIndex];
                        case 2: return _vsync ? "ON" : "OFF";
                        case 3: return "";
                    }
                    break;
                case OptionsTab.Controls:
                    return GetControlBinding(index);
            }
            return "";
        }

        private string GetControlBinding(int index)
        {
            switch (index)
            {
                case 0: return "SPACE";      // Jump
                case 1: return "F";          // Focus
                case 2: return "A / LEFT";   // Move Left
                case 3: return "D / RIGHT";  // Move Right
                case 4: return "";           // Back
            }
            return "";
        }

        private SpriteFont CreateDefaultFont() => null;
    }
}
