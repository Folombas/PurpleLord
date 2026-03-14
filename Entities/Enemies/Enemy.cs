// ============================================================================
// Enemy.cs - Базовый класс врага / Base enemy class
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Entities.Enemies
{
    public enum EnemyType
    {
        Bug,          // Баг - жук с глазами браузера / Browser bug
        Neuron,       // Нейрон - призрачный враг / Ghost neuron
        Coffee,       // Кружка кофе - летающий враг / Flying coffee mug
        Server        // Сервер - стационарная опасность / Stationary hazard
    }

    public enum EnemyBehavior
    {
        Stationary,   // Стоит на месте / Stays in place
        Patrol,       // Патрулирует область / Patrols area
        Chase,        // Преследует игрока / Chases player
        Flying        // Летает по траектории / Flies along trajectory
    }

    public abstract class Enemy : GameObject
    {
        protected EnemyType _type;
        protected EnemyBehavior _behavior;
        protected Texture2D _texture;
        protected Color _tintColor = Color.White;
        
        protected Vector2 _startPosition;
        protected Vector2 _patrolRange;
        protected float _patrolSpeed;
        protected float _patrolTimer;
        protected int _damage = 1;
        protected bool _isDead = false;

        protected Enemy(Vector2 position, EnemyType type, EnemyBehavior behavior)
            : base(position)
        {
            _type = type;
            _behavior = behavior;
            _startPosition = position;
            Tag = "Enemy";
        }

        public override void Update(GameTime gameTime)
        {
            if (_isDead) return;

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

        protected virtual void UpdateStationary(GameTime gameTime)
        {
            // Ничего не делаем / Do nothing
        }

        protected virtual void UpdatePatrol(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _patrolTimer += delta * _patrolSpeed;
            
            Position.X = _startPosition.X + (float)Math.Sin(_patrolTimer) * _patrolRange.X;
            Position.Y = _startPosition.Y + (float)Math.Abs(Math.Cos(_patrolTimer)) * _patrolRange.Y;
        }

        protected virtual void UpdateChase(GameTime gameTime)
        {
            // Будет переопределено в подклассах / Will be overridden in subclasses
        }

        protected virtual void UpdateFlying(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _patrolTimer += delta * _patrolSpeed;
            
            Position.X = _startPosition.X + (float)Math.Sin(_patrolTimer) * _patrolRange.X;
            Position.Y = _startPosition.Y + (float)Math.Sin(_patrolTimer * 2f) * _patrolRange.Y;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_isDead) return;

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
        }

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

        protected Color GetTypeColor()
        {
            switch (_type)
            {
                case EnemyType.Bug:
                    return new Color(100, 200, 50);    // Green bug
                case EnemyType.Neuron:
                    return new Color(200, 100, 255);   // Purple neuron
                case EnemyType.Coffee:
                    return new Color(150, 100, 50);    // Brown coffee
                case EnemyType.Server:
                    return new Color(50, 50, 50);      // Dark server
                default:
                    return Color.Red;
            }
        }

        public virtual void TakeDamage(int damage)
        {
            // Некоторые враги могут быть неуязвимы / Some enemies may be invulnerable
        }

        public void Kill()
        {
            _isDead = true;
            OnDeath?.Invoke(this);
        }

        public event Action<Enemy> OnDeath;

        public bool IsDead => _isDead;
        public int Damage => _damage;
        public EnemyType Type => _type;

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }

        public void SetPatrolParams(Vector2 range, float speed)
        {
            _patrolRange = range;
            _patrolSpeed = speed;
        }
    }
}
