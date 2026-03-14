// ============================================================================
// ContentManager.cs - Менеджер контента / Content manager
// Загрузка и кэширование текстур, звуков, шрифтов
// Loading and caching textures, sounds, fonts
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Managers
{
    /// <summary>
    /// Менеджер контента - централизованная загрузка и хранение ресурсов.
    /// Content manager - centralized loading and storage of resources.
    /// </summary>
    public class ContentManager : IDisposable
    {
        // Ссылка на MonoGame ContentManager
        private ContentManager _monoGameContent;
        
        // Графическое устройство / Graphics device
        private GraphicsDeviceManager _graphics;
        
        // Кэш текстур / Texture cache
        private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        
        // Кэш спрайтов (под-областей текстур) / Sprite cache (texture sub-regions)
        private Dictionary<string, SpriteData> _sprites = new Dictionary<string, SpriteData>();
        
        // Кэш эффектов / Effect cache
        private Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();

        /// <summary>
        /// Конструктор менеджера контента.
        /// Content manager constructor.
        /// </summary>
        public ContentManager(ContentManager monoGameContent, GraphicsDeviceManager graphics)
        {
            _monoGameContent = monoGameContent;
            _graphics = graphics;
        }

        /// <summary>
        /// Загрузка текстуры по пути.
        /// Load texture by path.
        /// </summary>
        public Texture2D LoadTexture(string path)
        {
            if (!_textures.ContainsKey(path))
            {
                try
                {
                    _textures[path] = _monoGameContent.Load<Texture2D>(path);
                    System.Diagnostics.Debug.WriteLine($"Loaded texture: {path}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to load texture '{path}': {ex.Message}");
                    // Создание заглушки / Create placeholder
                    _textures[path] = CreatePlaceholderTexture(64, 64, Color.Magenta);
                }
            }
            return _textures[path];
        }

        /// <summary>
        /// Загрузка спрайта (текстура + область).
        /// Load sprite (texture + region).
        /// </summary>
        public SpriteData LoadSprite(string path, Rectangle? sourceRect = null)
        {
            string key = path + (sourceRect.HasValue ? 
                $"_{sourceRect.Value}" : "");
            
            if (!_sprites.ContainsKey(key))
            {
                Texture2D texture = LoadTexture(path);
                Rectangle rect = sourceRect ?? new Rectangle(0, 0, texture.Width, texture.Height);
                
                _sprites[key] = new SpriteData
                {
                    Texture = texture,
                    SourceRect = rect,
                    Origin = new Vector2(rect.Width / 2f, rect.Height / 2f)
                };
            }
            return _sprites[key];
        }

        /// <summary>
        /// Загрузка эффекта (шейдера).
        /// Load effect (shader).
        /// </summary>
        public Effect LoadEffect(string path)
        {
            if (!_effects.ContainsKey(path))
            {
                try
                {
                    _effects[path] = _monoGameContent.Load<Effect>(path);
                    System.Diagnostics.Debug.WriteLine($"Loaded effect: {path}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to load effect '{path}': {ex.Message}");
                }
            }
            return _effects[path];
        }

        /// <summary>
        /// Создание текстуры-заглушки.
        /// Create placeholder texture.
        /// </summary>
        public Texture2D CreatePlaceholderTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(_graphics.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Создание текстуры из цвета.
        /// Create texture from color.
        /// </summary>
        public Texture2D CreateColorTexture(Color color, int width = 1, int height = 1)
        {
            Texture2D texture = new Texture2D(_graphics.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
                data[i] = color;
            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Загрузка всего контента.
        /// Load all content.
        /// </summary>
        public void LoadAllContent()
        {
            System.Diagnostics.Debug.WriteLine("Loading all content...");
            
            // Загрузка базовых текстур / Load base textures
            // Пути будут указаны относительно Content/
            // Paths are relative to Content/
            
            // Placeholder для игрока / Player placeholder
            LoadTexture("Sprites/player_placeholder");
            
            // Placeholder для земли / Ground placeholder
            LoadTexture("Sprites/ground_placeholder");
        }

        /// <summary>
        /// Получить текстуру из кэша.
        /// Get texture from cache.
        /// </summary>
        public Texture2D GetTexture(string path)
        {
            return _textures.ContainsKey(path) ? _textures[path] : null;
        }

        /// <summary>
        /// Получить спрайт из кэша.
        /// Get sprite from cache.
        /// </summary>
        public SpriteData GetSprite(string key)
        {
            return _sprites.ContainsKey(key) ? _sprites[key] : null;
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// Release resources.
        /// </summary>
        public void Dispose()
        {
            _textures.Clear();
            _sprites.Clear();
            _effects.Clear();
            _monoGameContent?.Unload();
        }
    }

    /// <summary>
    /// Данные спрайта - текстура + область + точка опоры.
    /// Sprite data - texture + region + origin point.
    /// </summary>
    public class SpriteData
    {
        public Texture2D Texture { get; set; }
        public Rectangle SourceRect { get; set; }
        public Vector2 Origin { get; set; }
        
        /// <summary>
        /// Ширина спрайта.
        /// Sprite width.
        /// </summary>
        public int Width => SourceRect.Width;
        
        /// <summary>
        /// Высота спрайта.
        /// Sprite height.
        /// </summary>
        public int Height => SourceRect.Height;
    }
}
