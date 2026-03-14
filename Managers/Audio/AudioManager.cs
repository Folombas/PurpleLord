// ============================================================================
// AudioManager.cs - Менеджер аудио / Audio manager
// Воспроизведение музыки и звуковых эффектов
// Music and sound effects playback
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace PurpleLordPlatformer.Managers
{
    public class AudioManager : IDisposable
    {
        private ContentManager _content;
        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private Dictionary<string, SoundEffectInstance> _music = new Dictionary<string, SoundEffectInstance>();
        private float _masterVolume = 1f;
        private float _musicVolume = 0.5f;
        private float _sfxVolume = 1f;

        public AudioManager(ContentManager content)
        {
            _content = content;
        }

        public void LoadSound(string name, string path)
        {
            try
            {
                _sounds[name] = _content.Load<SoundEffect>(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load sound '{path}': {ex.Message}");
            }
        }

        public void LoadMusic(string name, string path, bool isLooped = true)
        {
            try
            {
                var effect = _content.Load<SoundEffect>(path);
                _music[name] = effect.CreateInstance();
                _music[name].IsLooped = isLooped;
                _music[name].Volume = _musicVolume * _masterVolume;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load music '{path}': {ex.Message}");
            }
        }

        public void PlaySound(string name)
        {
            if (_sounds.ContainsKey(name))
            {
                var instance = _sounds[name].CreateInstance();
                instance.Volume = _sfxVolume * _masterVolume;
                instance.Play();
            }
        }

        public void PlayMusic(string name)
        {
            StopAllMusic();
            if (_music.ContainsKey(name))
            {
                _music[name].Play();
            }
        }

        public void StopAllMusic()
        {
            foreach (var track in _music.Values)
            {
                track.Stop();
            }
        }

        public void PauseAll()
        {
            foreach (var track in _music.Values)
            {
                if (track.State == SoundState.Playing)
                    track.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var track in _music.Values)
            {
                if (track.State == SoundState.Paused)
                    track.Resume();
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = MathHelper.Clamp(volume, 0f, 1f);
            UpdateVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = MathHelper.Clamp(volume, 0f, 1f);
            UpdateVolumes();
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = MathHelper.Clamp(volume, 0f, 1f);
        }

        private void UpdateVolumes()
        {
            foreach (var track in _music.Values)
            {
                track.Volume = _musicVolume * _masterVolume;
            }
        }

        public void Dispose()
        {
            StopAllMusic();
            foreach (var sound in _sounds.Values)
            {
                sound.Dispose();
            }
            _sounds.Clear();
            _music.Clear();
        }
    }
}
