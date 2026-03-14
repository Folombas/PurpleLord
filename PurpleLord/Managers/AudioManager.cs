// ============================================================================
// AudioManager.cs - Менеджер аудио (музыка и звуковые эффекты)
// Audio Manager (music and sound effects)
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace PurpleLord.Managers;

/// <summary>
/// Управление воспроизведением музыки и звуковых эффектов
/// Managing playback of music and sound effects
/// </summary>
public class AudioManager : IDisposable
{
    private readonly Dictionary<string, SoundEffect> _soundEffects = new();
    private readonly Dictionary<string, Song> _musicTracks = new();
    
    private SoundEffectInstance? _currentMusic;
    private bool _isDisposed;

    /// <summary>
    /// Общая громкость (от 0.0 до 1.0)
    /// Master volume (0.0 to 1.0)
    /// </summary>
    public float MasterVolume { get; set; } = 1.0f;

    /// <summary>
    /// Громкость музыки (от 0.0 до 1.0)
    /// Music volume (0.0 to 1.0)
    /// </summary>
    public float MusicVolume { get; set; } = 0.5f;

    /// <summary>
    /// Громкость звуковых эффектов (от 0.0 до 1.0)
    /// Sound effects volume (0.0 to 1.0)
    /// </summary>
    public float SFXVolume { get; set; } = 0.7f;

    /// <summary>
    /// Загрузка аудио контента
    /// Load audio content
    /// </summary>
    public void LoadContent(ContentManager content)
    {
        // Загрузка будет добавляться по мере создания ассетов
        // Content will be added as assets are created
    }

    /// <summary>
    /// Загрузка звукового эффекта
    /// Load sound effect
    /// </summary>
    public void LoadSoundEffect(string name, string assetPath)
    {
        if (!_soundEffects.ContainsKey(name))
        {
            var sound = content.Load<SoundEffect>(assetPath);
            _soundEffects[name] = sound;
        }
    }

    /// <summary>
    /// Загрузка музыкального трека
    /// Load music track
    /// </summary>
    public void LoadMusic(string name, string assetPath)
    {
        if (!_musicTracks.ContainsKey(name))
        {
            var song = content.Load<Song>(assetPath);
            _musicTracks[name] = song;
        }
    }

    /// <summary>
    /// Воспроизведение звукового эффекта
    /// Play sound effect
    /// </summary>
    public void PlaySound(string name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
    {
        if (_soundEffects.TryGetValue(name, out var sound))
        {
            var adjustedVolume = volume * SFXVolume * MasterVolume;
            sound.Play(adjustedVolume, pitch, pan);
        }
    }

    /// <summary>
    /// Воспроизведение звука с возвратом экземпляра (для контроля)
    /// Play sound with instance return (for control)
    /// </summary>
    public SoundEffectInstance? PlaySoundInstance(string name)
    {
        if (_soundEffects.TryGetValue(name, out var sound))
        {
            var instance = sound.CreateInstance();
            instance.Volume = SFXVolume * MasterVolume;
            instance.Play();
            return instance;
        }
        return null;
    }

    /// <summary>
    /// Воспроизведение музыки с зацикливанием
    /// Play music with looping
    /// </summary>
    public void PlayMusic(string name, bool loop = true)
    {
        if (_musicTracks.TryGetValue(name, out var song))
        {
            _currentMusic?.Stop();
            
            // Примечание: MonoGame не поддерживает прямое воспроизведение Song
            // Note: MonoGame doesn't support direct Song playback
            // Это заготовка для будущей реализации
            // This is a placeholder for future implementation
        }
    }

    /// <summary>
    /// Остановка текущей музыки
    /// Stop current music
    /// </summary>
    public void StopMusic()
    {
        _currentMusic?.Stop();
        _currentMusic = null;
    }

    /// <summary>
    /// Пауза музыки
    /// Pause music
    /// </summary>
    public void PauseMusic()
    {
        if (_currentMusic?.State == SoundState.Playing)
        {
            _currentMusic.Pause();
        }
    }

    /// <summary>
    /// Возобновление музыки
    /// Resume music
    /// </summary>
    public void ResumeMusic()
    {
        if (_currentMusic?.State == SoundState.Paused)
        {
            _currentMusic.Resume();
        }
    }

    /// <summary>
    /// Плавное изменение громкости музыки (fade)
    /// Smooth music volume fade
    /// </summary>
    public void FadeMusic(float targetVolume, float duration)
    {
        // Будет реализовано в Update
        // Will be implemented in Update
    }

    /// <summary>
    /// Обновление аудио менеджера
    /// Update audio manager
    /// </summary>
    public void Update(float deltaTime)
    {
        // Здесь можно реализовать fade эффекты
        // Fade effects can be implemented here
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _currentMusic?.Dispose();
            
            foreach (var sound in _soundEffects.Values)
            {
                sound.Dispose();
            }
            
            _soundEffects.Clear();
            _musicTracks.Clear();
            _isDisposed = true;
        }
    }
}
