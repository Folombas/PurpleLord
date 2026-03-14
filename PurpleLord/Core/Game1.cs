// ============================================================================
// Game1.cs - Главный игровой цикл Purple Lord
// Main game loop for Purple Lord
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLord.Managers;
using PurpleLord.Entities.Player;
using PurpleLord.Systems.Input;
using PurpleLord.Systems.Particle;
using PurpleLord.Effects.PostProcessing;
using PurpleLord.UI.Menus;
using PurpleLord.UI.HUD;

namespace PurpleLord;

/// <summary>
/// Главный класс игры, наследующий MonoGame.Game
/// Main game class inheriting from MonoGame.Game
/// </summary>
public class Game1 : Game
{
    #region Fields

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    // Менеджеры / Managers
    private SceneManager _sceneManager = null!;
    private ContentLoadManager _contentManager = null!;
    private InputManager _inputManager = null!;
    private AudioManager _audioManager = null!;
    private ParticleManager _particleManager = null!;
    private FocusManager _focusManager = null!;
    private SaveManager _saveManager = null!;

    // Пост-обработка / Post-processing
    private PostProcessingManager _postProcessing = null!;

    // Игрок / Player
    private Player _player = null!;

    // UI компоненты / UI Components
    private MainMenu _mainMenu = null!;
    private GameHUD _gameHUD = null!;

    // Настройки экрана / Screen settings
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;

    #endregion

    #region Properties

    /// <summary>
    /// Глобальный доступ к SpriteBatch
    /// Global access to SpriteBatch
    /// </summary>
    public static SpriteBatch SpriteBatchInstance = null!;

    /// <summary>
    /// Глобальный доступ к GraphicsDevice
    /// Global access to GraphicsDevice
    /// </summary>
    public static GraphicsDevice GraphicsDeviceInstance = null!;

    /// <summary>
    /// Текущее состояние игры (меню, игра, пауза)
    /// Current game state (menu, playing, paused)
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.Menu;

    /// <summary>
    /// Менеджер сцен для доступа из других классов
    /// Scene manager for access from other classes
    /// </summary>
    public static SceneManager SceneManagerInstance = null!;

    /// <summary>
    /// Менеджер фокуса для доступа из других классов
    /// Focus manager for access from other classes
    /// </summary>
    public static FocusManager FocusManagerInstance = null!;

    #endregion

    #region Constructor

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        // Настройка разрешения экрана / Screen resolution setup
        _graphics.PreferredBackBufferWidth = ScreenWidth;
        _graphics.PreferredBackBufferHeight = ScreenHeight;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Purple Lord: Искатель в Цифровой Вселенной";
        
        // Включение сглаживания / Enable anti-aliasing
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
    }

    #endregion

    #region Initialize

    /// <summary>
    /// Инициализация всех менеджеров и систем
    /// Initialize all managers and systems
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        // Инициализация менеджеров / Initialize managers
        _contentManager = new ContentLoadManager(Content);
        _inputManager = new InputManager();
        _audioManager = new AudioManager();
        _particleManager = new ParticleManager();
        _focusManager = new FocusManager();
        _saveManager = new SaveManager();
        _sceneManager = new SceneManager(this);
        _postProcessing = new PostProcessingManager(GraphicsDevice);

        // Сохранение глобальных ссылок / Save global references
        SceneManagerInstance = _sceneManager;
        FocusManagerInstance = _focusManager;

        // Инициализация игрока / Initialize player
        _player = new Player(this);

        // Инициализация UI / Initialize UI
        _mainMenu = new MainMenu(this);
        _gameHUD = new GameHUD(this);

        // Загрузка настроек / Load settings
        LoadSettings();
    }

    #endregion

    #region LoadContent

    /// <summary>
    /// Загрузка всего контента (текстуры, звуки, шрифты)
    /// Load all content (textures, sounds, fonts)
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SpriteBatchInstance = _spriteBatch;
        GraphicsDeviceInstance = GraphicsDevice;

        // Загрузка контента через менеджер / Load content through manager
        _contentManager.LoadAll();

        // Загрузка пост-обработки / Load post-processing
        _postProcessing.LoadContent(Content);

        // Инициализация аудио / Initialize audio
        _audioManager.LoadContent(Content);

        // Загрузка первого уровня / Load first level
        _sceneManager.LoadLevel("FrontendForest");
    }

    #endregion

    #region Update

    /// <summary>
    /// Обновление игровой логики
    /// Update game logic
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        // Обработка выхода из игры / Handle game exit
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Обновление менеджеров / Update managers
        _inputManager.Update();
        _focusManager.Update(deltaTime);
        _audioManager.Update(deltaTime);

        // Обновление в зависимости от состояния / Update based on state
        switch (CurrentState)
        {
            case GameState.Menu:
                _mainMenu.Update(deltaTime);
                break;

            case GameState.Playing:
                UpdateGameplay(deltaTime);
                break;

            case GameState.Paused:
                UpdatePaused(deltaTime);
                break;
        }

        // Обновление пост-обработки / Update post-processing
        _postProcessing.Update(deltaTime, _focusManager);

        base.Update(gameTime);
    }

    /// <summary>
    /// Обновление игрового процесса
    /// Update gameplay
    /// </summary>
    private void UpdateGameplay(float deltaTime)
    {
        // Применение замедления времени при активном фокусе
        // Apply time slowdown when focus is active
        float timeScale = _focusManager.IsFocusActive ? 0.3f : 1.0f;
        float scaledDelta = deltaTime * timeScale;

        _player.Update(scaledDelta);
        _particleManager.Update(deltaTime);
        _sceneManager.Update(deltaTime);
        _gameHUD.Update(deltaTime);

        // Проверка коллизий игрока с объектами уровня
        // Check player collisions with level objects
        _sceneManager.CurrentLevel?.CheckCollisions(_player);
    }

    /// <summary>
    /// Обновление в режиме паузы
    /// Update in pause mode
    /// </summary>
    private void UpdatePaused(float deltaTime)
    {
        // Обработка меню паузы / Handle pause menu
        if (_inputManager.IsKeyPressed(Keys.Escape))
        {
            CurrentState = GameState.Playing;
            IsMouseVisible = false;
        }
    }

    #endregion

    #region Draw

    /// <summary>
    /// Отрисовка всего игрового мира
    /// Draw all game world
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // Начало отрисовки спрайтов / Begin sprite drawing
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            _postProcessing.CurrentEffect);

        // Отрисовка в зависимости от состояния / Draw based on state
        switch (CurrentState)
        {
            case GameState.Menu:
                _mainMenu.Draw(_spriteBatch);
                break;

            case GameState.Playing:
            case GameState.Paused:
                DrawGameplay();
                if (CurrentState == GameState.Paused)
                {
                    DrawPauseOverlay();
                }
                break;
        }

        _spriteBatch.End();

        // Отрисовка UI поверх всего (без пост-обработки)
        // Draw UI on top (without post-processing)
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
            SamplerState.LinearClamp);
        
        if (CurrentState == GameState.Playing)
        {
            _gameHUD.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Отрисовка игрового процесса
    /// Draw gameplay
    /// </summary>
    private void DrawGameplay()
    {
        // Отрисовка фона уровня / Draw level background
        _sceneManager.CurrentLevel?.DrawBackground(_spriteBatch);

        // Отрисовка объектов уровня / Draw level objects
        _sceneManager.CurrentLevel?.Draw(_spriteBatch);

        // Отрисовка игрока / Draw player
        _player.Draw(_spriteBatch);

        // Отрисовка частиц / Draw particles
        _particleManager.Draw(_spriteBatch);
    }

    /// <summary>
    /// Отрисовка overlay паузы
    /// Draw pause overlay
    /// </summary>
    private void DrawPauseOverlay()
    {
        // Полупрозрачный фон / Semi-transparent background
        _spriteBatch.Draw(
            Texture2D.White,
            new Rectangle(0, 0, ScreenWidth, ScreenHeight),
            Color.Black * 0.7f);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Загрузка настроек из файла
    /// Load settings from file
    /// </summary>
    private void LoadSettings()
    {
        var settings = _saveManager.LoadSettings();
        
        if (settings != null)
        {
            _graphics.PreferredBackBufferWidth = settings.ScreenWidth;
            _graphics.PreferredBackBufferHeight = settings.ScreenHeight;
            _graphics.IsFullScreen = settings.IsFullScreen;
            _graphics.ApplyChanges();
            
            _audioManager.MasterVolume = settings.MasterVolume;
        }
    }

    /// <summary>
    /// Переключение в полноэкранный режим
    /// Toggle fullscreen mode
    /// </summary>
    public void ToggleFullscreen()
    {
        _graphics.IsFullScreen = !_graphics.IsFullScreen;
        _graphics.ApplyChanges();
    }

    /// <summary>
    /// Установка состояния игры
    /// Set game state
    /// </summary>
    public void SetGameState(GameState state)
    {
        CurrentState = state;
        IsMouseVisible = state == GameState.Menu || state == GameState.Paused;
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Освобождение ресурсов
    /// Dispose resources
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _contentManager?.Dispose();
            _audioManager?.Dispose();
            _postProcessing?.Dispose();
            _saveManager?.SaveSettings();
        }
        base.Dispose(disposing);
    }

    #endregion
}

/// <summary>
/// Перечисление состояний игры
/// Game state enumeration
/// </summary>
public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Victory
}
