// ============================================================================
// Game1.cs - Главный класс игры / Main game class
// Purple Lord Platformer - 2D платформер о выборе пути в IT
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Managers;
using PurpleLordPlatformer.Scenes;
using PurpleLordPlatformer.Systems;
using PurpleLordPlatformer.UI;

namespace PurpleLordPlatformer
{
    /// <summary>
    /// Главный класс игры, унаследованный от MonoGame.Game.
    /// Инициализирует все менеджеры, сервисы и запускает игровой цикл.
    ///
    /// Main game class inherited from MonoGame.Game.
    /// Initializes all managers, services and runs the game loop.
    /// </summary>
    public class Game1 : Game
    {
        // Графический адаптер и спрайт-батч / Graphics adapter and sprite batch
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Менеджеры / Managers
        private SceneManager _sceneManager;
        private Managers.ContentManager _contentManager;
        private InputManager _inputManager;
        private AudioManager _audioManager;
        private EffectManager _effectManager;
        
        // Пост-обработка / Post-processing
        private PostProcessingSystem _postProcessingSystem;
        
        // UI менеджер / UI Manager
        private UIManager _uiManager;
        
        // Состояние игры / Game state
        private Core.GameState _gameState;
        
        // Конструктор / Constructor
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content = new Microsoft.Xna.Framework.Content.ContentManager(this);
            
            // Настройка окна / Window settings
            Window.Title = "Purple Lord: Path of Choices | Фиолетовый Лорд: Путь Выбора";
            IsMouseVisible = true;
            
            // Настройка графики / Graphics settings
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.ApplyChanges();
            
            // Инициализация состояния / Initialize game state
            _gameState = new GameState();
        }

        /// <summary>
        /// Инициализация игры. Создание и настройка всех менеджеров.
        /// Game initialization. Create and configure all managers.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            
            // Инициализация менеджеров / Initialize managers
            _contentManager = new Managers.ContentManager(Content, _graphics);
            _inputManager = new InputManager();
            _audioManager = new AudioManager(Content);
            _effectManager = new EffectManager(GraphicsDevice);
            _postProcessingSystem = new PostProcessingSystem(GraphicsDevice);
            _uiManager = new UIManager(this, GraphicsDevice, Content);
            
            // Инициализация менеджера сцен / Initialize scene manager
            _sceneManager = new SceneManager(this, _contentManager, _inputManager, 
                _audioManager, _effectManager, _uiManager);
            
            // Загрузка начальной сцены (главное меню) / Load initial scene (main menu)
            _sceneManager.LoadScene<MainMenuScene>();
            
            // Подписка на события / Subscribe to events
            _sceneManager.OnSceneChanged += OnSceneChanged;
            
            // Инициализация пост-обработки / Initialize post-processing
            _postProcessingSystem.Initialize();
        }

        /// <summary>
        /// Загрузка контента. Вызывается после Initialize.
        /// Content loading. Called after Initialize.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Создание спрайт-батча / Create sprite batch
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Загрузка контента через менеджер / Load content through manager
            _contentManager.LoadAllContent();
            
            // Инициализация пост-эффектов / Initialize post-effects
            _postProcessingSystem.LoadContent();
        }

        /// <summary>
        /// Обновление игровой логики. Вызывается каждый кадр.
        /// Update game logic. Called every frame.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Обновление менеджера ввода / Update input manager
            _inputManager.Update(gameTime);
            
            // Проверка выхода из игры / Check for game exit
            if (_inputManager.IsKeyDown(Keys.Escape) || 
                _inputManager.IsGamePadButtonDown(Buttons.Back))
                Exit();
            
            // Обновление текущей сцены / Update current scene
            _sceneManager.Update(gameTime);
            
            // Обновление пост-обработки / Update post-processing
            _postProcessingSystem.Update(gameTime, _sceneManager.CurrentScene);
            
            // Обновление UI / Update UI
            _uiManager.Update(gameTime);
            
            // Обновление состояния игры / Update game state
            _gameState.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка игры. Вызывается каждый кадр.
        /// Draw game. Called every frame.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана с цветом фона текущей сцены / Clear screen with current scene background color
            Color clearColor = _sceneManager.CurrentScene?.BackgroundColor ?? Color.Black;
            GraphicsDevice.Clear(clearColor);
            
            // Начало отрисовки с пост-обработкой / Begin drawing with post-processing
            _postProcessingSystem.BeginDraw();
            
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                _sceneManager.CurrentScene?.Camera?.GetTransformMatrix() ?? Matrix.Identity
            );
            
            // Отрисовка текущей сцены / Draw current scene
            _sceneManager.Draw(_spriteBatch, gameTime);
            
            _spriteBatch.End();
            
            // Отрисовка UI поверх всего / Draw UI on top
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _uiManager.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
            
            // Применение пост-обработки / Apply post-processing
            _postProcessingSystem.EndDraw(_spriteBatch);
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// Обработчик события смены сцены.
        /// Scene change event handler.
        /// </summary>
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            // Логирование смены сцены / Log scene change
            System.Diagnostics.Debug.WriteLine(
                $"Scene changed: {oldScene?.GetType().Name} -> {newScene?.GetType().Name}");
            
            // Сброс пост-эффектов при смене сцены / Reset post-effects on scene change
            _postProcessingSystem.ResetEffects();
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// Release resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contentManager?.Dispose();
                _sceneManager?.Dispose();
                _audioManager?.Dispose();
                _effectManager?.Dispose();
                _postProcessingSystem?.Dispose();
                _uiManager?.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #region Properties | Свойства
        
        /// <summary>
        /// Доступ к менеджеру сцен из других классов.
        /// Access to scene manager from other classes.
        /// </summary>
        public SceneManager SceneManager => _sceneManager;
        
        /// <summary>
        /// Доступ к менеджеру контента.
        /// Access to content manager.
        /// </summary>
        public ContentManager ContentManager => _contentManager;
        
        /// <summary>
        /// Доступ к менеджеру ввода.
        /// Access to input manager.
        /// </summary>
        public InputManager InputManager => _inputManager;
        
        /// <summary>
        /// Доступ к аудио менеджеру.
        /// Access to audio manager.
        /// </summary>
        public AudioManager AudioManager => _audioManager;
        
        /// <summary>
        /// Доступ к графическому устройству.
        /// Access to graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice => GraphicsDevice;
        
        #endregion
    }
}
