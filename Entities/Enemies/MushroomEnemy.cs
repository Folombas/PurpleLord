// ============================================================================
// MushroomEnemy.cs - Ядовитый гриб мухомор / Poisonous mushroom enemy
// Стационарный враг, который испускает ядовитый газ и покачивается
// Stationary enemy that emits poison gas and sways side to side
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Entities.Enemies
{
    /// <summary>
    /// Ядовитый гриб мухомор. Стационарный враг, который:
    /// - Покачивается из стороны в сторону
    /// - Испускает зеленоватый ядовитый газ
    /// - Наносит урон при столкновении и накладывает эффект отравления
    /// </summary>
    public class MushroomEnemy : Enemy
    {
        // Параметры покачивания / Sway parameters
        private float _swayTimer = 0f;
        private float _swaySpeed = 2f;
        private float _swayAmount = 8f;
        private float _swayOffset = 0f;

        // Параметры ядовитого газа / Poison gas parameters
        private float _gasEmitTimer = 0f;
        private float _gasEmitInterval = 0.3f;
        private int _gasParticlesPerEmit = 3;
        
        // Частицы газа / Gas particles
        private PoisonGasParticle[] _gasParticles;
        private const int MaxParticles = 50;

        // Урон и отравление / Damage and poison
        private int _contactDamage = 1;
        private int _poisonDamage = 1;
        private float _poisonDuration = 3f;
        
        // Визуальные параметры / Visual parameters
        private float _pulseTimer = 0f;
        private float _pulseSpeed = 3f;
        private Color _capColor = new Color(220, 50, 50); // Красная шляпка
        private Color _stemColor = new Color(240, 220, 200); // Светлая ножка
        private Color _gasColor = new Color(100, 255, 100, 150); // Зеленоватый газ

        public MushroomEnemy(Vector2 position, EnemyBehavior behavior = EnemyBehavior.Stationary)
            : base(position, EnemyType.Neuron, behavior)
        {
            Width = 40;
            Height = 50;
            _damage = _contactDamage;
            Tag = "PoisonMushroom";
            
            // Инициализация частиц / Initialize particles
            _gasParticles = new PoisonGasParticle[MaxParticles];
            for (int i = 0; i < MaxParticles; i++)
            {
                _gasParticles[i] = new PoisonGasParticle();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_isDead) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Обновление покачивания / Update sway
            UpdateSway(delta);

            // Обновление частиц газа / Update gas particles
            UpdateGasParticles(delta);

            // Эмиссия газа / Emit gas
            _gasEmitTimer += delta;
            if (_gasEmitTimer >= _gasEmitInterval)
            {
                EmitGas();
                _gasEmitTimer = 0f;
            }

            // Обновление пульсации / Update pulse
            _pulseTimer += delta * _pulseSpeed;

            base.Update(gameTime);
        }

        private void UpdateSway(float delta)
        {
            _swayTimer += delta * _swaySpeed;
            _swayOffset = (float)Math.Sin(_swayTimer) * _swayAmount;
        }

        private void UpdateGasParticles(float delta)
        {
            foreach (var particle in _gasParticles)
            {
                if (particle.IsActive)
                {
                    particle.Update(delta);
                }
            }
        }

        private void EmitGas()
        {
            // Создаем частицы газа над грибом / Create gas particles above mushroom
            for (int i = 0; i < _gasParticlesPerEmit; i++)
            {
                foreach (var particle in _gasParticles)
                {
                    if (!particle.IsActive)
                    {
                        particle.Activate(
                            new Vector2(
                                Position.X + _swayOffset + (float)(Random.Shared.NextDouble() - 0.5) * 20,
                                Position.Y - Height / 4 + (float)(Random.Shared.NextDouble() - 0.5) * 10),
                            new Vector2(
                                (float)(Random.Shared.NextDouble() - 0.5) * 30,
                                -(float)Random.Shared.NextDouble() * 50 - 20),
                            _gasColor);
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_isDead) return;

            float alpha = 0.9f + (float)Math.Sin(_pulseTimer) * 0.1f;
            Color drawColor = new Color(
                (int)(_capColor.R * alpha),
                (int)(_capColor.G * alpha),
                (int)(_capColor.B * alpha));

            // Отрисовка ножки / Draw stem
            DrawStem(spriteBatch);

            // Отрисовка шляпки / Draw cap
            DrawCap(spriteBatch, drawColor);

            // Отрисовка белых точек на шляпке / Draw white spots
            DrawWhiteSpots(spriteBatch);

            // Отрисовка частиц газа / Draw gas particles
            DrawGasParticles(spriteBatch);

            // Отладочная отрисовка хитбокса / Debug hitbox
            if (DebugMode)
            {
                Rectangle bounds = Bounds;
                bounds.Offset((int)_swayOffset, 0);
                GraphicsUtils.DrawRectangle(spriteBatch, bounds, Color.Lime, 2);
            }
        }

        private void DrawStem(SpriteBatch spriteBatch)
        {
            float stemWidth = 14f;
            float stemHeight = Height / 2;
            
            Rectangle stemRect = new Rectangle(
                (int)(Position.X + _swayOffset - stemWidth / 2),
                (int)(Position.Y),
                (int)stemWidth,
                (int)stemHeight);
            
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, stemRect, _stemColor);

            // Юбочка на ножке / Ring on stem
            Rectangle skirtRect = new Rectangle(
                (int)(Position.X + _swayOffset - stemWidth),
                (int)(Position.Y + 8),
                (int)(stemWidth * 2),
                4);
            
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, skirtRect, new Color(255, 200, 200));
        }

        private void DrawCap(SpriteBatch spriteBatch, Color color)
        {
            float capRadius = Width / 2;
            Vector2 capCenter = new Vector2(Position.X + _swayOffset, Position.Y - Height / 4);

            // Рисуем полукруглую шляпку по частям / Draw semicircular cap in parts
            int segments = 8;
            for (int y = 0; y < capRadius; y++)
            {
                float xRange = (float)Math.Sqrt(Math.Max(0, capRadius * capRadius - y * y));
                int rowY = (int)(capCenter.Y - capRadius + y);
                int rowX = (int)(capCenter.X - xRange);
                int rowWidth = (int)(xRange * 2);

                if (rowWidth > 0)
                {
                    Rectangle rowRect = new Rectangle(rowX, rowY, rowWidth, 1);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, rowRect, color);
                }
            }
        }

        private void DrawWhiteSpots(SpriteBatch spriteBatch)
        {
            Vector2 capCenter = new Vector2(Position.X + _swayOffset, Position.Y - Height / 4);
            float capRadius = Width / 2;

            // Позиции точек на шляпке / Spot positions on cap
            Vector2[] spots = new Vector2[]
            {
                new Vector2(0, -capRadius * 0.3f),
                new Vector2(-capRadius * 0.5f, -capRadius * 0.1f),
                new Vector2(capRadius * 0.5f, -capRadius * 0.1f),
                new Vector2(-capRadius * 0.3f, capRadius * 0.2f),
                new Vector2(capRadius * 0.3f, capRadius * 0.2f),
                new Vector2(0, capRadius * 0.3f)
            };

            foreach (Vector2 spot in spots)
            {
                float spotSize = 3f + (float)Math.Sin(_pulseTimer + spot.X) * 1f;
                spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                    capCenter + spot,
                    null,
                    Color.White,
                    0f,
                    new Vector2(spotSize / 2, spotSize / 2),
                    spotSize,
                    SpriteEffects.None,
                    0f);
            }
        }

        private void DrawGasParticles(SpriteBatch spriteBatch)
        {
            foreach (var particle in _gasParticles)
            {
                if (particle.IsActive)
                {
                    particle.Draw(spriteBatch);
                }
            }
        }

        public void ApplyPoisonEffect(Entities.Player.Player player)
        {
            if (player != null && player.IsAlive)
            {
                player.ApplyPoison(_poisonDuration, _poisonDamage);
            }
        }

        protected override void UpdateStationary(GameTime gameTime)
        {
            // Мухоморы стационарны, но покачиваются / Mushrooms are stationary but sway
            // Логика в Update() / Logic in Update()
        }

        public override void TakeDamage(int damage)
        {
            // Мухоморы могут быть уничтожены / Mushrooms can be destroyed
            base.TakeDamage(damage);
        }

        public void SetPoisonParams(float duration, int damagePerTick)
        {
            _poisonDuration = duration;
            _poisonDamage = damagePerTick;
        }

        public void SetSwayParams(float speed, float amount)
        {
            _swaySpeed = speed;
            _swayAmount = amount;
        }

        public bool DebugMode { get; set; } = false;

        /// <summary>
        /// Простая система частиц для ядовитого газа
        /// Simple particle system for poison gas
        /// </summary>
        private class PoisonGasParticle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public Color Color;
            public float Lifetime;
            public float MaxLifetime;
            public float Size;
            public bool IsActive;

            public void Activate(Vector2 position, Vector2 velocity, Color color)
            {
                Position = position;
                Velocity = velocity;
                Color = color;
                MaxLifetime = 1.5f;
                Lifetime = MaxLifetime;
                Size = (float)(Random.Shared.NextDouble() * 8 + 4);
                IsActive = true;
            }

            public void Update(float delta)
            {
                if (!IsActive) return;

                Lifetime -= delta;
                if (Lifetime <= 0)
                {
                    IsActive = false;
                    return;
                }

                // Движение частицы / Particle motion
                Position += Velocity * delta;
                Velocity.Y -= 20f * delta; // Медленно всплывает / Slowly rises
                Velocity.X *= 0.98f; // Затухание по горизонтали / Horizontal damping

                // Затухание цвета / Color fade
                float alpha = Lifetime / MaxLifetime;
                Color = new Color(Color.R, Color.G, Color.B, (int)(alpha * 200));
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (!IsActive) return;

                spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                    Position,
                    null,
                    Color,
                    0f,
                    new Vector2(Size / 2, Size / 2),
                    Size,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}
