// ============================================================================
// EffectManager.cs - Менеджер эффектов / Effect manager
// Управление визуальными эффектами и частицами
// Visual effects and particles management
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Managers
{
    public class EffectManager : IDisposable
    {
        private GraphicsDevice _graphicsDevice;
        private List<IParticleEffect> _particleEffects = new List<IParticleEffect>();
        private List<IScreenEffect> _screenEffects = new List<IScreenEffect>();

        public EffectManager(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void AddParticleEffect(IParticleEffect effect)
        {
            _particleEffects.Add(effect);
        }

        public void AddScreenEffect(IScreenEffect effect)
        {
            _screenEffects.Add(effect);
        }

        public void RemoveParticleEffect(IParticleEffect effect)
        {
            _particleEffects.Remove(effect);
        }

        public void ClearParticleEffects()
        {
            _particleEffects.Clear();
        }

        public void ClearScreenEffects()
        {
            _screenEffects.Clear();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _particleEffects.Count - 1; i >= 0; i--)
            {
                _particleEffects[i].Update(gameTime);
                if (_particleEffects[i].IsFinished)
                {
                    _particleEffects.RemoveAt(i);
                }
            }

            foreach (var effect in _screenEffects)
            {
                effect.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var effect in _particleEffects)
            {
                effect.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            ClearParticleEffects();
            ClearScreenEffects();
        }

        public GraphicsDevice GraphicsDevice => _graphicsDevice;
    }

    public interface IParticleEffect
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        bool IsFinished { get; }
    }

    public interface IScreenEffect
    {
        void Update(GameTime gameTime);
        float Intensity { get; }
    }
}
