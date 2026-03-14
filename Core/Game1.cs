using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLord.Entities;
using PurpleLord.Core;

namespace PurpleLord
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Player _player;
        private Camera2D _camera;
        private List<Platform> _platforms;

        // Декорации
        private List<Cloud> _clouds;
        private List<Bird> _birds;
        private List<PalmTree> _palmTrees;
        private List<House> _houses;
        private List<SmokeParticle> _smokeParticles;
        private List<Flower> _flowers;

        // Новые элементы
        private List<Wave> _waves;
        private List<Volcano> _volcanoes;
        private List<Collectible> _collectibles;
        private List<Porcupine> _porcupines;
        private List<Dolphin> _dolphins;
        private Ship _ship;
        private List<XPFloatText> _xpTexts;
        private List<BloodParticle> _bloodParticles;

        // Солнце
        private Vector2 _sunPosition = new Vector2(100, 80);
        private float _sunRayTimer;

        // Размеры мира
        private const int WORLD_WIDTH = 3000;
        private const int GROUND_Y = 580;

        // Цвета
        private Color _skyColor = new Color(135, 206, 235);
        private Color _seaColor = new Color(30, 144, 255);
        private Color _mountainColor = new Color(100, 100, 150);
        private Color _grassColor = new Color(34, 139, 34);
        private Color _dirtColor = new Color(139, 90, 43);
        private Color _sunColor = new Color(255, 220, 0);
        
        // Стартовое меню
        private bool _showStartMenu = true;
        private float _menuBlinkTimer;
        private bool _menuTextVisible = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Полноэкранный режим
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            Window.Title = "Purple Lord - Path of Choices";
        }

        protected override void Initialize()
        {
            // === КАМЕРА ===
            _camera = new Camera2D(GraphicsDevice.Viewport);

            // === ПЛАТФОРМЫ И ЗЕМЛЯ ===
            _platforms = new List<Platform>();
            
            // Основная земля (вся длина мира)
            _platforms.Add(new Platform(0, GROUND_Y, WORLD_WIDTH, 200, true));
            
            // Платформы в воздухе (разбросаны по миру)
            _platforms.Add(new Platform(250, 480, 120, 25, false));
            _platforms.Add(new Platform(450, 400, 140, 25, false));
            _platforms.Add(new Platform(680, 320, 100, 25, false));
            _platforms.Add(new Platform(850, 420, 130, 25, false));
            _platforms.Add(new Platform(1100, 350, 150, 25, false));
            _platforms.Add(new Platform(1350, 280, 120, 25, false));
            _platforms.Add(new Platform(1550, 380, 140, 25, false));
            _platforms.Add(new Platform(1800, 300, 130, 25, false));
            _platforms.Add(new Platform(2000, 420, 160, 25, false));
            _platforms.Add(new Platform(2250, 350, 120, 25, false));
            _platforms.Add(new Platform(2450, 280, 140, 25, false));
            _platforms.Add(new Platform(2700, 400, 150, 25, false));

            // === ИГРОК (на земле, слева) ===
            _player = new Player(new Vector2(150, GROUND_Y - 60));

            // === ПАЛЬМЫ (расставлены по миру) ===
            _palmTrees = new List<PalmTree>
            {
                new PalmTree(new Vector2(100, GROUND_Y - 90)),
                new PalmTree(new Vector2(600, GROUND_Y - 90)),
                new PalmTree(new Vector2(1400, GROUND_Y - 90)),
                new PalmTree(new Vector2(2100, GROUND_Y - 90)),
                new PalmTree(new Vector2(2800, GROUND_Y - 90)),
            };

            // === ДОМИКИ (деревня справа) ===
            _houses = new List<House>
            {
                new House(new Vector2(950, GROUND_Y - 70), new Color(220, 200, 170)),
                new House(new Vector2(1080, GROUND_Y - 70), new Color(200, 180, 150)),
                new House(new Vector2(1200, GROUND_Y - 70), new Color(210, 190, 160)),
            };

            // === ЦВЕТЫ (украшают землю) ===
            _flowers = new List<Flower>();
            var random = new Random();
            for (int i = 0; i < 40; i++)
            {
                int x = random.Next(50, WORLD_WIDTH - 50);
                // Не ставим цветы на домах
                bool onHouse = false;
                foreach (var house in _houses)
                {
                    if (x > house.Position.X - 20 && x < house.Position.X + house.Width + 20)
                        onHouse = true;
                }
                if (!onHouse)
                {
                    _flowers.Add(new Flower(new Vector2(x, GROUND_Y - 12), 
                        random.Next(0, 3) == 0 ? Color.Red : 
                        random.Next(0, 2) == 0 ? Color.Yellow : Color.Pink));
                }
            }

            // === ОБЛАКА ===
            _clouds = new List<Cloud>();
            for (int i = 0; i < 8; i++)
            {
                _clouds.Add(new Cloud(
                    new Vector2(random.Next(0, WORLD_WIDTH), random.Next(30, 200)),
                    20f + random.NextSingle() * 20f
                ));
            }

            // === ПТИЦЫ ===
            _birds = new List<Bird>();
            for (int i = 0; i < 6; i++)
            {
                _birds.Add(new Bird(
                    new Vector2(random.Next(200, WORLD_WIDTH - 200), random.Next(80, 250)),
                    50f + random.NextSingle() * 50f
                ));
            }

            // === ДЫМ ===
            _smokeParticles = new List<SmokeParticle>();
            
            // === ВОЛНЫ (на море) ===
            _waves = new List<Wave>();
            for (int i = 0; i < 20; i++)
            {
                _waves.Add(new Wave(2600 + i * 35, GROUND_Y - 20 + (i % 3) * 10));
            }
            
            // === ВУЛКАНЫ ===
            _volcanoes = new List<Volcano>
            {
                new Volcano(new Vector2(2500, GROUND_Y - 120)),
                new Volcano(new Vector2(2750, GROUND_Y - 150))
            };
            
            // === ДИКОБРАЗЫ (ползают по земле) ===
            _porcupines = new List<Porcupine>();
            for (int i = 0; i < 8; i++)
            {
                int x = random.Next(400, WORLD_WIDTH - 400);
                // Не ставим дикобразов на старте
                if (x > 300 && x < 500) x += 500;
                _porcupines.Add(new Porcupine(new Vector2(x, GROUND_Y - 20)));
            }
            
            // === ПРЕДМЕТЫ ДЛЯ СБОРА (кокосы и бананы на пальмах) ===
            _collectibles = new List<Collectible>();
            foreach (var palm in _palmTrees)
            {
                // Добавляем кокосы
                _collectibles.Add(new Collectible(
                    new Vector2(palm.Position.X + 8, palm.Position.Y - 30),
                    CollectibleType.Coconut
                ));
                // Добавляем бананы
                _collectibles.Add(new Collectible(
                    new Vector2(palm.Position.X - 25, palm.Position.Y - 45),
                    CollectibleType.Banana
                ));
                _collectibles.Add(new Collectible(
                    new Vector2(palm.Position.X + 35, palm.Position.Y - 50),
                    CollectibleType.Banana
                ));
            }
            
            // === ДЕЛЬФИНЫ (прыгают в море) ===
            _dolphins = new List<Dolphin>();
            for (int i = 0; i < 5; i++)
            {
                _dolphins.Add(new Dolphin(2650 + i * 80, GROUND_Y - 30));
            }
            
            // === КОРАБЛЬ (вдалеке на море) ===
            _ship = new Ship(new Vector2(2800, GROUND_Y - 60));
            
            // === ТЕКСТЫ +XP ===
            _xpTexts = new List<XPFloatText>();
            
            // === ЧАСТИЦЫ КРОВИ ===
            _bloodParticles = new List<BloodParticle>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Звуки не загружаем (нужны внешние файлы)
            // Звуковые эффекты будут визуальными
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            // === СТАРТОВОЕ МЕНЮ ===
            if (_showStartMenu)
            {
                _menuBlinkTimer += delta;
                if (_menuBlinkTimer > 0.5f)
                {
                    _menuBlinkTimer = 0f;
                    _menuTextVisible = !_menuTextVisible;
                }
                
                // Нажатие любой клавиши для старта
                if (keyboardState.IsKeyDown(Keys.Enter) || keyboardState.IsKeyDown(Keys.Space) || 
                    keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.D) ||
                    keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.S))
                {
                    _showStartMenu = false;
                }
                
                if (keyboardState.IsKeyDown(Keys.Escape))
                    Exit();
                    
                base.Update(gameTime);
                return;
            }

            // Выход по Escape
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Обновление игрока
            _player.Update(gameTime, _platforms);

            // КАМЕРА СЛЕДУЕТ ЗА ИГРОКОМ (плавно)
            float targetX = _player.Position.X - GraphicsDevice.Viewport.Width / 2;
            targetX = Math.Max(0, Math.Min(targetX, WORLD_WIDTH - GraphicsDevice.Viewport.Width));
            _camera.Position = new Vector2(targetX, 0);

            // Обновление декораций
            foreach (var cloud in _clouds)
                cloud.Update(delta, WORLD_WIDTH);

            foreach (var bird in _birds)
                bird.Update(delta);
            
            // Обновление дельфинов
            foreach (var dolphin in _dolphins)
                dolphin.Update(delta);
            
            // Обновление корабля
            _ship.Update(delta);

            // Обновление волн
            foreach (var wave in _waves)
                wave.Update(delta);

            // Обновление вулканов
            foreach (var volcano in _volcanoes)
                volcano.Update(delta);
            
            // Обновление солнца
            UpdateSun(delta);

            // Обновление дикобразов
            foreach (var porcupine in _porcupines)
                porcupine.Update(delta, _platforms);
            
            // Обновление XP текстов
            foreach (var xpText in _xpTexts)
                xpText.Update(delta);
            _xpTexts.RemoveAll(x => x.Life <= 0);
            
            // Обновление частиц крови
            foreach (var blood in _bloodParticles)
                blood.Update(delta);
            _bloodParticles.RemoveAll(b => b.Life <= 0);

            // Проверка сбора предметов
            CheckCollectibles();

            // Проверка столкновений с дикобразами
            CheckPorcupineCollisions();

            // Дым из труб
            UpdateSmoke(delta);

            base.Update(gameTime);
        }
        
        private void UpdateSun(float delta)
        {
            _sunRayTimer += delta;
        }
        
        private void CheckCollectibles()
        {
            for (int i = _collectibles.Count - 1; i >= 0; i--)
            {
                var collectible = _collectibles[i];
                if (!collectible.IsCollected)
                {
                    // Простая проверка расстояния до игрока
                    float distance = Vector2.Distance(_player.Position, collectible.Position);
                    if (distance < 50f)
                    {
                        collectible.IsCollected = true;
                        int xpGain = collectible.Type == CollectibleType.Coconut ? 25 : 15;
                        _player.CollectItem(xpGain);
                        
                        // Визуальный эффект +XP
                        _xpTexts.Add(new XPFloatText(
                            _player.Position,
                            $"+{xpGain} XP",
                            collectible.Type == CollectibleType.Coconut ? new Color(210, 140, 80) : Color.Yellow
                        ));
                    }
                }
            }
        }

        private void CheckPorcupineCollisions()
        {
            foreach (var porcupine in _porcupines)
            {
                float distance = Vector2.Distance(_player.Position, porcupine.Position);
                if (distance < 50f && !porcupine.IsDead)
                {
                    int damage = 20;
                    _player.TakeDamage(porcupine.Position, damage);

                    // Эффект крови
                    for (int i = 0; i < 15; i++)
                    {
                        _bloodParticles.Add(new BloodParticle(
                            _player.Position,
                            new Vector2(
                                (float)(_random.NextDouble() - 0.5) * 300f,
                                (float)(_random.NextDouble() - 0.5) * 300f - 100f
                            )
                        ));
                    }
                }
            }
        }

        private Random _random = new Random();
        
        private void UpdateSmoke(float delta)
        {
            foreach (var house in _houses)
            {
                if (_random.NextSingle() < 0.15f)
                {
                    _smokeParticles.Add(new SmokeParticle(
                        new Vector2(house.Position.X + house.Width / 2 + _random.Next(-3, 3), 
                                   house.Position.Y - 25)
                    ));
                }
            }

            for (int i = _smokeParticles.Count - 1; i >= 0; i--)
            {
                _smokeParticles[i].Update(delta);
                if (_smokeParticles[i].Life <= 0)
                    _smokeParticles.RemoveAt(i);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_skyColor);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            // === ЗАДНИЙ ПЛАН (дальний) ===

            // Солнце
            DrawSun(_sunPosition);

            // Море на горизонте (справа)
            _spriteBatch.Draw(new Rectangle(2600, GROUND_Y - 40, 600, 60), _seaColor);
            
            // === КОРАБЛЬ ===
            _ship.Draw(_spriteBatch);
            
            // === ДЕЛЬФИНЫ ===
            foreach (var dolphin in _dolphins)
                dolphin.Draw(_spriteBatch);

            // === ВОЛНЫ ===
            foreach (var wave in _waves)
                wave.Draw(_spriteBatch);

            // Горы (на заднем плане)
            DrawMountains(2400, GROUND_Y - 40);
            DrawMountains(2700, GROUND_Y - 40);

            // === ВУЛКАНЫ ===
            foreach (var volcano in _volcanoes)
                volcano.Draw(_spriteBatch);

            // === ОБЛАКА ===
            foreach (var cloud in _clouds)
                cloud.Draw(_spriteBatch);

            // === ПТИЦЫ ===
            foreach (var bird in _birds)
                bird.Draw(_spriteBatch);

            // === ЦВЕТЫ ===
            foreach (var flower in _flowers)
                flower.Draw(_spriteBatch);

            // === ДОМИКИ ===
            foreach (var house in _houses)
                house.Draw(_spriteBatch);

            // === ДЫМ ===
            foreach (var smoke in _smokeParticles)
                smoke.Draw(_spriteBatch);

            // === ПАЛЬМЫ ===
            foreach (var palm in _palmTrees)
                palm.Draw(_spriteBatch);

            // === ПРЕДМЕТЫ ДЛЯ СБОРА ===
            foreach (var collectible in _collectibles)
                collectible.Draw(_spriteBatch);

            // === ПЛАТФОРМЫ ===
            foreach (var platform in _platforms)
                platform.Draw(_spriteBatch);

            // === ДИКОБРАЗЫ ===
            foreach (var porcupine in _porcupines)
                porcupine.Draw(_spriteBatch);
            
            // === ЧАСТИЦЫ КРОВИ ===
            foreach (var blood in _bloodParticles)
                blood.Draw(_spriteBatch);

            // === ИГРОК ===
            _player.Draw(_spriteBatch, gameTime);
            
            // === XP ТЕКСТЫ ===
            foreach (var xpText in _xpTexts)
                xpText.Draw(_spriteBatch);

            _spriteBatch.End();

            // === UI (без камеры) ===
            DrawUI();
            
            // === СТАРТОВОЕ МЕНЮ (поверх всего) ===
            if (_showStartMenu)
                DrawStartMenu();

            base.Draw(gameTime);
        }

        private void DrawSun(Vector2 position)
        {
            int radius = 40;

            // === ЭФФЕКТНЫЕ ЛУЧИ (вращаются) ===
            float rayRotation = _sunRayTimer * 0.5f;
            int numRays = 16;
            
            for (int i = 0; i < numRays; i++)
            {
                float angle = rayRotation + i * MathHelper.TwoPi / numRays;
                float rayLength = radius + 20 + (float)Math.Sin(_sunRayTimer * 3 + i) * 10;
                
                float rayX = position.X + (float)Math.Cos(angle) * rayLength;
                float rayY = position.Y + (float)Math.Sin(angle) * rayLength;
                
                // Рисуем луч от центра к краю
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                for (float d = radius; d < rayLength; d += 5)
                {
                    float alpha = 1 - (d - radius) / (rayLength - radius);
                    int size = (int)(8 * alpha);
                    _spriteBatch.Draw(
                        new Rectangle(
                            (int)(position.X + direction.X * d) - size/2,
                            (int)(position.Y + direction.Y * d) - size/2,
                            size, size
                        ),
                        new Color(255, 255, 200, (int)(alpha * 200))
                    );
                }
            }

            // Центр солнца (закрашенный круг с градиентом)
            for (int y = -radius; y <= radius; y += 3)
            {
                for (int x = -radius; x <= radius; x += 3)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        float dist = (float)Math.Sqrt(x * x + y * y) / radius;
                        Color sunGradient = new Color(
                            255,
                            (int)(220 * (1 - dist * 0.3f)),
                            (int)(50 * dist)
                        );
                        _spriteBatch.Draw(
                            new Rectangle((int)position.X + x, (int)position.Y + y, 3, 3),
                            sunGradient
                        );
                    }
                }
            }
            
            // Свечение вокруг солнца
            for (int r = radius + 5; r < radius + 20; r += 5)
            {
                for (int a = 0; a < 360; a += 15)
                {
                    float angle = a * MathHelper.Pi / 180;
                    float glowX = position.X + (float)Math.Cos(angle) * r;
                    float glowY = position.Y + (float)Math.Sin(angle) * r;
                    _spriteBatch.Draw(
                        new Rectangle((int)glowX - 2, (int)glowY - 2, 4, 4),
                        new Color(255, 200, 100, 100)
                    );
                }
            }
        }
        
        private void DrawStartMenu()
        {
            _spriteBatch.Begin();
            
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            int centerX = screenWidth / 2;
            int centerY = screenHeight / 2;
            
            // Затемнённый фон
            _spriteBatch.Draw(
                new Rectangle(0, 0, screenWidth, screenHeight),
                new Color(0, 0, 0, 180)
            );
            
            // Заголовок игры
            DrawCenteredText("PURPLE LORD", new Vector2(centerX, centerY - 100), Color.Purple, 2.5f);
            DrawCenteredText("Path of Choices", new Vector2(centerX, centerY - 50), Color.White, 1.2f);
            
            // Мигающий текст старта
            if (_menuTextVisible)
            {
                DrawCenteredText("НАЖМИТЕ ЛЮБУЮ КЛАВИШУ ДЛЯ СТАРТА", new Vector2(centerX, centerY + 50), Color.Yellow, 1f);
            }
            
            // Управление
            DrawCenteredText("Управление:", new Vector2(centerX, centerY + 120), Color.White, 0.9f);
            DrawCenteredText("A/D или ←/→ - Движение | Пробел/W - Прыжок | Escape - Выход", new Vector2(centerX, centerY + 155), Color.LightGray, 0.7f);
            
            // Описание игры
            DrawCenteredText("Собирайте кокосы и бананы для получения XP", new Vector2(centerX, centerY + 210), Color.Lime, 0.7f);
            DrawCenteredText("Избегайте колючих дикобразов!", new Vector2(centerX, centerY + 235), Color.Red, 0.7f);
            
            _spriteBatch.End();
        }
        
        private void DrawCenteredText(string text, Vector2 center, Color color, float scale)
        {
            int charWidth = 10;
            int charHeight = 14;
            int textWidth = (int)(text.Length * charWidth * scale);
            int textHeight = (int)(charHeight * scale);
            
            int startX = (int)center.X - textWidth / 2;
            int startY = (int)center.Y - textHeight / 2;
            
            int x = startX;
            int y = startY;
            
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    x += (int)(charWidth * scale);
                    continue;
                }
                
                // Рисуем символ
                _spriteBatch.Draw(new Rectangle(x, y, (int)(6 * scale), (int)(8 * scale)), color);
                x += (int)(charWidth * scale);
            }
        }

        private void DrawMountains(int startX, int baseY)
        {
            for (int i = 0; i < 5; i++)
            {
                int peakX = startX + i * 100;
                int peakHeight = 50 + (i % 3) * 30;
                int width = 80;
                
                for (int y = 0; y < peakHeight; y += 2)
                {
                    int rowWidth = width * (peakHeight - y) / peakHeight;
                    _spriteBatch.Draw(
                        new Rectangle(peakX - rowWidth / 2, baseY - y, rowWidth, 2),
                        _mountainColor
                    );
                }
            }
        }

        private void DrawUI()
        {
            _spriteBatch.Begin();

            // Заголовок
            DrawText("Purple Lord", new Vector2(15, 15), Color.Purple, 1.3f);
            DrawText("Path of Choices", new Vector2(15, 42), Color.DarkBlue, 0.8f);

            // Управление
            string controls = "A/D ←→ | Пробел Прыжок | Escape Выход";
            DrawText(controls, new Vector2(15, 700), Color.Black, 0.7f);

            // Позиция в мире
            string position = $"Позиция: {(int)_player.Position.X} / {WORLD_WIDTH}";
            DrawText(position, new Vector2(1100, 15), Color.DarkGreen, 0.7f);
            
            // === XP И УРОВЕНЬ ===
            string xpText = $"XP: {_player.XP}/{_player.XPToNextLevel} | Уровень: {_player.Level}";
            DrawText(xpText, new Vector2(15, 680), Color.Goldenrod, 0.9f);
            
            // Полоска XP
            int barWidth = 200;
            int barHeight = 12;
            float xpPercent = (float)_player.XP / _player.XPToNextLevel;
            
            // Фон полоски
            _spriteBatch.Draw(new Rectangle(15, 660, barWidth, barHeight), new Color(50, 50, 50));
            // Заполненная часть
            _spriteBatch.Draw(new Rectangle(15, 660, (int)(barWidth * xpPercent), barHeight), Color.Lime);
            // Рамка
            _spriteBatch.Draw(new Rectangle(15, 660, barWidth, 2), Color.White);
            _spriteBatch.Draw(new Rectangle(15, 660, 2, barHeight), Color.White);
            _spriteBatch.Draw(new Rectangle(15, 660 + barHeight - 2, barWidth, 2), Color.White);
            _spriteBatch.Draw(new Rectangle(15 + barWidth - 2, 660, 2, barHeight), Color.White);

            _spriteBatch.End();
        }

        private void DrawText(string text, Vector2 position, Color color, float scale)
        {
            // Простая отрисовка текста через прямоугольники (пиксельный шрифт)
            int x = (int)position.X;
            int y = (int)position.Y;
            int charSpacing = 8;
            
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    x += charSpacing;
                    continue;
                }
                
                // Рисуем символ как простой прямоугольник (заглушка)
                _spriteBatch.Draw(new Rectangle(x, y, 6, 8), color);
                x += charSpacing;
            }
        }
    }

    // ==================== КЛАССЫ ИГРЫ ====================

    public class Platform
    {
        public Rectangle Bounds;
        public bool IsGround;

        public Platform(int x, int y, int width, int height, bool isGround)
        {
            Bounds = new Rectangle(x, y, width, height);
            IsGround = isGround;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsGround)
            {
                // Трава
                spriteBatch.Draw(new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 10), _grassColor);
                
                // Земля
                spriteBatch.Draw(new Rectangle(Bounds.X, Bounds.Y + 10, Bounds.Width, Bounds.Height - 10), _dirtColor);
                
                // Детали травы
                for (int i = 0; i < Bounds.Width; i += 15)
                {
                    spriteBatch.Draw(new Rectangle(Bounds.X + i, Bounds.Y - 5, 4, 8), new Color(0, 180, 0));
                }
                
                // Детали земли (камешки)
                for (int i = 0; i < Bounds.Width; i += 40)
                {
                    spriteBatch.Draw(new Rectangle(Bounds.X + i + 10, Bounds.Y + 30, 6, 4), new Color(120, 70, 30));
                }
            }
            else
            {
                // Платформа в воздухе
                spriteBatch.Draw(new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 8), _grassColor);
                spriteBatch.Draw(new Rectangle(Bounds.X, Bounds.Y + 8, Bounds.Width, Bounds.Height - 8), _dirtColor);
            }
        }

        private Color _grassColor = new Color(34, 139, 34);
        private Color _dirtColor = new Color(139, 90, 43);
    }

    public class Cloud
    {
        public Vector2 Position;
        public float Speed;
        private float _scale;

        public Cloud(Vector2 position, float speed)
        {
            Position = position;
            Speed = speed;
            _scale = 0.8f + (speed / 40f);
        }

        public void Update(float delta, int worldWidth)
        {
            Position.X += Speed * delta;
            if (Position.X > worldWidth + 200)
                Position.X = -200;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color cloudColor = new Color(255, 255, 255, 230);
            
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 80, 40), cloudColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 35, (int)Position.Y - 25, 65, 50), cloudColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 80, (int)Position.Y, 75, 40), cloudColor);
        }
    }

    public class Bird
    {
        public Vector2 Position;
        public float Speed;
        private float _wingAngle;
        private float _startX;
        private float _amplitude;

        public Bird(Vector2 position, float speed)
        {
            Position = position;
            _startX = position.X;
            Speed = speed;
            _amplitude = 100f + (speed / 2f);
        }

        public void Update(float delta)
        {
            _wingAngle += 10f * delta;
            Position.X += (float)Math.Sin(_wingAngle * 0.3f) * Speed * delta;

            // Ограничиваем полёт
            if (Position.X < _startX - _amplitude) Position.X = _startX - _amplitude;
            if (Position.X > _startX + _amplitude) Position.X = _startX + _amplitude;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color white = Color.White;
            float wingOffset = (float)Math.Sin(_wingAngle) * 8f;
            
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 16, 3), white);
            spriteBatch.Draw(new Rectangle((int)Position.X + 3, (int)Position.Y - 6 + (int)wingOffset, 10, 2), white);
        }
    }

    public class PalmTree
    {
        public Vector2 Position;
        private float _height = 100f;

        public PalmTree(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color trunkColor = new Color(101, 67, 33);
            Color leafColor = new Color(0, 150, 0);

            // Ствол
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 16, (int)_height), trunkColor);

            // Листья
            float topX = Position.X + 8;
            float topY = Position.Y;

            for (int i = 0; i < 7; i++)
            {
                float angle = 1.57f + (i - 3) * 0.35f;
                float length = 45f + (i % 3) * 12f;
                
                for (int s = 0; s < (int)(length / 10); s++)
                {
                    float sx = topX + (float)Math.Cos(angle) * (s * 10);
                    float sy = topY - (float)Math.Sin(angle) * (s * 10);
                    int size = 8 - s;
                    if (size < 3) size = 3;
                    spriteBatch.Draw(new Rectangle((int)sx, (int)sy, size, size), leafColor);
                }
            }
        }
    }

    public class House
    {
        public Vector2 Position;
        public int Width = 80;
        public int Height = 60;
        public Color WallColor;

        public House(Vector2 position, Color wallColor)
        {
            Position = position;
            WallColor = wallColor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color roofColor = new Color(139, 69, 19);
            Color windowColor = new Color(100, 180, 220);
            Color doorColor = new Color(101, 67, 33);
            Color chimneyColor = new Color(150, 75, 30);

            // Стены
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, Width, Height), WallColor);

            // Крыша
            int roofHeight = 45;
            for (int row = 0; row < roofHeight; row += 2)
            {
                int rowWidth = Width + 20 - (row * 2);
                int xOffset = row;
                spriteBatch.Draw(
                    new Rectangle((int)Position.X - xOffset, (int)Position.Y - row, rowWidth, 2),
                    roofColor
                );
            }

            // Окна
            spriteBatch.Draw(new Rectangle((int)Position.X + 12, (int)Position.Y + 15, 18, 18), windowColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 50, (int)Position.Y + 15, 18, 18), windowColor);

            // Дверь
            spriteBatch.Draw(new Rectangle((int)Position.X + 30, (int)Position.Y + 35, 20, 25), doorColor);

            // Труба
            spriteBatch.Draw(new Rectangle((int)Position.X + 60, (int)Position.Y - 25, 10, 25), chimneyColor);
        }
    }

    public class SmokeParticle
    {
        public Vector2 Position;
        public float Life = 2.5f;
        public float MaxLife;
        public Vector2 Velocity;

        public SmokeParticle(Vector2 position)
        {
            Position = position;
            MaxLife = Life;
            Velocity = new Vector2(0, -12f);
        }

        public void Update(float delta)
        {
            Position += Velocity * delta;
            Velocity.X += (float)(new Random().NextDouble() - 0.5) * 8f * delta;
            Life -= delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = Life / MaxLife;
            Color smokeColor = new Color(220, 220, 220, (int)(alpha * 180));
            int size = (int)(10 + (1 - alpha) * 20);
            
            spriteBatch.Draw(
                new Rectangle((int)Position.X - size/2, (int)Position.Y - size/2, size, size),
                smokeColor
            );
        }
    }

    public class Flower
    {
        public Vector2 Position;
        public Color Color;

        public Flower(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Стебель
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 2, 10), new Color(0, 180, 0));

            // Лепестки
            spriteBatch.Draw(new Rectangle((int)Position.X - 4, (int)Position.Y - 4, 3, 3), Color);
            spriteBatch.Draw(new Rectangle((int)Position.X + 3, (int)Position.Y - 4, 3, 3), Color);
            spriteBatch.Draw(new Rectangle((int)Position.X - 4, (int)Position.Y + 1, 3, 3), Color);
            spriteBatch.Draw(new Rectangle((int)Position.X + 3, (int)Position.Y + 1, 3, 3), Color);

            // Центр
            spriteBatch.Draw(new Rectangle((int)Position.X - 1, (int)Position.Y - 1, 4, 4), Color.Yellow);
        }
    }
    
    // === ВОЛНЫ С ГРЕБНЯМИ ===
    public class Wave
    {
        public Vector2 Position;
        private float _phase;
        private float _speed;
        private float _amplitude;
        
        public Wave(float x, float y)
        {
            Position = new Vector2(x, y);
            _phase = (float)(new Random().NextDouble() * Math.PI * 2);
            _speed = 1.5f;
            _amplitude = 8f;
        }
        
        public void Update(float delta)
        {
            _phase += _speed * delta;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color waveColor = new Color(255, 255, 255, 180);
            Color foamColor = new Color(255, 255, 255, 220);
            
            // Рисуем волну синусоидальной формы
            int waveWidth = 40;
            float baseY = Position.Y;
            
            for (int x = 0; x < waveWidth; x += 3)
            {
                float yOffset = (float)Math.Sin(_phase + x * 0.15f) * _amplitude;
                int height = 8 + (int)(yOffset + _amplitude);
                
                // Гребень волны (белая пена)
                if (yOffset > _amplitude * 0.5f)
                {
                    spriteBatch.Draw(new Rectangle((int)Position.X + x, (int)(baseY - height), 4, 3), foamColor);
                }
                // Основная часть волны
                else
                {
                    spriteBatch.Draw(new Rectangle((int)Position.X + x, (int)(baseY - height + 3), 4, height - 3), waveColor);
                }
            }
        }
    }
    
    // === ВУЛКАН С ЛАВОЙ ===
    public class Volcano
    {
        public Vector2 Position;
        private float _eruptionTimer;
        private List<LavaParticle> _lavaParticles;
        private Random _random;
        
        public Volcano(Vector2 position)
        {
            Position = position;
            _lavaParticles = new List<LavaParticle>();
            _random = new Random();
        }
        
        public void Update(float delta)
        {
            _eruptionTimer += delta;
            
            // Периодические извержения
            if (_eruptionTimer > 2f && _random.NextSingle() < 0.3f)
            {
                _eruptionTimer = 0f;
                // Выброс лавы
                for (int i = 0; i < 5; i++)
                {
                    _lavaParticles.Add(new LavaParticle(
                        new Vector2(Position.X + _random.Next(-10, 10), Position.Y - 20),
                        new Vector2(_random.Next(-3, 4), _random.Next(-200, -100))
                    ));
                }
            }
            
            // Обновление частиц лавы
            for (int i = _lavaParticles.Count - 1; i >= 0; i--)
            {
                _lavaParticles[i].Update(delta);
                if (_lavaParticles[i].Life <= 0)
                    _lavaParticles.RemoveAt(i);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color volcanoColor = new Color(80, 60, 40);
            Color lavaColor = new Color(255, 100, 0);
            Color glowColor = new Color(255, 200, 50);
            
            // Конус вулкана
            int baseWidth = 120;
            int height = 100;
            
            for (int y = 0; y < height; y += 3)
            {
                int rowWidth = baseWidth * (height - y) / height;
                spriteBatch.Draw(
                    new Rectangle((int)Position.X - rowWidth / 2, (int)Position.Y - y, rowWidth, 3),
                    volcanoColor
                );
            }
            
            // Кратер с лавой
            spriteBatch.Draw(new Rectangle((int)Position.X - 15, (int)Position.Y - height - 5, 30, 10), lavaColor);
            
            // Свечение от лавы
            spriteBatch.Draw(new Rectangle((int)Position.X - 20, (int)Position.Y - height - 8, 40, 3), glowColor);
            
            // Частицы лавы
            foreach (var lava in _lavaParticles)
                lava.Draw(spriteBatch);
        }
    }
    
    public class LavaParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Life = 1.5f;
        public float MaxLife;
        
        public LavaParticle(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            MaxLife = Life;
        }
        
        public void Update(float delta)
        {
            Position += Velocity * delta;
            Velocity = new Vector2(Velocity.X, Velocity.Y + 400f * delta); // Гравитация
            Life -= delta;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = Life / MaxLife;
            Color lavaColor = new Color(255, (int)(100 * alpha), 0, (int)(255 * alpha));
            int size = (int)(6 + (1 - alpha) * 4);
            
            spriteBatch.Draw(
                new Rectangle((int)Position.X - size/2, (int)Position.Y - size/2, size, size),
                lavaColor
            );
        }
    }
    
    // === ПРЕДМЕТЫ ДЛЯ СБОРА ===
    public enum CollectibleType { Coconut, Banana }
    
    public class Collectible
    {
        public Vector2 Position;
        public CollectibleType Type;
        public bool IsCollected { get; set; } = false;
        private float _bobTimer;
        
        public Collectible(Vector2 position, CollectibleType type)
        {
            Position = position;
            Type = type;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsCollected) return;
            
            _bobTimer += 0.05f;
            float bobOffset = (float)Math.Sin(_bobTimer) * 3f;
            
            if (Type == CollectibleType.Coconut)
            {
                // Кокос - коричневый круг
                Color coconutColor = new Color(139, 90, 43);
                int x = (int)Position.X;
                int y = (int)(Position.Y + bobOffset);
                
                // Рисуем круг
                for (int dy = -10; dy <= 10; dy += 3)
                {
                    int rowWidth = (int)(2 * Math.Sqrt(100 - dy * dy));
                    spriteBatch.Draw(
                        new Rectangle(x - rowWidth / 2, y + dy, rowWidth, 3),
                        coconutColor
                    );
                }
                
                // Блик
                spriteBatch.Draw(new Rectangle(x - 3, y - 5, 4, 4), new Color(180, 140, 100));
            }
            else
            {
                // Банан - жёлтый полумесяц
                Color bananaColor = new Color(255, 220, 0);
                int x = (int)Position.X;
                int y = (int)(Position.Y + bobOffset);
                
                // Рисуем банан (изогнутая форма)
                for (int i = 0; i < 8; i++)
                {
                    int curveY = (int)(i * i * 0.5f);
                    spriteBatch.Draw(new Rectangle(x - 12 + i * 3, y + curveY, 4, 4), bananaColor);
                }
                
                // Хвостик
                spriteBatch.Draw(new Rectangle(x - 14, y, 4, 2), new Color(139, 90, 43));
            }
        }
    }
    
    // === ДИКОБРАЗ ===
    public class Porcupine
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsDead { get; set; } = false;
        private float _moveSpeed = 60f;
        private float _direction = 1;
        private float _animationTimer;
        private List<Platform> _platforms;
        
        public Porcupine(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
        
        public void Update(float delta, List<Platform> platforms)
        {
            _platforms = platforms;
            _animationTimer += delta;
            
            // Движение влево-вправо
            Velocity = new Vector2(_direction * _moveSpeed, Velocity.Y);
            Position = new Vector2(Position.X + Velocity.X * delta, Position.Y);
            
            // Гравитация
            Velocity = new Vector2(Velocity.X, Velocity.Y + 800f * delta);
            Position = new Vector2(Position.X, Position.Y + Velocity.Y * delta);
            
            // Коллизии с платформами
            CheckCollisions();
            
            // Смена направления на краях платформ или при столкновении
            bool shouldTurn = false;
            Rectangle ahead = new Rectangle(
                (int)Position.X + (int)(_direction * 20),
                (int)Position.Y + 15,
                5, 5
            );
            
            bool groundAhead = false;
            foreach (var platform in _platforms)
            {
                if (ahead.Intersects(platform.Bounds) && platform.IsGround)
                    groundAhead = true;
            }
            
            if (!groundAhead || Position.X <= 0 || Position.X >= 3000)
                shouldTurn = true;
            
            if (shouldTurn)
                _direction = -_direction;
        }
        
        private void CheckCollisions()
        {
            foreach (var platform in _platforms)
            {
                if (platform.IsGround)
                {
                    Rectangle porcupineBounds = new Rectangle(
                        (int)Position.X - 15,
                        (int)Position.Y - 15,
                        30, 30
                    );
                    
                    if (porcupineBounds.Intersects(platform.Bounds) && Velocity.Y >= 0)
                    {
                        Position = new Vector2(Position.X, platform.Bounds.Top - 15);
                        Velocity = new Vector2(Velocity.X, 0);
                    }
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsDead) return;
            
            Color bodyColor = new Color(139, 110, 80);
            Color spikeColor = new Color(200, 180, 150);
            Color eyeColor = Color.Black;
            
            int x = (int)Position.X;
            int y = (int)Position.Y;
            
            // Тело (овальное)
            spriteBatch.Draw(new Rectangle(x - 15, y - 10, 30, 20), bodyColor);
            
            // Колючки (треугольники по спине)
            float spikeAnim = (float)Math.Sin(_animationTimer * 10f) * 2f;
            for (int i = 0; i < 6; i++)
            {
                int spikeX = x - 12 + i * 5;
                int spikeY = y - 12 - (int)spikeAnim;
                spriteBatch.Draw(new Rectangle(spikeX, spikeY, 3, 6), spikeColor);
            }
            
            // Голова
            int headX = _direction > 0 ? x + 12 : x - 18;
            spriteBatch.Draw(new Rectangle(headX, y - 8, 12, 14), bodyColor);
            
            // Глаз
            int eyeX = _direction > 0 ? headX + 6 : headX + 2;
            spriteBatch.Draw(new Rectangle(eyeX, y - 5, 3, 3), eyeColor);
            
            // Нос
            int noseX = _direction > 0 ? headX + 10 : headX;
            spriteBatch.Draw(new Rectangle(noseX, y - 2, 4, 3), new Color(100, 80, 60));
            
            // Ноги (анимация ходьбы)
            float legOffset = (float)Math.Sin(_animationTimer * 15f) * 5f;
            spriteBatch.Draw(new Rectangle(x - 8, y + 8, 5, 7 + (int)legOffset), bodyColor);
            spriteBatch.Draw(new Rectangle(x + 3, y + 8, 5, 7 - (int)legOffset), bodyColor);
        }
    }
    
    // === ДЕЛЬФИН (прыгает в море) ===
    public class Dolphin
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private float _jumpCooldown;
        private bool _isJumping;
        private Random _random;
        
        public Dolphin(float x, float y)
        {
            Position = new Vector2(x, y);
            Velocity = Vector2.Zero;
            _random = new Random();
            _jumpCooldown = _random.NextSingle() * 3f;
        }
        
        public void Update(float delta)
        {
            if (_isJumping)
            {
                Position += Velocity * delta;
                Velocity = new Vector2(Velocity.X, Velocity.Y + 800f * delta); // Гравитация
                
                // Если упал в воду
                if (Position.Y > 580)
                {
                    Position = new Vector2(Position.X, 580);
                    _isJumping = false;
                    _jumpCooldown = 2f + _random.NextSingle() * 3f;
                }
            }
            else
            {
                _jumpCooldown -= delta;
                if (_jumpCooldown <= 0)
                {
                    // Прыжок!
                    _isJumping = true;
                    Velocity = new Vector2(150f, -400f + _random.NextSingle() * 100f);
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color dolphinColor = new Color(100, 150, 200);
            Color bellyColor = new Color(200, 200, 220);
            
            int x = (int)Position.X;
            int y = (int)Position.Y;
            
            // Тело (овальное, изогнутое при прыжке)
            if (_isJumping)
            {
                // Изогнутая форма в прыжке
                spriteBatch.Draw(new Rectangle(x - 20, y - 5, 40, 12), dolphinColor);
                // Хвост
                spriteBatch.Draw(new Rectangle(x - 25, y, 10, 8), dolphinColor);
                // Плавник
                spriteBatch.Draw(new Rectangle(x, y - 12, 8, 6), dolphinColor);
            }
            else
            {
                // Плывёт в воде (видна только часть)
                spriteBatch.Draw(new Rectangle(x - 15, y - 8, 30, 10), dolphinColor);
                // Плавник над водой
                spriteBatch.Draw(new Rectangle(x - 5, y - 15, 6, 8), dolphinColor);
            }
            
            // Глаз
            spriteBatch.Draw(new Rectangle(x + 10, y - 3, 3, 3), Color.Black);
        }
    }
    
    // === КОРАБЛЬ С ПАРУСАМИ ===
    public class Ship
    {
        public Vector2 Position;
        private float _bobTimer;
        
        public Ship(Vector2 position)
        {
            Position = position;
        }
        
        public void Update(float delta)
        {
            _bobTimer += delta;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            float bobOffset = (float)Math.Sin(_bobTimer * 1.5f) * 3f;
            int x = (int)Position.X;
            int y = (int)(Position.Y + bobOffset);
            
            Color hullColor = new Color(101, 67, 33);
            Color sailColor = new Color(245, 245, 220);
            Color mastColor = new Color(139, 90, 43);
            
            // Корпус корабля
            spriteBatch.Draw(new Rectangle(x - 40, y, 80, 20), hullColor);
            spriteBatch.Draw(new Rectangle(x - 35, y - 5, 70, 5), hullColor);
            
            // Мачта
            spriteBatch.Draw(new Rectangle(x - 3, y - 50, 6, 50), mastColor);
            
            // Паруса (два)
            spriteBatch.Draw(new Rectangle(x + 3, y - 45, 35, 25), sailColor);
            spriteBatch.Draw(new Rectangle(x + 3, y - 25, 30, 20), sailColor);
            
            // Флаг на вершине
            spriteBatch.Draw(new Rectangle(x + 3, y - 55, 15, 8), Color.Red);
            
            // Окна на корпусе
            spriteBatch.Draw(new Rectangle(x - 25, y + 5, 6, 6), new Color(60, 40, 20));
            spriteBatch.Draw(new Rectangle(x - 10, y + 5, 6, 6), new Color(60, 40, 20));
            spriteBatch.Draw(new Rectangle(x + 5, y + 5, 6, 6), new Color(60, 40, 20));
            spriteBatch.Draw(new Rectangle(x + 20, y + 5, 6, 6), new Color(60, 40, 20));
        }
    }
    
    // === ВСПЛЫВАЮЩИЙ ТЕКСТ +XP ===
    public class XPFloatText
    {
        public Vector2 Position;
        public string Text;
        public Color Color;
        public float Life = 1.5f;
        public float MaxLife;
        public float Scale;
        
        public XPFloatText(Vector2 position, string text, Color color)
        {
            Position = position;
            Text = text;
            Color = color;
            MaxLife = Life;
            Scale = 0.8f + new Random().NextSingle() * 0.3f;
        }
        
        public void Update(float delta)
        {
            Position = new Vector2(Position.X, Position.Y - 30f * delta);
            Life -= delta;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = Life / MaxLife;
            Color drawColor = new Color(Color.R, Color.G, Color.B, (int)(255 * alpha));
            
            int x = (int)Position.X;
            int y = (int)Position.Y;
            int charWidth = (int)(8 * Scale);
            int charHeight = (int)(10 * Scale);
            
            int startX = x - (Text.Length * charWidth) / 2;
            
            foreach (char c in Text)
            {
                if (c == ' ')
                {
                    startX += charWidth;
                    continue;
                }
                
                // Рисуем символ с тенью
                spriteBatch.Draw(new Rectangle(startX + 1, y + 1, charWidth - 2, charHeight - 2), Color.Black);
                spriteBatch.Draw(new Rectangle(startX, y, charWidth - 2, charHeight - 2), drawColor);
                startX += charWidth;
            }
        }
    }
    
    // === ЧАСТИЦЫ КРОВИ ===
    public class BloodParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Life = 1f;
        public float MaxLife;
        public float Size;
        
        public BloodParticle(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
            MaxLife = Life;
            Size = 3f + new Random().NextSingle() * 4f;
        }
        
        public void Update(float delta)
        {
            Position += Velocity * delta;
            Velocity = new Vector2(Velocity.X, Velocity.Y + 500f * delta); // Гравитация
            Velocity = new Vector2(Velocity.X * 0.98f, Velocity.Y); // Сопротивление воздуха
            Life -= delta;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = Life / MaxLife;
            Color bloodColor = new Color(180, 0, 0, (int)(200 * alpha));
            int size = (int)(Size * alpha);
            
            if (size > 0)
            {
                spriteBatch.Draw(
                    new Rectangle((int)Position.X - size/2, (int)Position.Y - size/2, size, size),
                    bloodColor
                );
            }
        }
    }
}
