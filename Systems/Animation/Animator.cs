// ============================================================================
// Animator.cs - Система анимации / Animation system
// Управление спрайт-анимациями
// Sprite animation management
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Systems.Animation
{
    public class Animator
    {
        private Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();
        private string _currentState = "";
        private Animation _currentAnimation;

        public void AddAnimation(string name, Animation animation)
        {
            _animations[name] = animation;
        }

        public void SetState(string state)
        {
            if (_currentState != state && _animations.ContainsKey(state))
            {
                _currentState = state;
                _currentAnimation = _animations[state];
                _currentAnimation.Reset();
            }
        }

        public void Update(GameTime gameTime)
        {
            _currentAnimation?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color? color = null)
        {
            if (_currentAnimation?.CurrentFrame != null)
            {
                spriteBatch.Draw(
                    _currentAnimation.CurrentFrame,
                    position,
                    null,
                    color ?? Color.White,
                    0f,
                    Origin,
                    1f,
                    SpriteEffects.None,
                    0f);
            }
        }

        public Texture2D CurrentTexture => _currentAnimation?.CurrentFrame;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public bool IsPlaying => _currentAnimation?.IsPlaying ?? false;
    }

    public class Animation
    {
        private List<Texture2D> _frames = new List<Texture2D>();
        private float _frameDuration;
        private float _timer = 0f;
        private int _currentFrameIndex = 0;
        private bool _isLooping = true;
        private bool _isPlaying = false;

        public Animation(List<Texture2D> frames, float frameDuration, bool isLooping = true)
        {
            _frames = frames;
            _frameDuration = frameDuration;
            _isLooping = isLooping;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isPlaying || _frames.Count == 0) return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _frameDuration)
            {
                _timer = 0f;
                _currentFrameIndex++;

                if (_currentFrameIndex >= _frames.Count)
                {
                    if (_isLooping)
                        _currentFrameIndex = 0;
                    else
                        _isPlaying = false;
                }
            }
        }

        public void Play()
        {
            _isPlaying = true;
            _currentFrameIndex = 0;
            _timer = 0f;
        }

        public void Stop()
        {
            _isPlaying = false;
            _currentFrameIndex = 0;
        }

        public void Reset()
        {
            Play();
        }

        public Texture2D CurrentFrame => _frames.Count > 0 ? _frames[_currentFrameIndex] : null;
        public bool IsPlaying => _isPlaying;
        public int FrameCount => _frames.Count;
    }
}
