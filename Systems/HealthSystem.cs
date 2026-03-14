// ============================================================================
// HealthSystem.cs - Система здоровья / Health system
// Управление здоровьем игрока и уроном
// Health and damage management
// ============================================================================

using System;
using Microsoft.Xna.Framework;

namespace PurpleLordPlatformer.Systems
{
    /// <summary>
    /// Компонент здоровья для игровых объектов.
    /// Health component for game objects.
    /// </summary>
    public class HealthComponent
    {
        private int _currentHealth;
        private int _maxHealth;
        private float _invincibilityTimer = 0f;
        private const float InvincibilityDuration = 1.5f;

        public event Action<int> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnHeal;

        public HealthComponent(int maxHealth = 3)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void Update(GameTime gameTime)
        {
            if (_invincibilityTimer > 0)
            {
                _invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_invincibilityTimer < 0) _invincibilityTimer = 0;
            }
        }

        public void TakeDamage(int damage)
        {
            if (_invincibilityTimer > 0) return;

            _currentHealth -= damage;
            _invincibilityTimer = InvincibilityDuration;

            OnDamageTaken?.Invoke(damage);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;

            OnHeal?.Invoke();
        }

        public void Reset()
        {
            _currentHealth = _maxHealth;
            _invincibilityTimer = 0;
        }

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public float HealthPercent => (float)_currentHealth / _maxHealth;
        public bool IsInvincible => _invincibilityTimer > 0;
        public float InvincibilityRemaining => _invincibilityTimer;

        public bool IsAlive => _currentHealth > 0;
    }

    /// <summary>
    /// Типы урона в игре.
    /// Damage types in game.
    /// </summary>
    public enum DamageType
    {
        Normal,       // Обычный урон / Normal damage
        Spike,        // Шипы / Spikes
        Enemy,        // Враг / Enemy
        Fall,         // Падение / Fall damage
        Laser,        // Лазер / Laser
        Magic         // Магия / Magic
    }

    /// <summary>
    /// Данные о уроне.
    /// Damage data.
    /// </summary>
    public struct DamageInfo
    {
        public int Amount { get; set; }
        public DamageType Type { get; set; }
        public Vector2 Source { get; set; }
        public bool CanBeBlocked { get; set; }
    }
}
