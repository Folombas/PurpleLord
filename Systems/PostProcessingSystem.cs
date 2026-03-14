// ============================================================================
// PostProcessingSystem.cs - Система пост-обработки
// Эффекты: размытие, рыбий глаз, расфокусировка, хроматическая аберрация
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Scenes;
using PurpleLordPlatformer.Core;

namespace PurpleLordPlatformer.Systems
{
    public class PostProcessingSystem : IDisposable
    {
        private GraphicsDevice _graphicsDevice;
        private RenderTarget2D _sceneRenderTarget;
        private RenderTarget2D _blurRenderTarget;
        private Effect _grayscaleEffect;
        private Effect _blurEffect;
        private Effect _chromaticAberrationEffect;
        private Effect _fisheyeEffect;
        
        // Параметры эффектов / Effect parameters
        private float _blurAmount = 0f;
        private float _fisheyeAmount = 0f;
        private float _chromaticAmount = 0f;
        private float _desaturation = 0f;
        private float _doubleImageOffset = 0f;
        
        // Расфокусировка / Focus loss
        private bool _isFocusLossActive = false;
        private float _focusLossIntensity = 0f;

        public PostProcessingSystem(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Initialize()
        {
            // Создание рендер-таргетов / Create render targets
            RecreateRenderTargets();
            
            // Подписка на событие изменения размера / Subscribe to resize event
            _graphicsDevice.DeviceResetting += (s, e) => RecreateRenderTargets();
        }

        private void RecreateRenderTargets()
        {
            _sceneRenderTarget?.Dispose();
            _blurRenderTarget?.Dispose();

            _sceneRenderTarget = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);

            _blurRenderTarget = new RenderTarget2D(
                _graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        public void LoadContent()
        {
            // Загрузка эффектов / Load effects
            // В реальном проекте загрузить из .fx файлов
            // In real project, load from .fx files
            CreateDefaultEffects();
        }

        private void CreateDefaultEffects()
        {
            // Создаём простые эффекты программно / Create simple effects programmatically
            // В реальном проекте использовать Effect.FromFile
        }

        public void Update(GameTime gameTime, Scene currentScene)
        {
            // Обновление параметров от состояния игры / Update parameters from game state
            if (currentScene is Levels.Level level)
            {
                var player = level.Player;
                if (player != null)
                {
                    // Эффект фокуса / Focus effect
                    if (player.IsFocusing)
                    {
                        _fisheyeAmount = MathHelper.Lerp(_fisheyeAmount, 0.3f, 0.1f);
                        _blurAmount = MathHelper.Lerp(_blurAmount, 0.5f, 0.05f);
                        _desaturation = MathHelper.Lerp(_desaturation, 0.5f, 0.05f);
                    }
                    else
                    {
                        _fisheyeAmount = MathHelper.Lerp(_fisheyeAmount, 0f, 0.05f);
                        _blurAmount = MathHelper.Lerp(_blurAmount, 0f, 0.05f);
                        _desaturation = MathHelper.Lerp(_desaturation, 0f, 0.05f);
                    }
                }
            }

            // Обновление расфокусировки / Update focus loss
            UpdateFocusLoss(gameTime);
        }

        private void UpdateFocusLoss(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isFocusLossActive)
            {
                _focusLossIntensity = MathHelper.Min(_focusLossIntensity + delta * 0.1f, 1f);
                _chromaticAmount = _focusLossIntensity * 0.02f;
                _doubleImageOffset = _focusLossIntensity * 5f;
                _desaturation = MathHelper.Max(_desaturation, _focusLossIntensity * 0.3f);
            }
            else
            {
                _focusLossIntensity = MathHelper.Max(_focusLossIntensity - delta * 0.2f, 0f);
                if (_focusLossIntensity <= 0.01f)
                {
                    _chromaticAmount = 0f;
                    _doubleImageOffset = 0f;
                }
            }
        }

        public void BeginDraw()
        {
            _graphicsDevice.SetRenderTarget(_sceneRenderTarget);
            _graphicsDevice.Clear(Color.Black);
        }

        public void EndDraw(SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);

            // Применение пост-обработки / Apply post-processing
            ApplyPostProcessing(spriteBatch);
        }

        private void ApplyPostProcessing(SpriteBatch spriteBatch)
        {
            // Если эффекты не загружены, рисуем напрямую / If effects not loaded, draw directly
            if (_sceneRenderTarget == null) return;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
                SamplerState.Linear, DepthStencilState.None, RasterizerState.CullNone);

            // Простая отрисовка без эффектов (заглушка)
            // Simple drawing without effects (placeholder)
            spriteBatch.Draw(_sceneRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        public void ResetEffects()
        {
            _blurAmount = 0f;
            _fisheyeAmount = 0f;
            _chromaticAmount = 0f;
            _desaturation = 0f;
            _doubleImageOffset = 0f;
            _isFocusLossActive = false;
            _focusLossIntensity = 0f;
        }

        public void SetFocusLossActive(bool active)
        {
            _isFocusLossActive = active;
        }

        public void TriggerScreenShake(float magnitude, float duration)
        {
            // Тряска экрана / Screen shake
        }

        public void SetBlurAmount(float amount)
        {
            _blurAmount = MathHelper.Clamp(amount, 0f, 1f);
        }

        public void SetDesaturation(float amount)
        {
            _desaturation = MathHelper.Clamp(amount, 0f, 1f);
        }

        public void Dispose()
        {
            _sceneRenderTarget?.Dispose();
            _blurRenderTarget?.Dispose();
            _grayscaleEffect?.Dispose();
            _blurEffect?.Dispose();
            _chromaticAberrationEffect?.Dispose();
            _fisheyeEffect?.Dispose();
        }
    }
}
