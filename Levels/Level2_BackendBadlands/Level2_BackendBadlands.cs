// ============================================================================
// Level2_BackendBadlands.cs - Уровень 2: Пустоши Бэкенда
// Level 2: Backend Badlands - Dark, industrial server-themed level
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Levels;
using PurpleLordPlatformer.Entities.Objects;
using PurpleLordPlatformer.Entities.Enemies;
using PurpleLordPlatformer.Core;

namespace PurpleLordPlatformer.Levels
{
    public class Level2_BackendBadlands : Level
    {
        private Texture2D _serverRackTexture;
        private float _logTimer = 0f;
        private string[] _logMessages = {
            "GET /api/users 200",
            "POST /api/data 201",
            "ERROR: Connection timeout",
            "DEBUG: Processing request...",
            "INFO: Server started on port 8080",
            "WARN: High memory usage detected",
            "PUT /api/update 200",
            "DELETE /api/cache 204"
        };

        public override string SceneId => "level2_backend";
        public override string SceneName => "Backend Badlands";
        public override Color BackgroundColor => new Color(20, 20, 30); // Dark gray

        public override void Initialize(Game game, ContentManager contentManager,
            Managers.InputManager inputManager, Managers.AudioManager audioManager,
            Managers.EffectManager effectManager, UI.UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            _stats = new LevelStats { LevelName = "Backend Badlands" };
        }

        public override void LoadContent()
        {
            _serverRackTexture = ContentManager.GetTexture("Sprites/level2/server_rack") ??
                GraphicsUtils.CreateColorTexture(new Color(50, 50, 60), 64, 128);

            base.LoadContent();
        }

        protected override void CreateLevel()
        {
            _playerSpawnPoint = new Vector2(100, 800);
            _levelBounds = new Rectangle(0, 0, 4000, 1080);

            CreatePlatforms();
            CreateKnowledgeItems();
            CreateEnemies();
            CreateDoors();
            CreateTriggers();
        }

        private void CreatePlatforms()
        {
            // Земля - металлическая платформа / Ground - metal platform
            var ground = new Platform(new Vector2(2000, 1050), 4000, 60, PlatformType.Solid);
            ground.TintColor = new Color(40, 40, 50);
            AddPlatform(ground);

            // Серверные стойки-платформы / Server rack platforms
            AddPlatform(new Platform(new Vector2(300, 700), 200, 40, PlatformType.Solid)
            { TintColor = new Color(60, 60, 80) });

            AddPlatform(new Platform(new Vector2(600, 600), 150, 40, PlatformType.Solid)
            { TintColor = new Color(70, 70, 90) });

            // Трубы API - узкие платформы / API pipes - narrow platforms
            AddPlatform(new Platform(new Vector2(900, 550), 100, 20, PlatformType.Solid)
            { TintColor = new Color(100, 80, 60) }); // Rust color

            AddPlatform(new Platform(new Vector2(1100, 500), 100, 20, PlatformType.Solid)
            { TintColor = new Color(120, 100, 80) });

            // Лабиринт труб / Pipe maze
            CreatePipeMaze(1400, 400);

            // Платформы-серверы / Server platforms
            for (int i = 0; i < 5; i++)
            {
                AddPlatform(new Platform(
                    new Vector2(2000 + i * 200, 350 - (i % 2) * 80),
                    150, 40, PlatformType.Solid)
                { TintColor = new Color(50 + i * 10, 50 + i * 10, 70 + i * 10) });
            }

            // Ключи-запросы / API key platforms
            AddPlatform(new Platform(new Vector2(1500, 700), 100, 30, PlatformType.Solid)
            { TintColor = new Color(0, 150, 0) }); // Green for key 1

            AddPlatform(new Platform(new Vector2(2200, 650), 100, 30, PlatformType.Solid)
            { TintColor = new Color(0, 100, 150) }); // Blue for key 2

            AddPlatform(new Platform(new Vector2(2800, 600), 100, 30, PlatformType.Solid)
            { TintColor = new Color(150, 100, 0) }); // Gold for key 3

            // Финишная платформа / Final platform
            AddPlatform(new Platform(new Vector2(3500, 200), 300, 40, PlatformType.Solid)
            { TintColor = new Color(100, 50, 150) });
        }

        private void CreatePipeMaze(float startX, float startY)
        {
            // Горизонтальные трубы / Horizontal pipes
            for (int i = 0; i < 4; i++)
            {
                AddPlatform(new Platform(
                    new Vector2(startX + i * 150, startY + (i % 2) * 60),
                    120, 15, PlatformType.Solid)
                { TintColor = new Color(80 + i * 10, 80 + i * 10, 100) });
            }

            // Вертикальные соединения / Vertical connections
            AddPlatform(new Platform(new Vector2(startX + 75, startY + 30), 20, 80, PlatformType.Solid)
            { TintColor = new Color(90, 90, 110) });

            AddPlatform(new Platform(new Vector2(startX + 225, startY + 30), 20, 80, PlatformType.Solid)
            { TintColor = new Color(90, 90, 110) });
        }

        private void CreateKnowledgeItems()
        {
            // Backend технологии / Backend technologies
            
            AddKnowledge(new KnowledgeItem(new Vector2(300, 650), KnowledgeType.Backend,
                "Go", "Язык от Google. Простой, быстрый, для высоконагруженных систем.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(600, 550), KnowledgeType.Backend,
                "Python", "Универсальный язык. Веб, Data Science, автоматизация.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(900, 500), KnowledgeType.Backend,
                "Node.js", "JavaScript на сервере. Асинхронный, событийный.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1200, 450), KnowledgeType.Backend,
                "Java", "Корпоративный стандарт. Надёжный, многопоточный.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1500, 650), KnowledgeType.Database,
                "PostgreSQL", "Продвинутая SQL БД. Надёжность и ACID.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2200, 600), KnowledgeType.Database,
                "Redis", "In-memory БД. Кэш, очереди, pub/sub.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2800, 550), KnowledgeType.Database,
                "MongoDB", "NoSQL документоориентированная БД. Гибкая схема.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2000, 300), KnowledgeType.DevOps,
                "Docker", "Контейнеризация. Изоляция и переносимость.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2400, 280), KnowledgeType.DevOps,
                "Kubernetes", "Оркестрация контейнеров. Масштабирование и отказоустойчивость.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(3500, 150), KnowledgeType.Backend,
                "gRPC", "RPC фреймворк от Google. Быстрый, типизированный.")
            { Width = 32, Height = 32 });

            _stats.KnowledgeTotal = _knowledgeItems.Count;
        }

        private void CreateEnemies()
        {
            // Серверы-враги (стационарные) / Server enemies (stationary)
            var server1 = new ServerEnemy(new Vector2(500, 1020));
            AddEnemy(server1);

            var server2 = new ServerEnemy(new Vector2(1000, 1020));
            AddEnemy(server2);

            // Летающие кружки с кофе из следующего уровня (пасхалка)
            var coffee1 = new CoffeeEnemy(new Vector2(1400, 350), EnemyBehavior.Flying);
            coffee1.SetPatrolParams(new Vector2(50, 30), 2f);
            AddEnemy(coffee1);

            var coffee2 = new CoffeeEnemy(new Vector2(2000, 300), EnemyBehavior.Flying);
            coffee2.SetPatrolParams(new Vector2(80, 40), 1.5f);
            AddEnemy(coffee2);
        }

        private void CreateDoors()
        {
            // Выход на уровень 3 / Exit to level 3
            var exitDoor = new Door(new Vector2(3700, 100), DoorType.Exit, "level3_neural");
            exitDoor.Width = 64;
            exitDoor.Height = 96;
            _doors.Add(exitDoor);
            exitDoor.OnEnter += OnDoorEnter;

            // Заблокированные двери (FOMO) / Locked doors (FOMO)
            var lockedDoor1 = new Door(new Vector2(3600, 100), DoorType.Locked, "bonus_junior");
            lockedDoor1.LockReason = "Требуется собрать все знания";
            _doors.Add(lockedDoor1);
        }

        private void CreateTriggers()
        {
            // Триггеры для ключей-запросов / API key triggers
        }

        protected override void LoadLevelMusic()
        {
            // Industrial techno for Backend Badlands
        }

        protected override void DrawLevelBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _logTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Тёмный фон с логами / Dark background with logs
            Rectangle screen = new Rectangle(0, 0, 
                GraphicsDevice.Viewport.Width, 
                GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, new Color(15, 15, 25));

            // Мигающие логи / Blinking logs
            if (_logTimer > 0.1f)
            {
                _logTimer = 0f;
            }

            // Отрисовка логов на фоне / Draw logs on background
            SpriteFont font = UIManager.DefaultFont;
            if (font != null)
            {
                for (int i = 0; i < 15; i++)
                {
                    string log = _logMessages[i % _logMessages.Length];
                    float alpha = 0.1f + (float)Math.Sin(_logTimer + i) * 0.05f;
                    Vector2 pos = new Vector2(50, 50 + i * 25);
                    spriteBatch.DrawString(font, log, pos, new Color(0, 255, 0, (int)(alpha * 255)));
                }
            }

            // Серверные стойки на фоне / Server racks in background
            for (int x = 0; x < GraphicsDevice.Viewport.Width / 100 + 1; x++)
            {
                Rectangle rack = new Rectangle(x * 100 - (int)(Camera.Position.X * 0.5f % 100), 
                    200, 80, 400);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, rack, new Color(30, 30, 40, 100));

                // Огни серверов / Server lights
                for (int y = 0; y < 8; y++)
                {
                    bool blink = (DateTime.Now.Millisecond + x * 100 + y * 50) % 1000 < 500;
                    Color lightColor = blink ? new Color(0, 255, 0, 200) : new Color(0, 50, 0, 100);
                    Rectangle light = new Rectangle(rack.X + 10, rack.Y + 20 + y * 50, 10, 10);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, light, lightColor);
                }
            }
        }

        private void OnDoorEnter(Entities.Objects.Door door, string targetLevelId)
        {
            System.Diagnostics.Debug.WriteLine($"Entering door to: {targetLevelId}");
        }
    }

    // Сервер-враг / Server enemy
    public class ServerEnemy : Enemy
    {
        private float _sparkTimer = 0f;

        public ServerEnemy(Vector2 position)
            : base(position, EnemyType.Server, EnemyBehavior.Stationary)
        {
            Width = 60;
            Height = 80;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _sparkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Корпус сервера / Server case
            Rectangle body = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, new Color(50, 50, 60));

            // Панель с огнями / Panel with lights
            Rectangle panel = new Rectangle(
                (int)(Position.X - Width / 2 + 5),
                (int)(Position.Y - Height / 2 + 10),
                (int)Width - 10,
                (int)Height - 20);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, panel, new Color(30, 30, 40));

            // Мигающие огни / Blinking lights
            bool blink = _sparkTimer % 1f < 0.5f;
            for (int i = 0; i < 5; i++)
            {
                Color lightColor = (i % 2 == 0 && blink) ? 
                    new Color(0, 255, 0) : new Color(0, 50, 0);
                Rectangle light = new Rectangle(
                    (int)(Position.X - Width / 2 + 10),
                    (int)(Position.Y - Height / 2 + 20 + i * 12),
                    8, 8);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, light, lightColor);
            }

            // Искры / Sparks
            if (blink)
            {
                for (int i = 0; i < 3; i++)
                {
                    Rectangle spark = new Rectangle(
                        (int)(Position.X + (i - 1) * 15),
                        (int)(Position.Y + Height / 2 + 5),
                        4, 4);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, spark, Color.Yellow);
                }
            }
        }
    }

    // Кружка кофе - летающий враг / Coffee mug - flying enemy
    public class CoffeeEnemy : Enemy
    {
        public CoffeeEnemy(Vector2 position, EnemyBehavior behavior)
            : base(position, EnemyType.Coffee, behavior)
        {
            Width = 30;
            Height = 35;
        }

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Кружка / Mug
            Rectangle mug = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, mug, new Color(150, 100, 50));

            // Ручка кружки / Mug handle
            Rectangle handle = new Rectangle(
                (int)(Position.X + Width / 2 - 5),
                (int)(Position.Y - 5),
                10, 15);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, handle, new Color(130, 80, 40));

            // Пар от кофе / Steam
            float steamOffset = (float)Math.Sin(_patrolTimer * 5) * 5;
            for (int i = 0; i < 3; i++)
            {
                Rectangle steam = new Rectangle(
                    (int)(Position.X - 5 + i * 5),
                    (int)(Position.Y - Height / 2 - 10 - i * 8 + steamOffset),
                    6, 6);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, steam, new Color(200, 200, 200, 150));
            }

            // Надпись "COFFEE" / "COFFEE" text
            // (упрощённо - просто полоска)
            Rectangle label = new Rectangle(
                (int)(Position.X - 10),
                (int)(Position.Y),
                20, 8);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, label, Color.White);
        }
    }
}
