// ============================================================================
// Enemy.cs - Базовый класс врага / Base enemy class
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLord.Core;

namespace PurpleLord.Entities.Enemies
{
    /// <summary>
    /// Типы врагов / Enemy types
    /// </summary>
    public enum EnemyType
    {
        Basic,        // Базовый враг / Basic enemy
        Flying,       // Летающий враг / Flying enemy
        Boss,         // Босс / Boss
        Mushroom      // Гриб мухомор / Poisonous mushroom
    }
    
    /// <summary>
    /// Поведение врага / Enemy behavior
    /// </summary>
    public enum EnemyBehavior
    {
        Stationary,   // Стоит на месте / Stays in place
        Patrol,       // Патрулирует / Patrols area
        Chase,        // Преследует игрока / Chases player
        Flying        // Летает по траектории / Flies along trajectory
    }
    
    /// <summary>
    /// Базовый класс для всех врагов.
    /// Base class for all enemies.
    /// </summary>
    public abstract class Enemy : GameObject
    {
        protected EnemyType _type;
        protected EnemyBehavior _behavior;
        protected int _damage = 1;
        protected int _health = 3;
        protected bool _isDead = false;
        
        // Параметры патрулирования / Patrol parameters
        protected Vector2 _startPosition;
        protected Vector2 _patrolRange;
        protected float _patrolSpeed = 2f;
        protected float _patrolTimer = 0f;
        
        // Визуальные параметры / Visual parameters
        protected Color _tintColor = Color.White;
        protected Texture2D _texture;
        
        /// <summary>
        /// Конструктор врага.
        /// Enemy constructor.
        /// </summary>
        protected Enemy(Vector2 position, EnemyType type, EnemyBehavior behavior)
            : base(position)
        {
            _type = type;
            _behavior = behavior;
            _startPosition = position;
            Tag = "Enemy";
            Width = 40;
            Height = 40;
        }
        
        /// <summary>
        /// Обновление врага.
        /// Update enemy.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (!IsActive || _isDead) return;
            
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            switch (_behavior)
            {
                case EnemyBehavior.Stationary:
                    UpdateStationary(gameTime);
                    break;
                case EnemyBehavior.Patrol:
                    UpdatePatrol(gameTime);
                    break;
                case EnemyBehavior.Chase:
                    UpdateChase(gameTime);
                    break;
                case EnemyBehavior.Flying:
                    UpdateFlying(gameTime);
                    break;
            }
            
            base.Update(gameTime);
        }
        
        /// <summary>
        /// Обновление для стационарного врага.
        /// Update for stationary enemy.
        /// </summary>
        protected virtual void UpdateStationary(GameTime gameTime)
        {
            // Ничего не делаем / Do nothing
        }
        
        /// <summary>
        /// Обновление для патрулирующего врага.
        /// Update for patrolling enemy.
        /// </summary>
        protected virtual void UpdatePatrol(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _patrolTimer += delta * _patrolSpeed;

            float newX = _startPosition.X + (float)Math.Sin(_patrolTimer) * _patrolRange.X;
            float newY = _startPosition.Y + (float)Math.Abs(Math.Cos(_patrolTimer)) * _patrolRange.Y;
            Position = new Vector2(newX, newY);
        }
        
        /// <summary>
        /// Обновление для преследующего врага.
        /// Update for chasing enemy.
        /// </summary>
        protected virtual void UpdateChase(GameTime gameTime)
        {
            // Будет переопределено в подклассах / Will be overridden in subclasses
        }
        
        /// <summary>
        /// Обновление для летающего врага.
        /// Update for flying enemy.
        /// </summary>
        protected virtual void UpdateFlying(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _patrolTimer += delta * _patrolSpeed;

            float newX = _startPosition.X + (float)Math.Sin(_patrolTimer) * _patrolRange.X;
            float newY = _startPosition.Y + (float)Math.Sin(_patrolTimer * 2f) * _patrolRange.Y;
            Position = new Vector2(newX, newY);
        }
        
        /// <summary>
        /// Отрисовка врага.
        /// Draw enemy.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsActive || _isDead) return;
            
            if (_texture != null)
            {
                spriteBatch.Draw(
                    _texture,
                    Position,
                    null,
                    _tintColor,
                    0f,
                    new Vector2(Width / 2, Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                DrawPlaceholder(spriteBatch);
            }
            
            base.Draw(spriteBatch, gameTime);
        }
        
        /// <summary>
        /// Отрисовка заглушки (для отладки).
        /// Draw placeholder (for debugging).
        /// </summary>
        protected virtual void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            Color color = GetTypeColor();
            Rectangle rect = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, color);
        }
        
        /// <summary>
        /// Получает цвет для типа врага.
        /// Gets color for enemy type.
        /// </summary>
        protected virtual Color GetTypeColor()
        {
            switch (_type)
            {
                case EnemyType.Basic:
                    return new Color(100, 200, 50);    // Green
                case EnemyType.Flying:
                    return new Color(200, 100, 255);   // Purple
                case EnemyType.Boss:
                    return new Color(255, 50, 50);     // Red
                case EnemyType.Mushroom:
                    return new Color(220, 50, 50);     // Mushroom red
                default:
                    return Color.Red;
            }
        }
        
        /// <summary>
        /// Нанести урон врагу.
        /// Deal damage to enemy.
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                Kill();
            }
        }
        
        /// <summary>
        /// Убить врага.
        /// Kill the enemy.
        /// </summary>
        public void Kill()
        {
            _isDead = true;
            IsActive = false;
            OnDeath?.Invoke(this);
        }
        
        /// <summary>
        /// Событие смерти врага.
        /// Enemy death event.
        /// </summary>
        public event Action<Enemy> OnDeath;
        
        // Свойства / Properties
        public bool IsDead => _isDead;
        public int Damage => _damage;
        public int Health => _health;
        public EnemyType Type => _type;
        public EnemyBehavior Behavior => _behavior;
        
        /// <summary>
        /// Установить параметры патрулирования.
        /// Set patrol parameters.
        /// </summary>
        public void SetPatrolParams(Vector2 range, float speed)
        {
            _patrolRange = range;
            _patrolSpeed = speed;
        }
        
        /// <summary>
        /// Установить текстуру.
        /// Set texture.
        /// </summary>
        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
        
        /// <summary>
        /// Установить урон.
        /// Set damage.
        /// </summary>
        public void SetDamage(int damage)
        {
            _damage = damage;
        }
    }
}
