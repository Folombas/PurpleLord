// ============================================================================
// Level1_FrontendForest.cs - Уровень 1: Лес Фронтенда
// Level 1: Frontend Forest - Bright, colorful CSS/HTML themed level
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
    public class Level1_FrontendForest : Level
    {
        private Texture2D _divBlockTexture;
        private Texture2D _bugEnemyTexture;
        private ParallaxLayer[] _parallaxLayers;
        private float _parallaxTimer = 0f;

        public override string SceneId => "level1_frontend";
        public override string SceneName => "Frontend Forest";
        public override Color BackgroundColor => new Color(135, 206, 235); // Sky blue

        public override void Initialize(Game game, ContentManager contentManager,
            Managers.InputManager inputManager, Managers.AudioManager audioManager,
            Managers.EffectManager effectManager, UI.UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            _stats = new LevelStats { LevelName = "Frontend Forest" };
        }

        public override void LoadContent()
        {
            // Загрузка текстур / Load textures
            _divBlockTexture = ContentManager.GetTexture("Sprites/level1/div_block") ??
                GraphicsUtils.CreateColorTexture(new Color(100, 149, 237), 64, 64);
            
            _bugEnemyTexture = ContentManager.GetTexture("Sprites/enemies/bug") ??
                GraphicsUtils.CreateColorTexture(new Color(100, 200, 50), 32, 32);

            // Загрузка музыки уровня / Load level music
            // AudioManager?.LoadMusic("level1_theme", "Music/level1_frontend");

            base.LoadContent();
        }

        protected override void CreateLevel()
        {
            _playerSpawnPoint = new Vector2(100, 500);
            _levelBounds = new Rectangle(0, 0, 3000, 1080);

            CreatePlatforms();
            CreateKnowledgeItems();
            CreateEnemies();
            CreateDoors();
            CreateTriggers();
        }

        private void CreatePlatforms()
        {
            // Земля / Ground
            var ground = new Platform(new Vector2(1500, 1050), 3000, 60, PlatformType.Solid);
            ground.TintColor = new Color(139, 90, 43); // Brown
            AddPlatform(ground);

            // Платформы-блоки div / Div block platforms
            // Стартовая платформа / Starting platform
            AddPlatform(new Platform(new Vector2(100, 550), 200, 40, PlatformType.Solid)
            { TintColor = new Color(255, 99, 71) }); // Tomato

            // Серия платформ / Platform series
            AddPlatform(new Platform(new Vector2(400, 500), 150, 40, PlatformType.Solid)
            { TintColor = new Color(60, 179, 113) }); // MediumSeaGreen

            AddPlatform(new Platform(new Vector2(650, 450), 150, 40, PlatformType.Solid)
            { TintColor = new Color(255, 165, 0) }); // Orange

            AddPlatform(new Platform(new Vector2(900, 400), 150, 40, PlatformType.Solid)
            { TintColor = new Color(147, 112, 219) }); // MediumPurple

            // Платформа с CSS-селектором / CSS selector platform
            AddPlatform(new Platform(new Vector2(1200, 350), 200, 40, PlatformType.Solid)
            { TintColor = new Color(255, 20, 147) }); // DeepPink

            // Лестница вверх / Staircase up
            for (int i = 0; i < 5; i++)
            {
                AddPlatform(new Platform(
                    new Vector2(1500 + i * 150, 300 - i * 50), 
                    120, 30, PlatformType.Solid)
                { TintColor = new Color(70, 130, 180) }); // SteelBlue
            }

            // Верхняя платформа / Top platform
            AddPlatform(new Platform(new Vector2(2400, 100), 400, 40, PlatformType.Solid)
            { TintColor = new Color(255, 215, 0) }); // Gold

            // Односторонние платформы / One-way platforms
            AddPlatform(new Platform(new Vector2(800, 700), 200, 30, PlatformType.OneWay)
            { TintColor = new Color(255, 182, 193, 180) }); // LightPink

            AddPlatform(new Platform(new Vector2(1100, 650), 200, 30, PlatformType.OneWay)
            { TintColor = new Color(173, 216, 230, 180) }); // LightBlue
        }

        private void CreateKnowledgeItems()
        {
            // HTML - первый предмет / HTML - first item
            AddKnowledge(new KnowledgeItem(new Vector2(400, 450), KnowledgeType.Frontend,
                "HTML", "Язык гипертекстовой разметки. Основа всех веб-страниц.")
            { Width = 32, Height = 32 });

            // CSS
            AddKnowledge(new KnowledgeItem(new Vector2(650, 400), KnowledgeType.Frontend,
                "CSS", "Каскадные таблицы стилей. Отвечают за внешний вид.")
            { Width = 32, Height = 32 });

            // JavaScript
            AddKnowledge(new KnowledgeItem(new Vector2(900, 350), KnowledgeType.Frontend,
                "JavaScript", "Язык программирования для интерактивности.")
            { Width = 32, Height = 32 });

            // React
            AddKnowledge(new KnowledgeItem(new Vector2(1200, 300), KnowledgeType.Frontend,
                "React", "Библиотека от Facebook для создания UI. Компонентный подход.")
            { Width = 32, Height = 32 });

            // Vue
            AddKnowledge(new KnowledgeItem(new Vector2(1700, 200), KnowledgeType.Frontend,
                "Vue.js", "Прогрессивный фреймворк. Простой и гибкий.")
            { Width = 32, Height = 32 });

            // Angular
            AddKnowledge(new KnowledgeItem(new Vector2(2000, 150), KnowledgeType.Frontend,
                "Angular", "Фреймворк от Google. Полноценная платформа для SPA.")
            { Width = 32, Height = 32 });

            // TypeScript
            AddKnowledge(new KnowledgeItem(new Vector2(2400, 50), KnowledgeType.Frontend,
                "TypeScript", "JavaScript с типами. Разработан Microsoft.")
            { Width = 32, Height = 32 });

            // SASS
            AddKnowledge(new KnowledgeItem(new Vector2(500, 680), KnowledgeType.Frontend,
                "SASS", "CSS-препроцессор. Переменные, вложенность, миксины.")
            { Width = 32, Height = 32 });

            _stats.KnowledgeTotal = _knowledgeItems.Count;
        }

        private void CreateEnemies()
        {
            // Баги-враги / Bug enemies
            var bug1 = new BugEnemy(new Vector2(500, 1020), EnemyBehavior.Patrol);
            bug1.SetPatrolParams(new Vector2(100, 0), 1f);
            AddEnemy(bug1);

            var bug2 = new BugEnemy(new Vector2(1000, 1020), EnemyBehavior.Patrol);
            bug2.SetPatrolParams(new Vector2(150, 0), 1.2f);
            AddEnemy(bug2);

            var bug3 = new BugEnemy(new Vector2(1500, 1020), EnemyBehavior.Patrol);
            bug3.SetPatrolParams(new Vector2(200, 0), 0.8f);
            AddEnemy(bug3);

            // Летающие баги / Flying bugs
            var flyingBug = new BugEnemy(new Vector2(800, 600), EnemyBehavior.Flying);
            flyingBug.SetPatrolParams(new Vector2(100, 50), 1.5f);
            AddEnemy(flyingBug);
        }

        private void CreateDoors()
        {
            // Выход на уровень 2 / Exit to level 2
            var exitDoor = new Door(new Vector2(2700, 950), DoorType.Exit, "level2_backend");
            exitDoor.Width = 64;
            exitDoor.Height = 96;
            _doors.Add(exitDoor);
            exitDoor.OnEnter += OnDoorEnter;
        }

        private void CreateTriggers()
        {
            // CSS-селектор триггеры / CSS selector triggers
            var trigger1 = new GameObject(new Vector2(300, 1000));
            trigger1.Width = 50;
            trigger1.Height = 50;
            trigger1.Tag = "Trigger.CSS";
            _triggers.Add(trigger1);
        }

        protected override void LoadLevelMusic()
        {
            // Chillout electronic for Frontend Forest
            // AudioManager?.LoadMusic("level1_theme", "Music/level1_frontend", true);
            // AudioManager?.PlayMusic("level1_theme");
        }

        protected override void DrawLevelBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _parallaxTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Параллакс слои / Parallax layers
            if (_parallaxLayers == null)
            {
                InitializeParallaxLayers();
            }

            foreach (var layer in _parallaxLayers)
            {
                float offsetX = _parallaxTimer * layer.Speed;
                for (int x = -1; x < 3; x++)
                {
                    Rectangle rect = new Rectangle(
                        (int)(x * GraphicsDevice.Viewport.Width + offsetX),
                        0,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height);
                    spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, layer.Color);
                }
            }
        }

        private void InitializeParallaxLayers()
        {
            _parallaxLayers = new ParallaxLayer[]
            {
                new ParallaxLayer { Color = new Color(100, 149, 237, 255), Speed = 10f },   // Sky
                new ParallaxLayer { Color = new Color(34, 139, 34, 100), Speed = 30f },    // Trees back
                new ParallaxLayer { Color = new Color(60, 179, 113, 150), Speed = 50f },   // Trees mid
                new ParallaxLayer { Color = new Color(143, 188, 143, 200), Speed = 80f }   // Trees front
            };
        }

        protected override void DrawLevelForeground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Передний план - листва, элементы UI уровня
            // Foreground - foliage, level UI elements
        }

        protected override void CheckLevelComplete()
        {
            // Уровень завершается когда игрок доходит до двери
            // Level completes when player reaches the door
        }

        private void OnDoorEnter(Entities.Objects.Door door, string targetLevelId)
        {
            // Переход на следующий уровень / Go to next level
            System.Diagnostics.Debug.WriteLine($"Entering door to: {targetLevelId}");
            // SceneManager.LoadScene<Level2_BackendBadlands>();
        }

        private class ParallaxLayer
        {
            public Color Color { get; set; }
            public float Speed { get; set; }
        }
    }

    // Специфичный враг для уровня 1 - Баг
    public class BugEnemy : Enemy
    {
        public BugEnemy(Vector2 position, EnemyBehavior behavior)
            : base(position, EnemyType.Bug, behavior)
        {
            Width = 40;
            Height = 30;
        }

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Рисуем жука с глазами-иконками браузеров
            // Draw bug with browser icon eyes
            
            // Тело жука / Bug body
            Rectangle body = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, new Color(100, 200, 50));

            // Глаза-браузеры / Browser eyes
            float eyeSize = 8f;
            Vector2 leftEye = new Vector2(Position.X - 10, Position.Y - 5);
            Vector2 rightEye = new Vector2(Position.X + 10, Position.Y - 5);

            // Chrome eye (left)
            Rectangle leftEyeRect = new Rectangle(
                (int)(leftEye.X - eyeSize / 2),
                (int)(leftEye.Y - eyeSize / 2),
                (int)eyeSize,
                (int)eyeSize);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, leftEyeRect, new Color(255, 200, 50));

            // Firefox eye (right)
            Rectangle rightEyeRect = new Rectangle(
                (int)(rightEye.X - eyeSize / 2),
                (int)(rightEye.Y - eyeSize / 2),
                (int)eyeSize,
                (int)eyeSize);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, rightEyeRect, new Color(255, 100, 50));

            // Лапки / Legs
            for (int i = -1; i <= 1; i++)
            {
                spriteBatch.Draw(
                    GraphicsUtils.WhiteTexture,
                    new Vector2(Position.X + i * 10, Position.Y + Height / 2),
                    null,
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    new Vector2(8, 2),
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}
