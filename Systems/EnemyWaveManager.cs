// ============================================================================
// EnemyWaveManager.cs - Менеджер волн врагов / Enemy wave manager
// Спавн врагов волнами с нарастающей сложностью
// Spawn enemies in waves with increasing difficulty
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PurpleLordPlatformer.Entities.Enemies;

namespace PurpleLordPlatformer.Systems
{
    public class EnemyWaveManager
    {
        private List<Wave> _waves = new List<Wave>();
        private int _currentWaveIndex = 0;
        private bool _isWaveActive = false;
        private float _waveDelayTimer = 0f;
        private float _waveDelay = 3f;
        
        private List<Enemy> _spawnedEnemies = new List<Enemy>();
        
        public event Action<int> OnWaveStart;
        public event Action<int> OnWaveComplete;
        public event Action OnAllWavesComplete;

        public void AddWave(Wave wave)
        {
            _waves.Add(wave);
        }

        public void Start()
        {
            if (_waves.Count > 0)
            {
                StartWave(0);
            }
        }

        private void StartWave(int index)
        {
            if (index >= _waves.Count)
            {
                OnAllWavesComplete?.Invoke();
                return;
            }

            _currentWaveIndex = index;
            _isWaveActive = true;
            Wave wave = _waves[index];

            // Спавн врагов / Spawn enemies
            foreach (var enemyConfig in wave.EnemyConfigs)
            {
                Enemy enemy = enemyConfig.CreateEnemy();
                enemy.OnDeath += (e) => OnEnemyDeath(e);
                _spawnedEnemies.Add(enemy);
            }

            OnWaveStart?.Invoke(index + 1);
        }

        public void Update(GameTime gameTime)
        {
            if (!_isWaveActive) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверка завершения волны / Check wave completion
            bool allDead = true;
            foreach (var enemy in _spawnedEnemies)
            {
                if (!enemy.IsDead)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                _isWaveActive = false;
                _spawnedEnemies.Clear();
                _waveDelayTimer = 0f;
                OnWaveComplete?.Invoke(_currentWaveIndex + 1);

                // Следующая волна / Next wave
                if (_currentWaveIndex + 1 < _waves.Count)
                {
                    _waveDelayTimer = _waveDelay;
                }
                else
                {
                    OnAllWavesComplete?.Invoke();
                }
            }

            // Таймер до следующей волны / Timer to next wave
            if (_waveDelayTimer > 0)
            {
                _waveDelayTimer -= delta;
                if (_waveDelayTimer <= 0)
                {
                    StartWave(_currentWaveIndex + 1);
                }
            }
        }

        private void OnEnemyDeath(Enemy enemy)
        {
            // Обработка смерти врага / Handle enemy death
        }

        public void Reset()
        {
            _currentWaveIndex = 0;
            _isWaveActive = false;
            _waveDelayTimer = 0f;
            foreach (var enemy in _spawnedEnemies)
            {
                enemy.OnDeath -= OnEnemyDeath;
            }
            _spawnedEnemies.Clear();
        }

        public int CurrentWave => _currentWaveIndex + 1;
        public int TotalWaves => _waves.Count;
        public bool IsWaveActive => _isWaveActive;
        public bool IsAllWavesComplete => _currentWaveIndex >= _waves.Count;
    }

    public class Wave
    {
        public List<EnemyConfig> EnemyConfigs { get; set; } = new List<EnemyConfig>();
        public string WaveName { get; set; } = "";

        public void AddEnemy(EnemyConfig config)
        {
            EnemyConfigs.Add(config);
        }
    }

    public class EnemyConfig
    {
        public Vector2 Position { get; set; }
        public EnemyType Type { get; set; }
        public EnemyBehavior Behavior { get; set; }
        public Vector2 PatrolRange { get; set; }
        public float PatrolSpeed { get; set; } = 1f;

        public virtual Enemy CreateEnemy()
        {
            Enemy enemy = new Enemy(Vector2.Zero, Type, Behavior)
            {
                Width = 40,
                Height = 40
            };
            enemy.SetPatrolParams(PatrolRange, PatrolSpeed);
            return enemy;
        }
    }

    public class BugEnemyConfig : EnemyConfig
    {
        public override Enemy CreateEnemy()
        {
            var enemy = new BugEnemy(Position, Behavior);
            enemy.SetPatrolParams(PatrolRange, PatrolSpeed);
            return enemy;
        }
    }

    public class NeuronEnemyConfig : EnemyConfig
    {
        public bool IsTrap { get; set; } = false;

        public override Enemy CreateEnemy()
        {
            var enemy = new NeuronEnemy(Position) { IsTrap = IsTrap };
            enemy.SetPatrolParams(PatrolRange, PatrolSpeed);
            return enemy;
        }
    }
}
