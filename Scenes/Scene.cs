// ============================================================================
// Scene.cs - Базовый класс сцены / Base scene class
// Абстрактный класс для всех сцен игры
// Abstract class for all game scenes
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Managers;

namespace PurpleLordPlatformer.Scenes
{
    /// <summary>
    /// Абстрактный базовый класс для всех сцен.
    /// Abstract base class for all scenes.
    /// </summary>
    public abstract class Scene
    {
        // Ссылки на менеджеры / Manager references
        protected Game Game { get; private set; }
        protected ContentManager ContentManager { get; private set; }
        protected InputManager InputManager { get; private set; }
        protected AudioManager AudioManager { get; private set; }
        protected EffectManager EffectManager { get; private set; }
        protected UIManager UIManager { get; private set; }
        
        // Камера сцены / Scene camera
        public Camera2D Camera { get; protected set; }
        
        // Цвет фона сцены / Scene background color
        public virtual Color BackgroundColor => Color.Black;
        
        // Идентификатор сцены / Scene identifier
        public abstract string SceneId { get; }
        
        // Название сцены / Scene name
        public abstract string SceneName { get; }

        /// <summary>
        /// Инициализация сцены. Вызывается при создании.
        /// Scene initialization. Called on creation.
        /// </summary>
        public virtual void Initialize(Game game, ContentManager contentManager,
            InputManager inputManager, AudioManager audioManager,
            EffectManager effectManager, UIManager uiManager)
        {
            Game = game;
            ContentManager = contentManager;
            InputManager = inputManager;
            AudioManager = audioManager;
            EffectManager = effectManager;
            UIManager = uiManager;
            
            // Создание камеры / Create camera
            Camera = new Camera2D(game.GraphicsDevice.Viewport);
        }

        /// <summary>
        /// Загрузка контента сцены.
        /// Scene content loading.
        /// </summary>
        public virtual void LoadContent()
        {
            // Загрузка контента сцены / Load scene content
        }

        /// <summary>
        /// Выгрузка контента сцены.
        /// Scene content unloading.
        /// </summary>
        public virtual void Unload()
        {
            // Выгрузка контента / Unload content
        }

        /// <summary>
        /// Обновление логики сцены.
        /// Scene logic update.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Обновление камеры / Update camera
            Camera?.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка сцены.
        /// Scene drawing.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Отрисовка фона / Draw background
            DrawBackground(spriteBatch, gameTime);
        }

        /// <summary>
        /// Отрисовка фона сцены.
        /// Scene background drawing.
        /// </summary>
        protected virtual void DrawBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // По умолчанию - сплошной цвет / Default - solid color
        }

        /// <summary>
        /// Приостановка сцены (для паузы).
        /// Pause scene (for pause menu).
        /// </summary>
        public virtual void Pause()
        {
            // Приостановить звуки / Pause sounds
            AudioManager?.PauseAll();
        }

        /// <summary>
        /// Возобновление сцены.
        /// Resume scene.
        /// </summary>
        public virtual void Resume()
        {
            // Возобновить звуки / Resume sounds
            AudioManager?.ResumeAll();
        }
    }
}
