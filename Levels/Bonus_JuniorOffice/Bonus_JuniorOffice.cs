// ============================================================================
// Bonus_JuniorOffice.cs - Бонусный уровень: Офис Джун
// Bonus Level: Junior Office - IDE-themed puzzle level
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
    public class Bonus_JuniorOffice : Level
    {
        private float _memeTimer = 0f;
        private int _memesCollected = 0;
        private int _memesTotal = 5;

        public override string SceneId => "bonus_junior";
        public override string SceneName => "Junior Office";
        public override Color BackgroundColor => new Color(240, 240, 230); // Office white

        public override void Initialize(Game game, ContentManager contentManager,
            Managers.InputManager inputManager, Managers.AudioManager audioManager,
            Managers.EffectManager effectManager, UI.UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            _stats = new LevelStats { LevelName = "Junior Office" };
        }

        protected override void CreateLevel()
        {
            _playerSpawnPoint = new Vector2(100, 700);
            _levelBounds = new Rectangle(0, 0, 2500, 1080);

            CreatePlatforms();
            CreateKnowledgeItems();
            CreateEnemies();
            CreateDoors();
            CreateMemeCollectibles();
        }

        private void CreatePlatforms()
        {
            // Пол офиса / Office floor
            var floor = new Platform(new Vector2(1250, 1050), 2500, 60, PlatformType.Solid);
            floor.TintColor = new Color(180, 180, 180);
            AddPlatform(floor);

            // Столы-платформы (IDE иконки) / Desk platforms (IDE icons)
            AddPlatform(new Platform(new Vector2(300, 700), 200, 50, PlatformType.Solid)
            { TintColor = new Color(0, 120, 215) }); // VS Code blue

            AddPlatform(new Platform(new Vector2(600, 600), 180, 50, PlatformType.Solid)
            { TintColor = new Color(255, 100, 100) }); // IntelliJ red

            AddPlatform(new Platform(new Vector2(900, 500), 200, 50, PlatformType.Solid)
            { TintColor = new Color(100, 200, 100) }); // PyCharm green

            AddPlatform(new Platform(new Vector2(1200, 400), 180, 50, PlatformType.Solid)
            { TintColor = new Color(255, 200, 0) }); // Rider yellow

            // Кубики-платформы / Cube platforms
            for (int i = 0; i < 4; i++)
            {
                AddPlatform(new Platform(
                    new Vector2(1500 + i * 150, 350 - (i % 2) * 60),
                    100, 40, PlatformType.Solid)
                { TintColor = new Color(150, 150, 150) });
            }

            // Финальная платформа / Final platform
            AddPlatform(new Platform(new Vector2(2200, 300), 250, 50, PlatformType.Solid)
            { TintColor = new Color(100, 100, 200) });
        }

        private void CreateKnowledgeItems()
        {
            // Junior-технологии / Junior technologies

            AddKnowledge(new KnowledgeItem(new Vector2(300, 650), KnowledgeType.Other,
                "Git Basics", "commit, push, pull. Основы版本控制.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(600, 550), KnowledgeType.Other,
                "Debugging", "Отладка кода. Breakpoints, step through.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(900, 450), KnowledgeType.Other,
                "Unit Tests", "Модульное тестирование. xUnit, NUnit.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1200, 350), KnowledgeType.Other,
                "Code Review", "Ревью кода. Обратная связь, best practices.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1500, 300), KnowledgeType.Other,
                "Agile/Scrum", "Методологии разработки. Спринты, стендапы.")
            { Width = 32, Height = 32 });

            _stats.KnowledgeTotal = _knowledgeItems.Count;
        }

        private void CreateEnemies()
        {
            // Кружки кофе - основные враги / Coffee mugs - main enemies
            for (int i = 0; i < 6; i++)
            {
                var coffee = new CoffeeEnemy(new Vector2(400 + i * 300, 500 + (i % 2) * 150), 
                    EnemyBehavior.Flying);
                coffee.SetPatrolParams(new Vector2(100, 50), 1.5f + i * 0.2f);
                AddEnemy(coffee);
            }

            // Стационарные "дедлайны" / Stationary "deadlines"
            AddEnemy(new DeadlineEnemy(new Vector2(1000, 1020)));
            AddEnemy(new DeadlineEnemy(new Vector2(1800, 1020)));
        }

        private void CreateDoors()
        {
            // Выход назад к меню / Exit back to menu
            var exitDoor = new Door(new Vector2(2350, 200), DoorType.Exit, "main_menu");
            exitDoor.Width = 64;
            exitDoor.Height = 96;
            _doors.Add(exitDoor);
            exitDoor.OnEnter += OnDoorEnter;
        }

        private void CreateMemeCollectibles()
        {
            // Мем-цитаты для сбора / Meme quotes to collect
            // Реализуется как специальные предметы
        }

        protected override void LoadLevelMusic()
        {
            // Lo-fi hip hop for office atmosphere
        }

        protected override void DrawLevelBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Офисный фон / Office background
            Rectangle screen = new Rectangle(0, 0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, new Color(240, 240, 230));

            // Окна офиса / Office windows
            DrawOfficeWindows(spriteBatch);

            // Кубиклы на фоне / Cubicles in background
            DrawCubicles(spriteBatch);

            // Постеры с мемами / Meme posters
            DrawMemePosters(spriteBatch, gameTime);
        }

        private void DrawOfficeWindows(SpriteBatch spriteBatch)
        {
            int windowWidth = 100;
            int windowHeight = 150;
            int spacing = 150;

            for (int x = 0; x < GraphicsDevice.Viewport.Width / spacing + 1; x++)
            {
                int wx = x * spacing - (int)(Camera.Position.X * 0.2f % spacing);
                Rectangle window = new Rectangle(wx, 100, windowWidth, windowHeight);
                
                // Небо за окном / Sky outside
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, window, 
                    new Color(135, 206, 235));

                // Рама / Frame
                GraphicsUtils.DrawRectangle(spriteBatch, window, Color.White, 5);

                // Перекладины / Crossbars
                GraphicsUtils.DrawLine(spriteBatch, 
                    new Vector2(wx + windowWidth / 2, 100),
                    new Vector2(wx + windowWidth / 2, 250), Color.White, 3);
                GraphicsUtils.DrawLine(spriteBatch,
                    new Vector2(wx, 175),
                    new Vector2(wx + windowWidth, 175), Color.White, 3);
            }
        }

        private void DrawCubicles(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < GraphicsDevice.Viewport.Width / 120 + 1; x++)
            {
                int cx = x * 120 - (int)(Camera.Position.X * 0.3f % 120);
                Rectangle cubicle = new Rectangle(cx, 400, 100, 200);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, cubicle, 
                    new Color(200, 200, 190));

                // Монитор на столе / Monitor on desk
                Rectangle monitor = new Rectangle(cx + 20, 450, 60, 40);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, monitor, new Color(50, 50, 50));

                // Экран монитора / Monitor screen
                Rectangle screen = new Rectangle(cx + 25, 455, 50, 30);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, screen, new Color(0, 100, 0));
            }
        }

        private void DrawMemePosters(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _memeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            string[] memes = {
                "It works on my machine",
                "I'll fix it in prod",
                "TODO: Fix this",
                "404: Motivation not found",
                "Coffee => Code"
            };

            SpriteFont font = UIManager.DefaultFont;
            if (font == null) return;

            for (int i = 0; i < memes.Length; i++)
            {
                float x = 200 + i * 400 - Camera.Position.X * 0.5f;
                if (x < -100 || x > GraphicsDevice.Viewport.Width + 100) continue;

                // Рамка постера / Poster frame
                Rectangle poster = new Rectangle((int)x, 150, 150, 100);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, poster, Color.White);
                GraphicsUtils.DrawRectangle(spriteBatch, poster, Color.Black, 3);

                // Текст мема / Meme text
                Vector2 textSize = font.MeasureString(memes[i]);
                float scale = Math.Min(0.8f, 140 / textSize.X);
                spriteBatch.DrawString(font, memes[i], 
                    new Vector2(x + 75 - textSize.X * scale / 2, 180),
                    Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        protected override void DrawLevelForeground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Передний план - растения, файлы
            // Foreground - plants, files
        }

        private void OnDoorEnter(Entities.Objects.Door door, string targetLevelId)
        {
            System.Diagnostics.Debug.WriteLine($"Entering door to: {targetLevelId}");
            
            if (targetLevelId == "main_menu")
            {
                SceneManager.LoadScene<UI.Menus.MainMenuScene>();
            }
        }
    }

    // Враг "Дедлайн" / Deadline enemy
    public class DeadlineEnemy : Enemy
    {
        private float _pulseTimer = 0f;

        public DeadlineEnemy(Vector2 position)
            : base(position, EnemyType.Server, EnemyBehavior.Stationary)
        {
            Width = 80;
            Height = 60;
            Tag = "Deadline";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _pulseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
        }

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Красный мигающий блок / Red blinking block
            float pulse = (float)Math.Sin(_pulseTimer) * 0.3f + 0.7f;
            Color color = new Color(255, (int)(100 * pulse), (int)(100 * pulse));

            Rectangle body = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, color);

            // Текст "DEADLINE" / "DEADLINE" text
            SpriteFont font = UIManager.DefaultFont;
            if (font != null)
            {
                Vector2 textSize = font.MeasureString("DEADLINE");
                spriteBatch.DrawString(font, "DEADLINE",
                    new Vector2(Position.X - textSize.X / 2, Position.Y - textSize.Y / 2),
                    Color.White);
            }

            // Восклицательные знаки / Exclamation marks
            for (int i = 0; i < 3; i++)
            {
                float offsetX = (i - 1) * 25;
                Rectangle exclaim = new Rectangle(
                    (int)(Position.X + offsetX) - 3,
                    (int)(Position.Y + Height / 2 + 10),
                    6, 20);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, exclaim, Color.Yellow);
            }
        }
    }
}
