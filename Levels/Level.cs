// ============================================================================
// Level.cs - Базовый класс уровня / Base level class
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;
using PurpleLordPlatformer.Entities.Player;
using PurpleLordPlatformer.Entities.Objects;
using PurpleLordPlatformer.Entities.Enemies;
using PurpleLordPlatformer.Core;
using PurpleLordPlatformer.Managers;
using PurpleLordPlatformer.Scenes;

namespace PurpleLordPlatformer.Levels
{
    public abstract class Level : Scene
    {
        protected Player _player;
        protected List<Platform> _platforms = new List<Platform>();
        protected List<KnowledgeItem> _knowledgeItems = new List<KnowledgeItem>();
        protected List<Enemy> _enemies = new List<Enemy>();
        protected List<GameObject> _triggers = new List<GameObject>();
        protected List<Door> _doors = new List<Door>();
        
        protected PhysicsSystem.PhysicsSystem _physicsSystem;
        protected LevelStats _stats;
        
        protected Vector2 _playerSpawnPoint;
        protected Rectangle _levelBounds;
        protected int _knowledgeToUnlock = 0;
        protected bool _isLevelComplete = false;

        public override void Initialize(Game game, ContentManager contentManager,
            InputManager inputManager, AudioManager audioManager,
            EffectManager effectManager, UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            _physicsSystem = new PhysicsSystem.PhysicsSystem();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            CreateLevel();
            InitializePlayer();
            SetupCamera();
            LoadLevelMusic();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_isLevelComplete) return;

            // Обновление игрока / Update player
            _player?.Update(gameTime);

            // Обновление объектов / Update objects
            foreach (var platform in _platforms) platform.Update(gameTime);
            foreach (var knowledge in _knowledgeItems) knowledge.Update(gameTime);
            foreach (var enemy in _enemies) enemy.Update(gameTime);

            // Обновление физики / Update physics
            _physicsSystem.Update(gameTime);

            // Проверка коллизий / Check collisions
            CheckCollisions();

            // Обновление камеры / Update camera
            if (_player != null)
            {
                Camera.SetTarget(_player.Position);
            }

            // Проверка завершения уровня / Check level completion
            CheckLevelComplete();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Отрисовка фона уровня / Draw level background
            DrawLevelBackground(spriteBatch, gameTime);

            // Отрисовка объектов / Draw objects
            foreach (var platform in _platforms) platform.Draw(spriteBatch, gameTime);
            foreach (var knowledge in _knowledgeItems) knowledge.Draw(spriteBatch, gameTime);
            foreach (var enemy in _enemies) enemy.Draw(spriteBatch, gameTime);
            foreach (var door in _doors) door.Draw(spriteBatch, gameTime);

            // Отрисовка игрока / Draw player
            _player?.Draw(spriteBatch, gameTime);

            // Отрисовка переднего плана / Draw foreground
            DrawLevelForeground(spriteBatch, gameTime);
        }

        protected virtual void CreateLevel()
        {
            // Переопределяется в подклассах / Overridden in subclasses
        }

        protected virtual void InitializePlayer()
        {
            _player = new Player(_playerSpawnPoint);
            _physicsSystem.AddObject(_player);
        }

        protected virtual void SetupCamera()
        {
            Camera.Bounds = _levelBounds;
        }

        protected virtual void LoadLevelMusic()
        {
            // Переопределяется в подклассах / Overridden in subclasses
        }

        protected virtual void DrawLevelBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Параллакс фон / Parallax background
        }

        protected virtual void DrawLevelForeground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Передний план / Foreground
        }

        protected virtual void CheckCollisions()
        {
            if (_player == null) return;

            // Коллизии с платформами / Platform collisions
            _player.SetGrounded(false);
            foreach (var platform in _platforms)
            {
                if (_player.Intersects(platform))
                {
                    ResolvePlatformCollision(_player, platform);
                }
            }

            // Коллизии с предметами знаний / Knowledge item collisions
            foreach (var knowledge in _knowledgeItems)
            {
                if (!knowledge.IsCollected && _player.Intersects(knowledge))
                {
                    knowledge.Collect();
                    _stats.KnowledgeCollected++;
                }
            }

            // Коллизии с врагами / Enemy collisions
            foreach (var enemy in _enemies)
            {
                if (!enemy.IsDead && _player.Intersects(enemy))
                {
                    HandleEnemyCollision(enemy);
                }
            }
        }

        protected virtual void ResolvePlatformCollision(Player player, Platform platform)
        {
            Rectangle playerRect = player.Bounds;
            Rectangle platRect = platform.Bounds;

            // Определяем направление столкновения / Determine collision direction
            float overlapX = Math.Min(playerRect.Right - platRect.Left, platRect.Right - playerRect.Left);
            float overlapY = Math.Min(playerRect.Bottom - platRect.Top, platRect.Bottom - playerRect.Top);

            if (overlapX < overlapY)
            {
                // Столкновение по горизонтали / Horizontal collision
                if (playerRect.Center.X < platRect.Center.X)
                    player.Position.X = platRect.Left - player.Width / 2;
                else
                    player.Position.X = platRect.Right + player.Width / 2;
                player.Velocity.X = 0;
            }
            else
            {
                // Столкновение по вертикали / Vertical collision
                if (playerRect.Center.Y < platRect.Center.Y && player.Velocity.Y >= 0)
                {
                    // Приземление на платформу / Landing on platform
                    player.Position.Y = platRect.Top - player.Height / 2;
                    player.Velocity.Y = 0;
                    player.SetGrounded(true);
                }
                else if (platform.IsOneWay)
                {
                    // Односторонняя платформа - только сверху / One-way - only from top
                    if (player.Velocity.Y > 0 && playerRect.Bottom - platRect.Top < 20)
                    {
                        player.Position.Y = platRect.Top - player.Height / 2;
                        player.Velocity.Y = 0;
                        player.SetGrounded(true);
                    }
                }
                else
                {
                    // Удар головой / Head hit
                    player.Position.Y = platRect.Bottom + player.Height / 2;
                    player.Velocity.Y = 0;
                }
            }
        }

        protected virtual void HandleEnemyCollision(Enemy enemy)
        {
            // Урон игроку / Damage player
            // TODO: Реализовать систему здоровья / TODO: Implement health system
        }

        protected virtual void CheckLevelComplete()
        {
            // Проверка условия завершения / Check completion condition
        }

        protected void AddPlatform(Platform platform)
        {
            _platforms.Add(platform);
            _physicsSystem.AddCollider(new PhysicsSystem.BoxCollider(
                platform.Position, 
                new Vector2(platform.Width, platform.Height)));
        }

        protected void AddKnowledge(KnowledgeItem item)
        {
            _knowledgeItems.Add(item);
            item.OnCollect += OnKnowledgeCollected;
        }

        protected void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
        }

        protected virtual void OnKnowledgeCollected(KnowledgeItem item)
        {
            AudioManager?.PlaySound("knowledge_collect");
            // Показать всплывающее описание / Show popup description
        }

        public override void Unload()
        {
            base.Unload();
            _platforms.Clear();
            _knowledgeItems.Clear();
            _enemies.Clear();
            _triggers.Clear();
            _doors.Clear();
        }

        #region Properties | Свойства

        public Player Player => _player;
        public LevelStats Stats => _stats;
        public int TotalKnowledge => _knowledgeItems.Count;
        public int CollectedKnowledge => _stats?.KnowledgeCollected ?? 0;
        public float CompletionPercent => TotalKnowledge > 0 
            ? (float)CollectedKnowledge / TotalKnowledge * 100f : 0f;

        #endregion
    }
}
