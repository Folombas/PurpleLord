// ============================================================================
// SceneManager.cs - Менеджер сцен и уровней
// Scene and Level Manager
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PurpleLord.Entities.Player;

namespace PurpleLord.Managers;

/// <summary>
/// Управляет загрузкой и переключением между уровнями
/// Manages loading and switching between levels
/// </summary>
public class SceneManager
{
    private readonly Game _game;
    private readonly ContentManager _content;
    
    /// <summary>
    /// Текущий активный уровень
    /// Current active level
    /// </summary>
    public Level? CurrentLevel { get; private set; }

    /// <summary>
    /// Список загруженных уровней
    /// List of loaded levels
    /// </summary>
    private readonly Dictionary<string, Level> _loadedLevels = new();

    /// <summary>
    /// Текущее название уровня
    /// Current level name
    /// </summary>
    public string CurrentLevelName { get; private set; } = string.Empty;

    public SceneManager(Game game)
    {
        _game = game;
        _content = game.Content;
    }

    /// <summary>
    /// Загрузка уровня по названию
    /// Load level by name
    /// </summary>
    public void LoadLevel(string levelName)
    {
        // Если уровень уже загружен, используем кэш
        // If level already loaded, use cache
        if (_loadedLevels.TryGetValue(levelName, out var cachedLevel))
        {
            CurrentLevel = cachedLevel;
            CurrentLevelName = levelName;
            CurrentLevel.OnEnter();
            return;
        }

        // Создание нового уровня
        // Create new level
        CurrentLevel = levelName switch
        {
            "FrontendForest" => new FrontendForestLevel(_game),
            "BackendBadlands" => new BackendBadlandsLevel(_game),
            "NeuralNetworkNebula" => new NeuralNetworkNebulaLevel(_game),
            "JuniorOffice" => new JuniorOfficeLevel(_game),
            _ => new FrontendForestLevel(_game)
        };

        _loadedLevels[levelName] = CurrentLevel;
        CurrentLevelName = levelName;
        CurrentLevel.LoadContent(_content);
        CurrentLevel.OnEnter();
    }

    /// <summary>
    /// Обновление текущего уровня
    /// Update current level
    /// </summary>
    public void Update(float deltaTime)
    {
        CurrentLevel?.Update(deltaTime);
    }

    /// <summary>
    /// Отрисовка фона уровня
    /// Draw level background
    /// </summary>
    public void DrawBackground(SpriteBatch spriteBatch)
    {
        CurrentLevel?.DrawBackground(spriteBatch);
    }

    /// <summary>
    /// Отрисовка уровня
    /// Draw level
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        CurrentLevel?.Draw(spriteBatch);
    }

    /// <summary>
    /// Проверка коллизий игрока с объектами уровня
    /// Check player collisions with level objects
    /// </summary>
    public void CheckCollisions(Player player)
    {
        CurrentLevel?.CheckCollisions(player);
    }

    /// <summary>
    /// Переход к следующему уровню
    /// Transition to next level
    /// </summary>
    public void GoToNextLevel()
    {
        // Логика переключения на следующий уровень
        // Logic for switching to next level
    }

    /// <summary>
    /// Перезагрузка текущего уровня
    /// Reload current level
    /// </summary>
    public void ReloadLevel()
    {
        if (!string.IsNullOrEmpty(CurrentLevelName))
        {
            LoadLevel(CurrentLevelName);
        }
    }
}
