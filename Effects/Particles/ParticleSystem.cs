// ============================================================================
// ParticleSystem.cs - Система частиц / Particle system
// Для эффектов шлейфа, искр, снега и т.д.
// For trail effects, sparks, snow, etc.
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Effects.Particles
{
    public class ParticleSystem
    {
        private List<Particle> _particles = new List<Particle>();
        private Texture2D _particleTexture;
        private Vector2 _emitPosition;
        private int _maxParticles = 100;
        private ParticleSettings _currentSettings;

        public ParticleSystem(Texture2D texture, int maxParticles = 100)
        {
            _particleTexture = texture;
            _maxParticles = maxParticles;
        }

        public void SetEmitPosition(Vector2 position)
        {
            _emitPosition = position;
        }

        public void Emit(int count, ParticleSettings settings)
        {
            _currentSettings = settings;
            for (int i = 0; i < count; i++)
            {
                if (_particles.Count >= _maxParticles)
                {
                    _particles.RemoveAt(0);
                }

                var particle = new Particle
                {
                    Position = _emitPosition + new Vector2(
                        (float)Random.Shared.NextDouble() - 0.5f,
                        (float)Random.Shared.NextDouble() - 0.5f) * settings.Spread,
                    Velocity = new Vector2(
                        (float)(Random.Shared.NextDouble() * 2 - 1) * settings.VelocityRange.X,
                        (float)(Random.Shared.NextDouble() * 2 - 1) * settings.VelocityRange.Y) + settings.BaseVelocity,
                    Color = settings.StartColor,
                    Size = settings.StartSize,
                    Lifetime = settings.Lifetime,
                    Age = 0f
                };

                _particles.Add(particle);
            }
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var p = _particles[i];
                p.Age += delta;
                p.Position += p.Velocity * delta;
                p.Velocity += _currentSettings.Acceleration * delta;

                float lifePercent = 1f - (p.Age / p.Lifetime);
                p.Color = Color.Lerp(_currentSettings.EndColor, _currentSettings.StartColor, lifePercent);
                p.Size = MathHelper.Lerp(_currentSettings.EndSize, _currentSettings.StartSize, lifePercent);

                if (p.Age >= p.Lifetime)
                {
                    _particles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
            {
                spriteBatch.Draw(
                    _particleTexture,
                    particle.Position,
                    null,
                    particle.Color,
                    0f,
                    new Vector2(_particleTexture.Width / 2f, _particleTexture.Height / 2f),
                    particle.Size / _particleTexture.Width,
                    SpriteEffects.None,
                    0f);
            }
        }

        public void Clear()
        {
            _particles.Clear();
        }

        public int ParticleCount => _particles.Count;
    }

    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Size;
        public float Lifetime;
        public float Age;
    }

    public struct ParticleSettings
    {
        public Vector2 BaseVelocity;
        public Vector2 VelocityRange;
        public Vector2 Acceleration;
        public float Spread;
        public float Lifetime;
        public float StartSize;
        public float EndSize;
        public Color StartColor;
        public Color EndColor;

        public static ParticleSettings Default => new ParticleSettings
        {
            BaseVelocity = Vector2.Zero,
            VelocityRange = new Vector2(50, 50),
            Acceleration = Vector2.Zero,
            Spread = 10f,
            Lifetime = 0.5f,
            StartSize = 8f,
            EndSize = 0f,
            StartColor = Color.White,
            EndColor = Color.Transparent
        };

        public static ParticleSettings Trail => new ParticleSettings
        {
            BaseVelocity = new Vector2(-100, 0),
            VelocityRange = new Vector2(20, 20),
            Acceleration = new Vector2(0, 50),
            Spread = 5f,
            Lifetime = 0.3f,
            StartSize = 6f,
            EndSize = 0f,
            StartColor = new Color(128, 0, 128, 200),
            EndColor = Color.Transparent
        };

        public static ParticleSettings Focus => new ParticleSettings
        {
            BaseVelocity = new Vector2(0, -50),
            VelocityRange = new Vector2(30, 30),
            Acceleration = Vector2.Zero,
            Spread = 20f,
            Lifetime = 0.8f,
            StartSize = 10f,
            EndSize = 0f,
            StartColor = new Color(0, 255, 255, 150),
            EndColor = Color.Transparent
        };
    }
}
