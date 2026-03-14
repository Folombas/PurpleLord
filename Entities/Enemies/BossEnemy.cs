// ============================================================================
// BossEnemy.cs - Базовый класс босса / Base boss class
// Особые враги с уникальными механиками
// Special enemies with unique mechanics
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities.Enemies;
using PurpleLordPlatformer.Systems;

namespace PurpleLordPlatformer.Entities.Enemies
{
    public enum BossPhase
    {
        Idle,
        Attack1,
        Attack2,
        Attack3,
        Special,
        Enraged
    }

    public abstract class BossEnemy : Enemy
    {
        protected BossPhase _currentPhase = BossPhase.Idle;
        protected float _phaseTimer = 0f;
        protected float _phaseDuration = 3f;
        protected int _maxHealth = 10;
        protected float _enrageThreshold = 0.3f; // 30% HP
        protected bool _isEnraged = false;

        protected BossEnemy(Vector2 position, EnemyType type)
            : base(position, type, EnemyBehavior.Stationary)
        {
            _health = new HealthComponent(_maxHealth);
        }

        public override void Update(GameTime gameTime)
        {
            if (_isDead) return;

            _health.Update(gameTime);
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _phaseTimer += delta;

            // Проверка на ярость / Check for enrage
            if (!_isEnraged && HealthPercent <= _enrageThreshold)
            {
                _isEnraged = true;
                OnEnrage?.Invoke();
            }

            // Смена фаз / Phase change
            if (_phaseTimer >= _phaseDuration)
            {
                ChangePhase();
                _phaseTimer = 0f;
            }

            UpdatePhase(gameTime);

            base.Update(gameTime);
        }

        protected virtual void ChangePhase()
        {
            // Переключается в подклассах / Overridden in subclasses
            switch (_currentPhase)
            {
                case BossPhase.Idle:
                    _currentPhase = BossPhase.Attack1;
                    break;
                case BossPhase.Attack1:
                    _currentPhase = BossPhase.Attack2;
                    break;
                case BossPhase.Attack2:
                    _currentPhase = BossPhase.Attack3;
                    break;
                case BossPhase.Attack3:
                    _currentPhase = _isEnraged ? BossPhase.Special : BossPhase.Idle;
                    break;
                case BossPhase.Special:
                    _currentPhase = BossPhase.Idle;
                    break;
            }
        }

        protected virtual void UpdatePhase(GameTime gameTime)
        {
            // Логика фазы / Phase logic
        }

        protected virtual void DrawHealthBar(SpriteBatch spriteBatch)
        {
            // Полоска здоровья босса / Boss health bar
            float barWidth = 400;
            float barHeight = 20;
            Vector2 pos = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth / 2f - barWidth / 2,
                20);

            // Фон / Background
            Rectangle bgRect = new Rectangle((int)pos.X, (int)pos.Y, (int)barWidth, (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bgRect, new Color(50, 0, 0));

            // Заполнение / Fill
            Rectangle fillRect = new Rectangle((int)pos.X, (int)pos.Y,
                (int)(barWidth * HealthPercent), (int)barHeight);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, fillRect,
                _isEnraged ? new Color(255, 100, 0) : Color.Red);

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bgRect, Color.White, 2);

            // Название босса / Boss name
            SpriteFont font = UIManager.DefaultFont;
            if (font != null)
            {
                string name = GetBossName();
                Vector2 nameSize = font.MeasureString(name);
                spriteBatch.DrawString(font, name,
                    new Vector2(pos.X + barWidth / 2 - nameSize.X / 2, pos.Y - 25),
                    Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            }
        }

        protected abstract string GetBossName();

        public override void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);
            OnDamageTaken?.Invoke(damage);

            if (_health.CurrentHealth <= 0)
            {
                Kill();
            }
        }

        public event Action<int> OnDamageTaken;
        public event Action OnDefeat;
        public event Action OnEnrage;

        public BossPhase CurrentPhase => _currentPhase;
        public bool IsEnraged => _isEnraged;
        public float HealthPercent => _maxHealth > 0 ? (float)CurrentHealth / _maxHealth : 0;
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _health != null ? _health.CurrentHealth : _maxHealth;
        
        protected HealthComponent _health;
    }

    // Пример босса для уровня 1 - Багомонстр / Example boss for level 1 - BugMonster
    public class BugMonsterBoss : BossEnemy
    {
        private Vector2 _targetPosition;
        private float _moveSpeed = 100f;

        public BugMonsterBoss(Vector2 position)
            : base(position, EnemyType.Bug)
        {
            _maxHealth = 15;
            Width = 120;
            Height = 80;
            _phaseDuration = 4f;
        }

        protected override void UpdatePhase(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_currentPhase)
            {
                case BossPhase.Idle:
                    // Ожидание / Waiting
                    break;

                case BossPhase.Attack1:
                    // Рывок к игроку / Dash to player
                    MoveToTarget(delta);
                    break;

                case BossPhase.Attack2:
                    // Выпуск миньонов / Spawn minions
                    break;

                case BossPhase.Attack3:
                    // Атака по области / Area attack
                    break;

                case BossPhase.Special:
                    // Быстрая атака / Fast attack
                    _moveSpeed = 200f;
                    MoveToTarget(delta);
                    break;
            }
        }

        private void MoveToTarget(float delta)
        {
            if (_targetPosition != Vector2.Zero)
            {
                Vector2 direction = _targetPosition - Position;
                if (direction.Length() > 10)
                {
                    direction.Normalize();
                    Position += direction * _moveSpeed * delta;
                }
            }
        }

        public void SetTarget(Vector2 target)
        {
            _targetPosition = target;
        }

        protected override string GetBossName() => "BugMonster | Багомонстр";

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Огромный жук / Giant bug
            Color bodyColor = _isEnraged ? new Color(255, 100, 50) : new Color(50, 150, 50);

            // Тело / Body
            Rectangle body = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, bodyColor);

            // Глаза / Eyes
            float eyeSize = 15f;
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(Position.X - 30), (int)(Position.Y - 20), (int)eyeSize, (int)eyeSize),
                Color.Red);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(Position.X + 15), (int)(Position.Y - 20), (int)eyeSize, (int)eyeSize),
                Color.Red);

            // Лапы / Legs
            for (int i = 0; i < 6; i++)
            {
                float legAngle = _phaseTimer + i * 0.5f;
                Vector2 legEnd = Position + new Vector2(
                    (float)Math.Cos(legAngle) * 60 - Width / 2,
                    (float)Math.Sin(legAngle) * 30 + Height / 2);

                GraphicsUtils.DrawLine(spriteBatch,
                    new Vector2(Position.X - Width / 2, Position.Y + Height / 4),
                    legEnd, bodyColor, 5);
            }

            // Полоска здоровья / Health bar
            DrawHealthBar(spriteBatch);
        }
    }
}
