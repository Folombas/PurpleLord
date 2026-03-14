// ============================================================================
// SceneTransition.cs - Переходы между сценами / Scene transitions
// Эффекты плавного появления и исчезновения
// Fade in/out effects
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Core
{
    public class SceneTransition
    {
        private TransitionState _state = TransitionState.None;
        private float _timer = 0f;
        private float _duration = 0.5f;
        private Action _onComplete;

        public void StartFadeOut(float duration, Action onComplete = null)
        {
            _state = TransitionState.FadingOut;
            _timer = 0f;
            _duration = duration;
            _onComplete = onComplete;
        }

        public void StartFadeIn(float duration, Action onComplete = null)
        {
            _state = TransitionState.FadingIn;
            _timer = 0f;
            _duration = duration;
            _onComplete = onComplete;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_state)
            {
                case TransitionState.FadingOut:
                    _timer += delta;
                    if (_timer >= _duration)
                    {
                        _timer = _duration;
                        _state = TransitionState.FadedOut;
                        _onComplete?.Invoke();
                    }
                    break;

                case TransitionState.FadingIn:
                    _timer += delta;
                    if (_timer >= _duration)
                    {
                        _timer = _duration;
                        _state = TransitionState.None;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state == TransitionState.None) return;

            float alpha = GetAlpha();
            if (alpha <= 0) return;

            Rectangle screen = new Rectangle(0, 0,
                GraphicsDeviceManager.DefaultBackBufferWidth,
                GraphicsDeviceManager.DefaultBackBufferHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, 
                new Color(0, 0, 0, (int)(alpha * 255)));
        }

        private float GetAlpha()
        {
            switch (_state)
            {
                case TransitionState.FadingOut:
                    return _timer / _duration;
                case TransitionState.FadingIn:
                    return 1f - (_timer / _duration);
                case TransitionState.FadedOut:
                    return 1f;
                default:
                    return 0f;
            }
        }

        public bool IsTransitioning => _state != TransitionState.None;
        public bool IsFadedOut => _state == TransitionState.FadedOut;

        public void Reset()
        {
            _state = TransitionState.None;
            _timer = 0f;
        }
    }

    public enum TransitionState
    {
        None,
        FadingIn,
        FadingOut,
        FadedOut
    }
}
