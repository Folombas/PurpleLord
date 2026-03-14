// ============================================================================
// Checkpoint.cs - Точка сохранения / Checkpoint
// Место возрождения игрока после смерти
// Player respawn point after death
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Entities.Objects
{
    public class Checkpoint : GameObject
    {
        private bool _isActivated = false;
        private float _pulseTimer = 0f;
        private float _pulseSpeed = 2f;
        private Texture2D _texture;

        public Checkpoint(Vector2 position) : base(position)
        {
            Width = 40;
            Height = 60;
            Tag = "Checkpoint";
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _pulseTimer += delta * _pulseSpeed;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float pulse = (float)Math.Sin(_pulseTimer) * 0.3f + 0.7f;
            Color color = _isActivated ? 
                new Color(0, 255, 100, (int)(255 * pulse)) : 
                new Color(100, 100, 100, (int)(200 * pulse));

            if (_texture != null)
            {
                spriteBatch.Draw(_texture, Position, null, color, 0f,
                    new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                DrawCheckpointPlaceholder(spriteBatch, color);
            }
        }

        private void DrawCheckpointPlaceholder(SpriteBatch spriteBatch, Color color)
        {
            // Флаг чекпоинта / Checkpoint flag
            float x = Position.X;
            float y = Position.Y;

            // Шест / Pole
            Rectangle pole = new Rectangle(
                (int)(x - 3),
                (int)(y - Height / 2),
                6,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, pole, color);

            // Флаг / Flag
            if (!_isActivated)
            {
                // Серый флаг (не активирован) / Gray flag (not activated)
                Rectangle flag = new Rectangle(
                    (int)(x),
                    (int)(y - Height / 2 + 10),
                    30,
                    20);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, flag, 
                    new Color(100, 100, 100, 200));
            }
            else
            {
                // Зелёный флаг (активирован) / Green flag (activated)
                Rectangle flag = new Rectangle(
                    (int)(x),
                    (int)(y - Height / 2 + 10),
                    30,
                    20);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, flag,
                    new Color(0, 255, 100, 200));

                // Частицы вокруг / Particles around
                DrawParticles(spriteBatch);
            }
        }

        private void DrawParticles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 5; i++)
            {
                float angle = _pulseTimer + i * MathHelper.Pi * 2 / 5;
                float radius = 25 + (float)Math.Sin(_pulseTimer * 3 + i) * 5;
                Vector2 particlePos = Position + new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);

                spriteBatch.Draw(GraphicsUtils.WhiteTexture, particlePos, null,
                    new Color(0, 255, 100, 150), 0f, Vector2.Zero,
                    3f, SpriteEffects.None, 0f);
            }
        }

        public void Activate()
        {
            if (!_isActivated)
            {
                _isActivated = true;
                _pulseSpeed = 4f;
                OnActivate?.Invoke(this);
            }
        }

        public Vector2 GetSpawnPosition()
        {
            return new Vector2(Position.X, Position.Y - 20);
        }

        public event Action<Checkpoint> OnActivate;

        public bool IsActivated => _isActivated;
    }
}
