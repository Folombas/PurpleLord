// ============================================================================
// ContentLoadManager.cs - Менеджер загрузки контента
// Content Load Manager
// ============================================================================

using Microsoft.Xna.Framework.Content;

namespace PurpleLord.Managers;

/// <summary>
/// Централизованная загрузка всех игровых ассетов
/// Centralized loading of all game assets
/// </summary>
public class ContentLoadManager : IDisposable
{
    private readonly ContentManager _content;
    private bool _isDisposed;

    /// <summary>
    /// Кэш загруженных текстур
    /// Cache of loaded textures
    /// </summary>
    public Dictionary<string, object> LoadedAssets { get; } = new();

    public ContentLoadManager(ContentManager content)
    {
        _content = content;
    }

    /// <summary>
    /// Загрузка всего контента
    /// Load all content
    /// </summary>
    public void LoadAll()
    {
        // Загрузка будет добавляться по мере создания ассетов
        // Content will be added as assets are created
        // Это заготовка для будущей системы загрузки
        // This is a placeholder for future loading system
    }

    /// <summary>
    /// Загрузка текстуры по названию
    /// Load texture by name
    /// </summary>
    public T Load<T>(string assetName) where T : class
    {
        if (LoadedAssets.TryGetValue(assetName, out var cached))
        {
            return (T)cached;
        }

        var asset = _content.Load<T>(assetName);
        LoadedAssets[assetName] = asset;
        return asset;
    }

    /// <summary>
    /// Выгрузка конкретного ассета
    /// Unload specific asset
    /// </summary>
    public void Unload(string assetName)
    {
        if (LoadedAssets.ContainsKey(assetName))
        {
            LoadedAssets.Remove(assetName);
        }
    }

    /// <summary>
    /// Полная выгрузка контента
    /// Unload all content
    /// </summary>
    public void UnloadAll()
    {
        LoadedAssets.Clear();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            UnloadAll();
            _isDisposed = true;
        }
    }
}
