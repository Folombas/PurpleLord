using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace PurpleLord
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        
        // Игрок
        private Vector2 _playerPos = new Vector2(150, 535);
        private Vector2 _playerVel = Vector2.Zero;
        private bool _isGrounded = true;
        private bool _facingRight = true;
        private bool _prevJump = false;
        private float _idleTimer = 0;
        
        // Камера
        private float _cameraX = 0;
        private float _cameraY = 0; // Вертикальная камера
        
        // Мир
        private const int GROUND_Y = 580;
        private List<Platform> _platforms;
        private List<Palm> _palms;
        private List<Item> _items;
        private List<Enemy> _enemies;
        
        // UI
        private int _xp = 0;
        private int _level = 1;
        private int _keysCount = 0;
        
        // Инвентарь и копание
        private bool _hasShovel = false; // Есть ли лопата
        private int _diamonds = 0; // Количество алмазов
        private int _rocks = 0; // Количество камней
        private bool _isDigging = false; // Копает ли сейчас
        private float _digTimer = 0; // Таймер анимации копания
        
        // Частицы земли при копании
        private List<DirtParticle> _dirtParticles;

        // Земля с камнями и алмазами
        private List<Rock> _rocksInGround;
        private List<Diamond> _diamondsInGround;
        private List<DugHole> _dugHoles; // Выкопанные ямы
        
        // Грунт для копания (сетка блоков земли)
        private List<DirtBlock> _dirtBlocks;
        
        // Меню
        private bool _showMenu = true;
        private float _menuBlink = 0;
        
        // Горы и вулканы
        private List<Mountain> _mountains;
        private List<Volcano> _volcanoes;
        private float _lavaTimer = 0;
        
        // Чайки и домики
        private List<Seagull> _seagulls;
        private List<House> _houses;
        private List<Smoke> _smokes;
        
        // Кровь
        private List<BloodDrop> _bloodDrops;
        
        // Джунгли на холмах
        private List<JungleHill> _jungleHills;

        // Кусты с клубникой
        private List<Bush> _bushes;

        // Пиратские сундуки
        private List<TreasureChest> _treasureChests;

        // Золотые ключи
        private List<Key> _keys;

        // Фоновые холмы (не двигаются с камерой)
        private List<BackgroundHill> _backgroundHills;

        // Туман на холмах
        private List<Fog> _fog;

        // Облака
        private List<Cloud> _clouds;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Window.Title = "Purple Lord Platformer";
        }

        protected override void Initialize()
        {
            _graphics.ApplyChanges();
            
            _platforms = new List<Platform>();
            _palms = new List<Palm>();
            _items = new List<Item>();
            _enemies = new List<Enemy>();
            _treasureChests = new List<TreasureChest>();
            _keys = new List<Key>();
            _backgroundHills = new List<BackgroundHill>();
            _fog = new List<Fog>();
            _rocksInGround = new List<Rock>();
            _diamondsInGround = new List<Diamond>();
            _dugHoles = new List<DugHole>();
            _dirtParticles = new List<DirtParticle>();
            _dirtBlocks = new List<DirtBlock>();
            
            // Генерируем землю (бесконечно вправо)
            for (int i = 0; i < 20; i++)
            {
                _platforms.Add(new Platform { X = i * 500, Y = GROUND_Y, W = 500, H = 200 });
            }
            
            // Пальмы с предметами - отдельно кокосы, отдельно бананы
            for (int i = 0; i < 5; i++)
            {
                int px = 300 + i * 1000;
                _palms.Add(new Palm { X = px, Y = GROUND_Y - 100 });
                // Кокосовая пальма - только кокосы (коричневые, +25 XP)
                _items.Add(new Item { X = px + 8, Y = GROUND_Y - 130, IsCoconut = true });
                _items.Add(new Item { X = px - 5, Y = GROUND_Y - 140, IsCoconut = true });
                _items.Add(new Item { X = px + 20, Y = GROUND_Y - 135, IsCoconut = true });
                _items.Add(new Item { X = px + 12, Y = GROUND_Y - 145, IsCoconut = true });
            }
            for (int i = 0; i < 5; i++)
            {
                int px = 800 + i * 1000;
                _palms.Add(new Palm { X = px, Y = GROUND_Y - 100 });
                // Банановая пальма - только бананы (жёлтые, +15 XP)
                _items.Add(new Item { X = px - 25, Y = GROUND_Y - 145, IsCoconut = false });
                _items.Add(new Item { X = px + 35, Y = GROUND_Y - 150, IsCoconut = false });
                _items.Add(new Item { X = px - 35, Y = GROUND_Y - 155, IsCoconut = false });
                _items.Add(new Item { X = px + 45, Y = GROUND_Y - 145, IsCoconut = false });
                _items.Add(new Item { X = px - 15, Y = GROUND_Y - 160, IsCoconut = false });
                _items.Add(new Item { X = px + 25, Y = GROUND_Y - 165, IsCoconut = false });
            }
            
            // Кусты с клубникой
            _bushes = new List<Bush>();
            for (int i = 0; i < 15; i++)
            {
                int bx = 150 + i * 350;
                _bushes.Add(new Bush { X = bx, Y = GROUND_Y - 25 });
                // Клубника на кусте (красная, +10 XP)
                _items.Add(new Item { X = bx - 15, Y = GROUND_Y - 35, IsCoconut = false, IsStrawberry = true });
                _items.Add(new Item { X = bx + 5, Y = GROUND_Y - 40, IsCoconut = false, IsStrawberry = true });
                _items.Add(new Item { X = bx + 20, Y = GROUND_Y - 35, IsCoconut = false, IsStrawberry = true });
            }
            
            // Враги (дикобразы)
            for (int i = 0; i < 8; i++)
            {
                _enemies.Add(new Enemy { X = 400 + i * 600, Y = GROUND_Y - 20, Type = 0 });
            }
            
            // Реки с крокодилами (каждые 2000 пикселей)
            for (int i = 1; i < 5; i++)
            {
                int riverX = i * 2000;
                _platforms.Add(new Platform { X = riverX - 60, Y = GROUND_Y, W = 120, H = 30, IsRiver = true });
                _enemies.Add(new Enemy { X = riverX - 30, Y = GROUND_Y - 15, Type = 1 });
            }
            
            // Горы на заднем плане
            _mountains = new List<Mountain>();
            for (int i = 0; i < 15; i++)
            {
                _mountains.Add(new Mountain { X = 2400 + i * 150, Height = 60 + (i % 3) * 40 });
            }
            
            // Вулканы
            _volcanoes = new List<Volcano>();
            _volcanoes.Add(new Volcano { X = 2500, Y = GROUND_Y - 120 });
            _volcanoes.Add(new Volcano { X = 2750, Y = GROUND_Y - 150 });
            
            // Чайки
            _seagulls = new List<Seagull>();
            for (int i = 0; i < 10; i++)
            {
                _seagulls.Add(new Seagull { X = 300 + i * 250, Y = 80 + (i % 3) * 40 });
            }
            
            // Домики
            _houses = new List<House>();
            _houses.Add(new House { X = 450, Y = GROUND_Y - 180, Color = new Color(220, 200, 170) });
            _houses.Add(new House { X = 950, Y = GROUND_Y - 180, Color = new Color(200, 180, 150) });
            _houses.Add(new House { X = 1450, Y = GROUND_Y - 180, Color = new Color(210, 190, 160) });
            
            // Дым
            _smokes = new List<Smoke>();
            
            // Кровь
            _bloodDrops = new List<BloodDrop>();
            
            // Джунгли на холмах
            _jungleHills = new List<JungleHill>();
            for (int i = 0; i < 8; i++)
            {
                _jungleHills.Add(new JungleHill 
                { 
                    X = 800 + i * 400, 
                    Height = 80 + (i % 3) * 40,
                    Width = 200 + (i % 2) * 100
                });
            }
            
            // Кусты с клубникой уже инициализированы выше

            // Пиратские сундуки на земле (случайные позиции)
            for (int i = 0; i < 12; i++)
            {
                int cx = 150 + _random.Next(0, 8000);
                bool hasShovel = _random.NextDouble() < 0.2; // 20% шанс лопаты в сундуке
                _treasureChests.Add(new TreasureChest { X = cx, Y = GROUND_Y - 35, HasShovel = hasShovel });
            }

            // Золотые ключи на пляже в песке (случайные позиции возле моря)
            for (int i = 0; i < 8; i++)
            {
                int kx = 2400 + _random.Next(0, 2000); // Пляж возле моря
                _keys.Add(new Key { X = kx, Y = GROUND_Y - 20 });
            }

            // === ТЕСТОВАЯ ЛОПАТА (в начале карты для теста) ===
            _hasShovel = true; // Даём лопату сразу для теста

            // Фоновые холмы (силуэты на заднем плане)
            for (int i = 0; i < 20; i++)
            {
                _backgroundHills.Add(new BackgroundHill
                {
                    X = i * 400,
                    Height = 100 + _random.Next(0, 150),
                    Width = 300 + _random.Next(0, 200)
                });
            }

            // Туман на холмах (полупрозрачные полосы)
            for (int i = 0; i < 15; i++)
            {
                _fog.Add(new Fog
                {
                    X = i * 600,
                    Y = GROUND_Y - 50 - _random.Next(0, 100),
                    Width = 400 + _random.Next(0, 300),
                    Height = 30 + _random.Next(0, 50)
                });
            }

            // Облака
            _clouds = new List<Cloud>();
            for (int i = 0; i < 20; i++)
            {
                _clouds.Add(new Cloud
                {
                    X = _random.Next(0, 5000),
                    Y = _random.Next(30, 200),
                    Speed = 20 + _random.NextSingle() * 30,
                    Scale = 0.8f + _random.NextSingle() * 0.6f
                });
            }

            // Камни и булыжники в земле (разных размеров и форм)
            for (int i = 0; i < 50; i++)
            {
                int rx = _random.Next(0, 10000);
                int ry = GROUND_Y + 10 + _random.Next(0, 100); // В земле, не на поверхности
                int size = 15 + _random.Next(0, 25); // Разные размеры
                int type = _random.Next(0, 3); // 0 = круглый, 1 = овальный, 2 = угловатый
                _rocksInGround.Add(new Rock { X = rx, Y = ry, Size = size, Type = type });
            }

            // Алмазы в земле (спрятаны под некоторыми камнями)
            for (int i = 0; i < 30; i++)
            {
                int dx = _random.Next(0, 10000);
                int dy = GROUND_Y + 30 + _random.Next(0, 80); // Глубже камней
                _diamondsInGround.Add(new Diamond { X = dx, Y = dy, Size = 8 + _random.Next(0, 6) });
            }

            // === ГРУНТ ДЛЯ КОПАНИЯ (сетка блоков земли под поверхностью) ===
            // Генерируем блоки грунта под каждой платформой
            foreach (var platform in _platforms)
            {
                if (!platform.IsRiver) // Не генерируем под реками
                {
                    // 3 ряда блоков грунта под поверхностью
                    for (int row = 0; row < 3; row++)
                    {
                        // Разбиваем платформу на блоки по 40 пикселей
                        for (int bx = platform.X; bx < platform.X + platform.W; bx += 40)
                        {
                            // 30% шанс что блок будет (для разнообразия)
                            if (_random.NextDouble() < 0.7)
                            {
                                _dirtBlocks.Add(new DirtBlock
                                {
                                    X = bx + 20, // Центр блока
                                    Y = GROUND_Y + 20 + row * 40,
                                    Size = 35 + _random.Next(0, 10), // Разные размеры
                                    Collected = false
                                });
                            }
                        }
                    }
                }
            }
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kb = Keyboard.GetState();
            
            // Меню
            if (_showMenu)
            {
                _menuBlink += dt;
                if (kb.IsKeyDown(Keys.Enter) || kb.IsKeyDown(Keys.Space) ||
                    kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D))
                    _showMenu = false;
                if (kb.IsKeyDown(Keys.Escape)) Exit();
                base.Update(gameTime);
                return;
            }
            
            // Выход
            if (kb.IsKeyDown(Keys.Escape)) Exit();
            
            // Движение
            float move = 0;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                move = -1;
                _facingRight = false;
                _idleTimer = 0;
            }
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                move = 1;
                _facingRight = true;
                _idleTimer = 0;
            }

            // Копание лопатой (клавиша K)
            if (_hasShovel && kb.IsKeyDown(Keys.K) && !_isDigging)
            {
                _isDigging = true;
                _digTimer = 0.3f; // Длительность анимации копания
                
                // Точка копания (где лопата касается земли)
                Vector2 digPoint = new Vector2(
                    _playerPos.X + (_facingRight ? 40 : -40),
                    GROUND_Y + 10
                );
                
                // Создаём частицы земли в разные стороны
                int particleCount = 15 + _random.Next(0, 10); // 15-25 частиц
                for (int i = 0; i < particleCount; i++)
                {
                    float angle = (float)(_random.NextDouble() * (float)Math.PI * 2); // Случайный угол 0-360°
                    float speed = 100 + (float)_random.NextDouble() * 200; // Случайная скорость
                    _dirtParticles.Add(new DirtParticle
                    {
                        X = digPoint.X,
                        Y = digPoint.Y,
                        VX = (float)Math.Cos(angle) * speed,
                        VY = (float)Math.Sin(angle) * speed - 150, // Вверх от земли
                        Size = 3 + (float)_random.NextDouble() * 5,
                        Life = 0.8f + (float)_random.NextDouble() * 0.5f,
                        Rotation = (float)_random.NextDouble() * (float)Math.PI * 2
                    });
                }
                
                // === КОПАЕМ ГРУНТ! ===
                // Проверяем блоки грунта в радиусе копания
                int digRadius = 70; // Радиус действия лопаты
                foreach (var block in _dirtBlocks)
                {
                    if (!block.Collected && 
                        Vector2.Distance(digPoint, new Vector2(block.X, block.Y)) < digRadius)
                    {
                        block.Collected = true; // Удаляем блок
                        _xp += 5; // +5 XP за каждый блок грунта
                        PlaySound(250, 0.08f); // Звук копания грунта
                        
                        // Создаём выкопанную яму в этом месте
                        _dugHoles.Add(new DugHole 
                        { 
                            X = digPoint.X, // Яма в точке копания
                            Y = digPoint.Y, // На уровне земли
                            Life = 10f, // Яма видна 10 секунд
                            Size = 50 + _random.Next(0, 20) // Размер ямы 50-70 пикселей
                        });
                        
                        // Дополнительные частицы из блока
                        for (int p = 0; p < 8; p++)
                        {
                            float angle = (float)(_random.NextDouble() * (float)Math.PI * 2);
                            float speed = 80 + (float)_random.NextDouble() * 150;
                            _dirtParticles.Add(new DirtParticle
                            {
                                X = block.X,
                                Y = block.Y,
                                VX = (float)Math.Cos(angle) * speed,
                                VY = (float)Math.Sin(angle) * speed - 50,
                                Size = 4 + (float)_random.NextDouble() * 4,
                                Life = 0.6f + (float)_random.NextDouble() * 0.4f,
                                Rotation = (float)_random.NextDouble() * (float)Math.PI * 2
                            });
                        }
                    }
                }
                
                // Проверяем, есть ли рядом камни или алмазы для копания
                foreach (var rock in _rocksInGround)
                {
                    if (!rock.Collected && Vector2.Distance(_playerPos, new Vector2(rock.X, rock.Y)) < 60)
                    {
                        rock.Collected = true;
                        _rocks++;
                        PlaySound(300, 0.1f); // Звук копания
                        // Создаём яму
                        _dugHoles.Add(new DugHole { X = rock.X, Y = rock.Y, Life = 5f });
                    }
                }
                
                // Проверяем алмазы (только если выкопали камень)
                foreach (var diamond in _diamondsInGround)
                {
                    if (!diamond.Collected && Vector2.Distance(_playerPos, new Vector2(diamond.X, diamond.Y)) < 80)
                    {
                        diamond.Collected = true;
                        _diamonds++;
                        _xp += 50; // Алмаз +50 XP
                        PlaySound(1500, 0.2f); // Звук алмаза
                        if (_xp >= 100) { _xp -= 100; _level++; }
                    }
                }
            }
            
            // Таймер анимации копания
            if (_isDigging)
            {
                _digTimer -= dt;
                if (_digTimer <= 0) _isDigging = false;
            }
            
            // Обновление частиц земли
            for (int i = _dirtParticles.Count - 1; i >= 0; i--)
            {
                var p = _dirtParticles[i];
                p.X += p.VX * dt;
                p.Y += p.VY * dt;
                p.VY += 300 * dt; // Гравитация
                p.VX *= 0.98f; // Сопротивление воздуха
                p.Rotation += 5 * dt; // Вращение частицы
                p.Life -= dt;
                if (p.Life <= 0) _dirtParticles.RemoveAt(i);
            }
            
            // Прыжок (только по нажатию!)
            bool jump = kb.IsKeyDown(Keys.Space) || kb.IsKeyDown(Keys.W);
            if (jump && !_prevJump)
            {
                if (_isGrounded)
                {
                    _playerVel.Y = -520;
                    _isGrounded = false;
                    // Звук прыжка
                    PlaySound(600, 0.1f);
                }
            }
            _prevJump = jump;
            
            // Физика
            _playerVel.X = move * 280;
            if (!_isGrounded) _playerVel.Y += 1400 * dt;
            else _playerVel.Y = 0;
            
            _playerPos += _playerVel * dt;
            
            // Коллизии с землёй
            _isGrounded = false;
            foreach (var p in _platforms)
            {
                if (!p.IsRiver && _playerPos.X > p.X && _playerPos.X < p.X + p.W &&
                    _playerPos.Y + 50 >= p.Y && _playerPos.Y + 50 <= p.Y + 20 && _playerVel.Y >= 0)
                {
                    _playerPos.Y = p.Y - 50;
                    _playerVel.Y = 0;
                    _isGrounded = true;
                }
            }
            
            // === ПРОВАЛИВАНИЕ В ЯМЫ (только если игрок стоит на поверхности ямы) ===
            foreach (var hole in _dugHoles)
            {
                if (hole.Life > 0)
                {
                    float distX = Math.Abs(_playerPos.X - hole.X);
                    float distY = _playerPos.Y - hole.Y;
                    
                    // Игрок должен быть НАД ямой (не глубоко)
                    if (distX < hole.Size / 2 && distY > -60 && distY < -20 && _playerVel.Y >= 0)
                    {
                        // Медленное проваливание
                        _playerPos.Y += 1.5f;
                        _playerVel.Y = 0;
                        _isGrounded = true;
                    }
                }
            }
            
            // Границы
            if (_playerPos.Y > 2500) _playerPos = new Vector2(150, 535); // Максимальная глубина
            if (_playerPos.X < 30) _playerPos.X = 30;
            
            // Камера следует за игроком (по X и Y)
            _cameraX = Math.Max(0, _playerPos.X - 640);
            // Вертикальная камера следует за игроком с ограничением
            float targetCameraY = _playerPos.Y - 360; // Цель камеры по Y
            if (targetCameraY < 0) targetCameraY = 0; // Не выше поверхности
            if (targetCameraY > 800) targetCameraY = 800; // Не слишком глубоко
            _cameraY = MathHelper.Lerp(_cameraY, targetCameraY, 0.05f); // Плавное слежение
            
            // === ГЕНЕРАЦИЯ НОВОЙ ЗЕМЛИ ПРИ КОПАНИИ ВНИЗ ===
            if (_playerPos.Y > GROUND_Y + 50)
            {
                // Игрок копает вниз - генерируем землю под ним
                int generateBelow = (int)_playerPos.Y + 300; // Генерируем ниже игрока
                
                // Генерируем только если ещё не генерировали на этой глубине
                if (generateBelow < 2000)
                {
                    // Проверяем, есть ли уже блоки на этой глубине
                    bool hasBlocks = false;
                    foreach (var block in _dirtBlocks)
                    {
                        if (block.Y > generateBelow - 50 && block.Y < generateBelow + 50)
                        {
                            hasBlocks = true;
                            break;
                        }
                    }
                    
                    // Если нет блоков - генерируем новый слой
                    if (!hasBlocks && _playerVel.Y >= 0)
                    {
                        // Генерируем блоки грунта по всей ширине экрана
                        for (int bx = (int)(_cameraX - 200); bx < _cameraX + 1500; bx += 35)
                        {
                            _dirtBlocks.Add(new DirtBlock
                            {
                                X = bx + 17,
                                Y = generateBelow,
                                Size = 30 + _random.Next(0, 10),
                                Collected = false
                            });
                        }
                        
                        // Генерируем камни
                        for (int i = 0; i < 3; i++)
                        {
                            _rocksInGround.Add(new Rock 
                            { 
                                X = (int)(_cameraX + _random.Next(0, 1280)), 
                                Y = generateBelow + _random.Next(20, 80), 
                                Size = 15 + _random.Next(0, 20), 
                                Type = _random.Next(0, 3) 
                            });
                        }
                        
                        // Генерируем алмазы
                        for (int i = 0; i < 2; i++)
                        {
                            _diamondsInGround.Add(new Diamond 
                            { 
                                X = (int)(_cameraX + _random.Next(0, 1280)), 
                                Y = generateBelow + _random.Next(50, 120), 
                                Size = 8 + _random.Next(0, 8) 
                            });
                        }
                    }
                }
            }
            
            // Обновление облаков
            foreach (var cloud in _clouds)
            {
                cloud.X += cloud.Speed * dt;
                if (cloud.X > _cameraX + 1400)
                    cloud.X = _cameraX - 200;
            }

            // Сбор предметов
            foreach (var item in _items)
            {
                if (!item.Collected && Vector2.Distance(_playerPos, new Vector2(item.X, item.Y)) < 50)
                {
                    item.Collected = true;
                    if (item.IsStrawberry)
                    {
                        _xp += 10; // Клубника +10 XP
                        PlaySound(1200, 0.1f); // Высокий звук
                    }
                    else if (item.IsCoconut)
                    {
                        _xp += 25; // Кокос +25 XP
                        PlaySound(880, 0.15f);
                    }
                    else
                    {
                        _xp += 15; // Банан +15 XP
                        PlaySound(880, 0.15f);
                    }
                    if (_xp >= 100) { _xp -= 100; _level++; }
                }
            }

            // Сбор золотых ключей
            foreach (var key in _keys)
            {
                if (!key.Collected && Vector2.Distance(_playerPos, new Vector2(key.X, key.Y)) < 50)
                {
                    key.Collected = true;
                    _keysCount++;
                    PlaySound(1000, 0.15f); // Звук подбора ключа
                }
            }

            // Открытие пиратских сундуков
            foreach (var chest in _treasureChests)
            {
                if (!chest.Opened && Vector2.Distance(_playerPos, new Vector2(chest.X, chest.Y)) < 50)
                {
                    if (_keysCount > 0)
                    {
                        chest.Opened = true;
                        _keysCount--;
                        
                        // Проверяем, есть ли в сундуке лопата (20% шанс)
                        if (chest.HasShovel)
                        {
                            _hasShovel = true;
                            PlaySound(700, 0.2f); // Звук нахождения лопаты
                        }
                        else
                        {
                            _xp += 100; // Сундук с сокровищами +100 XP
                            PlaySound(600, 0.3f); // Звук открытия сундука
                            if (_xp >= 100) { _xp -= 100; _level++; }
                        }
                    }
                }
            }
            
            // Столкновения с врагами
            foreach (var e in _enemies)
            {
                if (Vector2.Distance(_playerPos, new Vector2(e.X, e.Y)) < 55)
                {
                    _xp = Math.Max(0, _xp - 20);
                    _playerPos.X += _playerPos.X < e.X ? -50 : 50;
                    _playerPos.Y -= 30;

                    // Негативный звук урона (низкий тон)
                    PlaySound(150, 0.3f);

                    // КРОВЬ! 20 капель в разные стороны
                    for (int i = 0; i < 20; i++)
                    {
                        _bloodDrops.Add(new BloodDrop
                        {
                            X = _playerPos.X,
                            Y = _playerPos.Y,
                            VX = (float)(_random.NextDouble() - 0.5) * 400f,
                            VY = (float)(_random.NextDouble() - 0.5) * 400f - 200f,
                            Size = 3 + (float)_random.NextDouble() * 5f,
                            Life = 1.5f
                        });
                    }
                }
            }
            
            // Движение врагов
            foreach (var e in _enemies)
            {
                e.DirTimer += dt;
                if (e.DirTimer > 2) { e.Dir *= -1; e.DirTimer = 0; }
                e.X += e.Dir * 30 * dt;
            }
            
            // Чайки летают
            foreach (var s in _seagulls)
            {
                s.X += s.Speed * dt;
                s.Y += (float)Math.Sin(s.Timer) * 0.5f;
                s.Timer += dt * 2;
                if (s.X > _cameraX + 1400) s.X = _cameraX - 100;
            }
            
            // Дым из труб
            foreach (var h in _houses)
            {
                if (_random.NextDouble() < 0.05)
                {
                    // Дым идёт из центра трубы (справа от дома)
                    int chimneyX = h.X + 210 - 40 + 30/2; // X дома + ширина стены - позиция трубы + половина ширины трубы
                    int chimneyY = h.Y - 60; // Высота трубы
                    _smokes.Add(new Smoke { X = chimneyX, Y = chimneyY - 10, Life = 2 });
                }
            }
            for (int i = _smokes.Count - 1; i >= 0; i--)
            {
                _smokes[i].Y -= 20 * dt;
                _smokes[i].X += (float)(_random.NextDouble() - 0.5) * 30 * dt;
                _smokes[i].Size += 10 * dt;
                _smokes[i].Life -= dt;
                if (_smokes[i].Life <= 0) _smokes.RemoveAt(i);
            }
            
            // Кровь - физика капель
            for (int i = _bloodDrops.Count - 1; i >= 0; i--)
            {
                var b = _bloodDrops[i];
                b.X += b.VX * dt;
                b.Y += b.VY * dt;
                b.VY += 800 * dt; // Гравитация
                b.VX *= 0.98f; // Сопротивление воздуха
                b.Life -= dt;
                if (b.Life <= 0) _bloodDrops.RemoveAt(i);
            }
            
            base.Update(gameTime);
        }
        
        private Random _random = new Random();
        
        // Простая генерация звука
        private void PlaySound(int frequency, float duration)
        {
            try
            {
                int sampleRate = 44100;
                int samples = (int)(sampleRate * duration);
                byte[] samplesData = new byte[samples * 2];
                
                for (int i = 0; i < samples; i++)
                {
                    float t = (float)i / sampleRate;
                    float envelope = 1 - t / duration;
                    short sample = (short)((float)Math.Sin(2 * Math.PI * frequency * t) * envelope * 16000);
                    samplesData[i * 2] = (byte)(sample & 0xFF);
                    samplesData[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
                }
                
                SoundEffect sound = new SoundEffect(samplesData, sampleRate, AudioChannels.Mono);
                sound.Play();
            }
            catch { }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(135, 206, 235));

            _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(-_cameraX, -_cameraY, 0));

            // Солнце с лучами (теперь привязано к камере - всегда видно слева)
            DrawSun((int)_cameraX + 100, 80 + (int)_cameraY);

            // === ФОНОВЫЕ ХОЛМЫ (не двигаются с камерой - параллакс) ===
            // Рисуем холмы без смещения камеры - они стоят на месте
            foreach (var hill in _backgroundHills)
            {
                // Силуэт холма (тёмно-синий/серый градиент)
                for (int y = 0; y < hill.Height; y += 3)
                {
                    int w = hill.Width * (hill.Height - y) / hill.Height;
                    int hillY = GROUND_Y - y;
                    // Градиент от тёмного к светлому (туманная дымка)
                    byte alpha = (byte)(100 - y / 3);
                    Color hillColor = new Color(60 + y / 10, 60 + y / 10, 80 + y / 10, alpha);
                    DrawRect(hill.X - w / 2, hillY, w, 3, hillColor);
                }
            }

            // === ТУМАН НА ХОЛМАХ ===
            foreach (var f in _fog)
            {
                // Полупрозрачный белый туман
                Color fogColor = new Color(200, 200, 220, 80);
                DrawRect((int)f.X, (int)f.Y, (int)f.Width, (int)f.Height, fogColor);
                
                // Дополнительные слои тумана (для пушистости)
                Color fogColor2 = new Color(220, 220, 230, 50);
                DrawRect((int)f.X - 20, (int)f.Y + 10, (int)(f.Width / 2), (int)(f.Height / 2), fogColor2);
                DrawRect((int)f.X + (int)(f.Width / 2), (int)f.Y - 10, (int)(f.Width / 3), (int)(f.Height / 3), fogColor2);
            }

            _spriteBatch.End();

            // === ОБЛАКА (рисуем без трансформации камеры - стоят на месте) ===
            _spriteBatch.Begin();
            foreach (var cloud in _clouds)
            {
                // Пушистые белые облака из нескольких кругов
                int cx = (int)cloud.X;
                int cy = (int)cloud.Y;
                float s = cloud.Scale;

                // Основная часть облака
                DrawCircle(cx, cy, (int)(25 * s), new Color(255, 255, 255, 240));
                DrawCircle(cx + (int)(15 * s), cy, (int)(20 * s), new Color(255, 255, 255, 240));
                DrawCircle(cx - (int)(15 * s), cy, (int)(20 * s), new Color(255, 255, 255, 240));
                DrawCircle(cx + (int)(10 * s), cy - (int)(10 * s), (int)(18 * s), new Color(255, 255, 255, 240));
                DrawCircle(cx - (int)(10 * s), cy - (int)(8 * s), (int)(16 * s), new Color(255, 255, 255, 240));

                // для пушистости - ещё круги
                DrawCircle(cx + (int)(20 * s), cy - (int)(5 * s), (int)(15 * s), new Color(255, 255, 255, 230));
                DrawCircle(cx - (int)(20 * s), cy - (int)(5 * s), (int)(14 * s), new Color(255, 255, 255, 230));
                DrawCircle(cx, cy - (int)(12 * s), (int)(17 * s), new Color(255, 255, 255, 235));
            }
            _spriteBatch.End();

            // Возвращаемся к рендерингу с камерой
            _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(-_cameraX, -_cameraY, 0));
            
            // Чайки
            foreach (var s in _seagulls)
            {
                // Крылья
                float wingOffset = (float)Math.Sin(s.Timer) * 8f;
                DrawRect((int)s.X - 10, (int)s.Y, 8, 3, Color.White);
                DrawRect((int)s.X + 2, (int)s.Y, 8, 3, Color.White);
                DrawRect((int)s.X - 10, (int)(s.Y - 5 + wingOffset), 8, 2, Color.White);
                DrawRect((int)s.X + 2, (int)(s.Y - 5 - wingOffset), 8, 2, Color.White);
            }
            
            // Море справа
            DrawRect(2600, GROUND_Y - 40, 2000, 60, new Color(30, 144, 255));
            
            // Джунгли на холмах
            foreach (var hill in _jungleHills)
            {
                // Холм
                for (int y = 0; y < hill.Height; y += 3)
                {
                    int w = hill.Width * (hill.Height - y) / hill.Height;
                    Color grassGrad = new Color(34 + y / 3, 139 - y / 4, 34);
                    DrawRect(hill.X - w/2, GROUND_Y - y, w, 3, grassGrad);
                }
                // Пальмы на холме
                int palmCount = 2 + (hill.Width / 100);
                for (int p = 0; p < palmCount; p++)
                {
                    int px = hill.X - hill.Width/3 + p * (hill.Width / palmCount);
                    int py = GROUND_Y - hill.Height + 10;
                    // Ствол
                    DrawRect(px, py, 8, 40, new Color(101, 67, 33));
                    // Листья
                    for (int l = 0; l < 5; l++)
                    {
                        float angle = 1.57f + (l - 2) * 0.4f;
                        for (int s = 0; s < 3; s++)
                        {
                            float sx = px + 4 + (float)Math.Cos(angle) * (s * 12);
                            float sy = py - (float)Math.Sin(angle) * (s * 12);
                            DrawRect((int)sx, (int)sy, 6 - s, 6 - s, new Color(0, 180, 0));
                        }
                    }
                }
            }
            
            // Горы на заднем плане
            foreach (var m in _mountains)
            {
                for (int y = 0; y < m.Height; y += 3)
                {
                    int w = m.Width * (m.Height - y) / m.Height;
                    DrawRect(m.X - w/2, GROUND_Y - 40 - y, w, 3, new Color(100, 100, 150));
                }
            }
            
            // Вулканы с лавой
            _lavaTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var v in _volcanoes)
            {
                // Конус вулкана
                for (int y = 0; y < 100; y += 3)
                {
                    int w = 120 * (100 - y) / 100;
                    DrawRect((int)v.X - w/2, (int)v.Y - y, w, 3, new Color(80, 60, 40));
                }
                // Кратер с лавой
                DrawRect((int)v.X - 15, (int)v.Y - 105, 30, 10, new Color(255, 100, 0));
                // Лава извергается
                for (int i = 0; i < 8; i++)
                {
                    float lx = v.X + (float)Math.Sin(_lavaTimer * 3 + i) * 15;
                    float ly = v.Y - 105 - (float)Math.Abs(Math.Sin(_lavaTimer * 5 + i)) * 30;
                    DrawRect((int)lx - 3, (int)ly, 6, 6, new Color(255, 50, 0));
                }
            }
            
            // Земля/платформы
            foreach (var p in _platforms)
            {
                if (p.IsRiver)
                {
                    DrawRect(p.X, p.Y, p.W, p.H, new Color(65, 105, 225));
                }
                else
                {
                    DrawRect(p.X, p.Y, p.W, 10, new Color(34, 139, 34));
                    DrawRect(p.X, p.Y + 10, p.W, p.H - 10, new Color(139, 90, 43));
                }
            }

            // === БЛОКИ ГРУНТА (которые можно копать) ===
            foreach (var block in _dirtBlocks)
            {
                if (!block.Collected)
                {
                    int bx = (int)block.X;
                    int by = (int)block.Y;
                    int size = (int)block.Size;
                    
                    // Основной блок земли (коричневый с текстурой)
                    DrawRect(bx - size/2, by - size/2, size, size, new Color(139, 90, 43));
                    
                    // Текстура грунта (комки земли)
                    DrawCircle(bx - size/4, by - size/4, size/6, new Color(120, 70, 30));
                    DrawCircle(bx + size/5, by + size/6, size/7, new Color(100, 60, 25));
                    DrawCircle(bx - size/6, by + size/4, size/8, new Color(110, 65, 28));
                    
                    // Травинки сверху (если блок у поверхности)
                    if (by < GROUND_Y + 40)
                    {
                        for (int g = 0; g < 3; g++)
                        {
                            int gx = bx - size/2 + g * (size/2);
                            DrawRect(gx, by - size/2 - 5, 3, 5, new Color(34, 139, 34));
                        }
                    }
                    
                    // Блеск влаги на блоке
                    if ((gameTime.TotalGameTime.TotalSeconds * 3) % 1 < 0.3f)
                    {
                        DrawRect(bx + size/6, by - size/3, 4, 4, new Color(160, 110, 60));
                    }
                }
            }

            // === КАМНИ И БУЛЫЖНИКИ В ЗЕМЛЕ ===
            foreach (var rock in _rocksInGround)
            {
                if (!rock.Collected)
                {
                    int rx = (int)rock.X;
                    int ry = (int)rock.Y;
                    int size = rock.Size;

                    if (rock.Type == 0) // Круглый камень
                    {
                        DrawCircle(rx, ry, size, new Color(100, 100, 100));
                        DrawCircle(rx - size/3, ry - size/3, size/3, new Color(120, 120, 120));
                    }
                    else if (rock.Type == 1) // Овальный камень
                    {
                        DrawRect(rx - size, ry - size/2, size * 2, size, new Color(110, 100, 90));
                        DrawRect(rx - size/2, ry - size/3, size, size/2, new Color(130, 120, 110));
                    }
                    else // Угловатый камень
                    {
                        // Рисуем как многоугольник из прямоугольников
                        DrawRect(rx - size, ry - size/2, size * 2, size/3, new Color(90, 90, 100));
                        DrawRect(rx - size/2, ry, size, size * 2/3, new Color(100, 100, 110));
                        DrawRect(rx + size/3, ry - size/3, size/2, size/2, new Color(80, 80, 90));
                    }
                    
                    // Блеск на камне
                    DrawRect(rx - size/4, ry - size/4, 4, 4, new Color(180, 180, 180));
                }
            }

            // === АЛМАЗЫ В ЗЕМЛЕ (сверкают) ===
            foreach (var diamond in _diamondsInGround)
            {
                if (!diamond.Collected)
                {
                    int dx = (int)diamond.X;
                    int dy = (int)diamond.Y;
                    int size = diamond.Size;

                    // Алмаз (ромб)
                    for (int i = 0; i < size; i += 2)
                    {
                        int w = (size - i) * 2;
                        DrawRect(dx - w/2, dy - size + i, w, 2, new Color(100, 200, 255));
                        DrawRect(dx - w/2, dy + size - i, w, 2, new Color(100, 200, 255));
                    }
                    
                    // Сверкание алмаза
                    if ((gameTime.TotalGameTime.TotalSeconds * 8) % 1 < 0.5f)
                    {
                        DrawRect(dx - 2, dy - 2, 4, 4, Color.White);
                        DrawRect(dx + size/2, dy, 3, 3, new Color(200, 255, 255));
                        DrawRect(dx - size/2, dy + size/2, 3, 3, new Color(200, 255, 255));
                    }
                }
            }

            // === ВЫКОПАННЫЕ ЯМЫ (видны 10 секунд) ===
            foreach (var hole in _dugHoles)
            {
                hole.Life -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (hole.Life > 0)
                {
                    // Яма (тёмное углубление в земле)
                    int holeSize = (int)hole.Size;
                    
                    // Внешняя часть ямы (тень)
                    DrawCircle((int)hole.X, (int)hole.Y, holeSize + 10, new Color(60, 40, 20, 180));
                    
                    // Внутренняя часть ямы (глубина)
                    DrawCircle((int)hole.X, (int)hole.Y, holeSize - 5, new Color(40, 25, 10, 220));
                    
                    // Края ямы (комки земли вокруг)
                    for (int e = 0; e < 8; e++)
                    {
                        float edgeAngle = e * (float)Math.PI / 4;
                        int edgeX = (int)hole.X + (int)(Math.Cos(edgeAngle) * holeSize);
                        int edgeY = (int)hole.Y + (int)(Math.Sin(edgeAngle) * holeSize);
                        DrawCircle(edgeX, edgeY, 8, new Color(139, 90, 43, 200));
                    }
                    
                    // Дно ямы (самая тёмная часть)
                    DrawCircle((int)hole.X, (int)hole.Y, holeSize / 3, new Color(20, 12, 5, 255));
                }
            }
            // Удаляем исчезнувшие ямы
            _dugHoles.RemoveAll(h => h.Life <= 0);

            // === ЧАСТИЦЫ ЗЕМЛИ (разлетаются при копании) ===
            foreach (var p in _dirtParticles)
            {
                // Рисуем частицу как вращающийся квадрат земли
                int size = (int)p.Size;
                Color dirtColor = new Color(139, 90, 43, (int)(p.Life * 255));
                
                // Рисуем несколько квадратов для эффекта комка земли
                DrawRect((int)p.X - size/2, (int)p.Y - size/2, size, size, dirtColor);
                DrawRect((int)p.X + size/3, (int)p.Y - size/3, size/2, size/2, new Color(120, 70, 30, (int)(p.Life * 200)));
                DrawRect((int)p.X - size/4, (int)p.Y + size/4, size/3, size/3, new Color(100, 60, 25, (int)(p.Life * 180)));
            }

            // Домики (увеличенные в 3 раза)
            foreach (var h in _houses)
            {
                int hx = h.X;
                int hy = h.Y;
                int wallWidth = 210;  // 70 * 3
                int wallHeight = 180; // 60 * 3
                int roofHeight = 120; // 40 * 3

                // Стены (кирпичные, увеличенные)
                DrawRect(hx, hy, wallWidth, wallHeight, h.Color);
                
                // Кирпичная текстура стен
                for (int row = 0; row < wallHeight; row += 18)
                {
                    for (int col = 0; col < wallWidth; col += 30)
                    {
                        int offset = (row / 18) % 2 * 15;
                        DrawRect(hx + col + offset, hy + row, 28, 2, new Color(h.Color.R - 30, h.Color.G - 30, h.Color.B - 30));
                    }
                }

                // === КРАСИВАЯ ДВУСКАТНАЯ КРЫША ===
                // Левый скат крыши (идёт вверх и вправо к центру)
                for (int i = 0; i < roofHeight; i += 4)
                {
                    int slopeWidth = i * 2 + 15;
                    int roofX = hx + wallWidth/2 - slopeWidth/2;
                    int roofY = hy - roofHeight + i;
                    DrawRect(roofX, roofY, slopeWidth, 4, new Color(139, 69, 19));
                    // Черепица
                    if (i % 12 == 0)
                    {
                        DrawRect(roofX + 5, roofY, slopeWidth - 10, 2, new Color(100, 50, 15));
                    }
                }
                // Правый скат крыши (идёт вверх и влево к центру)
                for (int i = 0; i < roofHeight; i += 4)
                {
                    int slopeWidth = i * 2 + 15;
                    int roofX = hx + wallWidth/2 - slopeWidth/2;
                    int roofY = hy - roofHeight + i;
                    DrawRect(roofX, roofY, slopeWidth, 4, new Color(139, 69, 19));
                    // Черепица
                    if (i % 12 == 0)
                    {
                        DrawRect(roofX + 5, roofY, slopeWidth - 10, 2, new Color(100, 50, 15));
                    }
                }
                // Конёк крыши (верх)
                DrawRect(hx + wallWidth/2 - 15, hy - roofHeight - 5, 30, 10, new Color(160, 80, 30));
                // Декоративный элемент на коньке
                DrawRect(hx + wallWidth/2 - 5, hy - roofHeight - 15, 10, 12, new Color(180, 100, 40));

                // === ОКНА С РАМАМИ (увеличенные в 3 раза) ===
                // Левое окно
                int win1X = hx + 30;
                int win1Y = hy + 45;
                int winSize = 45; // 15 * 3
                // Стекло
                DrawRect(win1X, win1Y, winSize, winSize, new Color(100, 180, 220));
                // Рама (деревянная)
                DrawRect(win1X - 6, win1Y - 6, winSize + 12, 6, new Color(101, 67, 33)); // Верх
                DrawRect(win1X - 6, win1Y + winSize, winSize + 12, 6, new Color(101, 67, 33)); // Низ
                DrawRect(win1X - 6, win1Y - 6, 6, winSize + 12, new Color(101, 67, 33)); // Лево
                DrawRect(win1X + winSize, win1Y - 6, 6, winSize + 12, new Color(101, 67, 33)); // Право
                // Переплёт (крест)
                DrawRect(win1X + winSize/2 - 3, win1Y - 6, 6, winSize + 12, new Color(101, 67, 33));
                DrawRect(win1X - 6, win1Y + winSize/2 - 3, winSize + 12, 6, new Color(101, 67, 33));
                // Подоконник
                DrawRect(win1X - 10, win1Y + winSize + 8, winSize + 20, 8, new Color(120, 80, 40));

                // Правое окно
                int win2X = hx + wallWidth - 30 - winSize;
                int win2Y = hy + 45;
                // Стекло
                DrawRect(win2X, win2Y, winSize, winSize, new Color(100, 180, 220));
                // Рама (деревянная)
                DrawRect(win2X - 6, win2Y - 6, winSize + 12, 6, new Color(101, 67, 33)); // Верх
                DrawRect(win2X - 6, win2Y + winSize, winSize + 12, 6, new Color(101, 67, 33)); // Низ
                DrawRect(win2X - 6, win2Y - 6, 6, winSize + 12, new Color(101, 67, 33)); // Лево
                DrawRect(win2X + winSize, win2Y - 6, 6, winSize + 12, new Color(101, 67, 33)); // Право
                // Переплёт (крест)
                DrawRect(win2X + winSize/2 - 3, win2Y - 6, 6, winSize + 12, new Color(101, 67, 33));
                DrawRect(win2X - 6, win2Y + winSize/2 - 3, winSize + 12, 6, new Color(101, 67, 33));
                // Подоконник
                DrawRect(win2X - 10, win2Y + winSize + 8, winSize + 20, 8, new Color(120, 80, 40));

                // === ДВЕРЬ С ЗАМКОМ (увеличенная в 3 раза) ===
                int doorX = hx + wallWidth/2 - 30;
                int doorY = hy + wallHeight - 75; // Дверь внизу стены
                int doorWidth = 60;  // 20 * 3
                int doorHeight = 75; // 25 * 3
                // Дверное полотно
                DrawRect(doorX, doorY, doorWidth, doorHeight, new Color(101, 67, 33));
                // Дверная коробка
                DrawRect(doorX - 6, doorY - 6, doorWidth + 12, 6, new Color(120, 80, 50));
                DrawRect(doorX - 6, doorY, 6, doorHeight, new Color(120, 80, 50));
                DrawRect(doorX + doorWidth, doorY, 6, doorHeight, new Color(120, 80, 50));
                // Дверные панели (филенки)
                DrawRect(doorX + 10, doorY + 10, 15, 25, new Color(80, 50, 25));
                DrawRect(doorX + 35, doorY + 10, 15, 25, new Color(80, 50, 25));
                DrawRect(doorX + 10, doorY + 40, 15, 25, new Color(80, 50, 25));
                DrawRect(doorX + 35, doorY + 40, 15, 25, new Color(80, 50, 25));
                // Дверная ручка
                DrawCircle(doorX + doorWidth - 12, doorY + doorHeight/2, 5, new Color(255, 215, 0));
                // Замок (скважина)
                DrawRect(doorX + doorWidth - 15, doorY + doorHeight/2 + 10, 6, 10, Color.Black);
                // Замочная пластина
                DrawRect(doorX + doorWidth - 18, doorY + doorHeight/2 + 8, 12, 16, new Color(200, 150, 100));

                // === ТРУБА (увеличенная в 3 раза) ===
                int chimneyX = hx + wallWidth - 40;
                int chimneyY = hy - 60;
                int chimneyWidth = 30; // 10 * 3
                int chimneyHeight = 60; // 20 * 3
                DrawRect(chimneyX, chimneyY, chimneyWidth, chimneyHeight, new Color(150, 75, 30));
                // Кирпичи на трубе
                for (int row = 0; row < chimneyHeight; row += 12)
                {
                    DrawRect(chimneyX, chimneyY + row, chimneyWidth, 2, new Color(120, 60, 25));
                }
                // Оголовок трубы (расширение сверху)
                DrawRect(chimneyX - 6, chimneyY - 10, chimneyWidth + 12, 15, new Color(160, 85, 35));
            }
            
            // Дым
            foreach (var s in _smokes)
            {
                int alpha = (int)(s.Life / 2f * 200);
                DrawRect((int)s.X - (int)s.Size/2, (int)s.Y - (int)s.Size/2, (int)s.Size, (int)s.Size, 
                    new Color(200, 200, 200, alpha));
            }
            
            // Кровь
            foreach (var b in _bloodDrops)
            {
                int alpha = (int)(b.Life / 1.5f * 255);
                DrawCircle((int)b.X, (int)b.Y, (int)b.Size, new Color(180, 0, 0, alpha));
            }
            
            // Пальмы
            foreach (var palm in _palms)
            {
                // Ствол пальмы (коричневый с текстурой)
                DrawRect(palm.X, palm.Y, 16, 100, new Color(101, 67, 33));
                // Текстура ствола (кольца)
                for (int r = 0; r < 10; r++)
                {
                    DrawRect(palm.X - 8, palm.Y + r * 10, 16, 2, new Color(80, 50, 20));
                }
                
                // Длинные зелёные листья (пальмовые ветви)
                float topX = palm.X + 8;
                float topY = palm.Y;
                
                // 9 длинных изогнутых листьев
                for (int i = 0; i < 9; i++)
                {
                    float angle = 1.57f + (i - 4) * 0.30f; // От -1.2 до +1.2 радиан
                    float length = 70f + (i % 3) * 15f; // Разная длина: 70, 85, 100
                    
                    // Рисуем каждый лист как длинную изогнутую ветвь
                    for (int s = 0; s < (int)(length / 8); s++)
                    {
                        // Изгиб листа вниз
                        float curveAngle = angle + s * 0.08f;
                        float sx = topX + (float)Math.Cos(curveAngle) * (s * 8);
                        float sy = topY - (float)Math.Sin(curveAngle) * (s * 8) + s * 3;
                        
                        // Ширина листа уменьшается к концу
                        int leafWidth = Math.Max(3, 12 - s);
                        int leafHeight = Math.Max(2, 8 - s / 2);
                        
                        // Градиент цвета листа (светлее к концу)
                        Color leafColor = new Color(
                            20 + s * 3,
                            180 - s * 5,
                            20 + s * 2
                        );
                        
                        // Рисуем сегмент листа
                        DrawRect((int)sx - leafWidth/2, (int)sy - leafHeight/2, leafWidth, leafHeight, leafColor);
                        
                        // Добавляем мелкие листья по бокам для пушистости
                        if (s > 2 && s % 2 == 0)
                        {
                            float sideAngle = curveAngle + 0.5f;
                            float sideX = sx + (float)Math.Cos(sideAngle) * 6;
                            float sideY = sy - (float)Math.Sin(sideAngle) * 6;
                            DrawRect((int)sideX - 3, (int)sideY - 2, 6, 4, new Color(30, 160, 30));
                            
                            sideAngle = curveAngle - 0.5f;
                            sideX = sx + (float)Math.Cos(sideAngle) * 6;
                            sideY = sy - (float)Math.Sin(sideAngle) * 6;
                            DrawRect((int)sideX - 3, (int)sideY - 2, 6, 4, new Color(30, 160, 30));
                        }
                    }
                    
                    // Центральная жила листа (тёмная)
                    for (int s = 0; s < (int)(length / 10); s++)
                    {
                        float curveAngle = angle + s * 0.08f;
                        float sx = topX + (float)Math.Cos(curveAngle) * (s * 8);
                        float sy = topY - (float)Math.Sin(curveAngle) * (s * 8) + s * 3;
                        DrawRect((int)sx - 1, (int)sy - 1, 2, 2, new Color(0, 100, 0));
                    }
                }
            }
            
            // Кусты с клубникой
            foreach (var bush in _bushes)
            {
                // Куст (зелёный круг)
                DrawCircle(bush.X, bush.Y, 20, new Color(34, 100, 34));
                // Листья
                for (int l = 0; l < 5; l++)
                {
                    float angle = 1.57f + (l - 2) * 0.4f;
                    DrawRect(
                        bush.X + (int)((float)Math.Cos(angle) * 15),
                        bush.Y + (int)((float)Math.Sin(angle) * 10) - 10,
                        8, 8, new Color(0, 150, 0)
                    );
                }
            }

            // Предметы
            foreach (var item in _items)
            {
                if (!item.Collected)
                {
                    if (item.IsStrawberry)
                    {
                        // Клубника - красный треугольник с зелёным хвостиком
                        DrawRect((int)item.X - 6, (int)item.Y, 12, 10, Color.Red);
                        DrawRect((int)item.X - 3, (int)item.Y - 4, 6, 4, new Color(0, 150, 0));
                        // Зёрнышки
                        DrawRect((int)item.X - 4, (int)item.Y + 2, 2, 2, Color.Yellow);
                        DrawRect((int)item.X + 2, (int)item.Y + 2, 2, 2, Color.Yellow);
                        DrawRect((int)item.X - 1, (int)item.Y + 5, 2, 2, Color.Yellow);
                    }
                    else if (item.IsCoconut)
                        DrawCircle((int)item.X, (int)item.Y, 10, new Color(139, 90, 43));
                    else
                        DrawRect((int)item.X - 12, (int)item.Y, 24, 8, Color.Yellow);
                }
            }

            // Пиратские сундуки (увеличенные в 1.5 раза)
            foreach (var chest in _treasureChests)
            {
                int cx = (int)chest.X;
                int cy = (int)chest.Y + 10; // Сундук стоит на земле

                // Масштаб 1.5x
                int w = 30; // половина ширины (было 20)
                int h = 38; // высота (было 25)

                if (chest.Opened)
                {
                    // Открытый сундук - видны сокровища
                    // Корпус
                    DrawRect(cx - w, cy, w * 2, h, new Color(101, 67, 33));
                    // Открытая крышка (сдвинута назад)
                    for (int row = 0; row < 18; row += 3)
                    {
                        int cw = w * 2 - row * 2;
                        DrawRect(cx - cw/2, cy - row - 22, cw, 4, new Color(139, 90, 43));
                    }
                    // Сокровища внутри (золотые монеты блестят) - увеличенные
                    DrawCircle(cx - 12, cy + 8, 8, new Color(255, 215, 0));
                    DrawCircle(cx + 12, cy + 8, 8, new Color(255, 215, 0));
                    DrawCircle(cx, cy + 12, 9, new Color(255, 255, 100));
                    // Блеск сокровищ
                    if ((gameTime.TotalGameTime.TotalSeconds * 5) % 1 < 0.5f)
                    {
                        DrawRect(cx - 5, cy + 5, 3, 3, Color.White);
                        DrawRect(cx + 8, cy + 5, 3, 3, Color.White);
                    }
                }
                else
                {
                    // Закрытый сундук
                    // Основной корпус сундука (тёмно-коричневый)
                    DrawRect(cx - w, cy, w * 2, h, new Color(101, 67, 33));
                    // Крышка сундука (полукруглая)
                    for (int row = 0; row < 18; row += 3)
                    {
                        int cw = w * 2 - row * 2;
                        DrawRect(cx - cw/2, cy - row, cw, 4, new Color(139, 90, 43));
                    }
                    // Золотая окантовка
                    DrawRect(cx - w, cy - 4, w * 2, 6, new Color(255, 215, 0));
                    DrawRect(cx - w, cy + h - 4, w * 2, 4, new Color(255, 215, 0));
                    // Замок (увеличенный)
                    DrawCircle(cx, cy + 15, 9, new Color(255, 215, 0));
                    DrawRect(cx - 3, cy + 9, 6, 9, new Color(200, 150, 0));
                    // Металлические полосы (увеличенные)
                    DrawRect(cx - w + 3, cy + 8, 9, 27, new Color(180, 150, 100));
                    DrawRect(cx + w - 12, cy + 8, 9, 27, new Color(180, 150, 100));
                }
            }

            // Золотые ключи на пляже
            foreach (var key in _keys)
            {
                if (!key.Collected)
                {
                    int kx = (int)key.X;
                    int ky = (int)key.Y;

                    // Золотой ключ (лежит в песке)
                    // Стержень ключа
                    DrawRect(kx - 15, ky - 2, 20, 4, new Color(255, 215, 0));
                    // Головка ключа (круглая)
                    DrawCircle(kx - 12, ky, 8, new Color(255, 215, 0));
                    // Зубцы ключа
                    DrawRect(kx + 5, ky + 2, 4, 6, new Color(255, 215, 0));
                    DrawRect(kx + 8, ky + 2, 3, 4, new Color(255, 215, 0));
                    // Блеск ключа
                    if ((gameTime.TotalGameTime.TotalSeconds * 3) % 1 < 0.5f)
                    {
                        DrawRect(kx - 14, ky - 2, 3, 3, Color.White);
                    }
                }
            }
            
            // Враги
            foreach (var e in _enemies)
            {
                if (e.Type == 0) // Дикобраз
                {
                    int ex = (int)e.X;
                    int ey = (int)e.Y;
                    int faceDir = e.Dir > 0 ? 1 : -1;

                    // Тело (коричневое)
                    DrawRect(ex - 15, ey - 10, 30, 20, new Color(139, 110, 80));
                    
                    // Иголки (светло-коричневые)
                    for (int i = 0; i < 6; i++)
                        DrawRect(ex - 12 + i * 5, ey - 15, 3, 8, new Color(200, 180, 150));
                    
                    // Голова (мордочка)
                    DrawRect(ex + (faceDir > 0 ? 10 : -20), ey - 8, 15, 14, new Color(139, 110, 80));
                    
                    // Глаз (белок + зрачок)
                    int eyeX = ex + (faceDir > 0 ? 18 : -12);
                    int eyeY = ey - 6;
                    DrawCircle(eyeX, eyeY, 5, Color.White);
                    DrawRect(eyeX + faceDir * 2, eyeY - 2, 3, 4, Color.Black);
                    
                    // Нос (чёрный треугольник)
                    int noseX = ex + (faceDir > 0 ? 24 : -24);
                    DrawRect(noseX - 3, ey - 4, 6, 5, Color.Black);
                    
                    // Рот (линия)
                    DrawRect(ex + (faceDir > 0 ? 12 : -18), ey, 10, 2, new Color(80, 60, 40));
                    
                    // Ножки (4 короткие лапки)
                    DrawRect(ex - 10, ey + 8, 8, 7, new Color(100, 80, 60));
                    DrawRect(ex - 2, ey + 8, 8, 7, new Color(100, 80, 60));
                    DrawRect(ex + 6, ey + 8, 8, 7, new Color(100, 80, 60));
                    DrawRect(ex + 14, ey + 8, 8, 7, new Color(100, 80, 60));
                    
                    // Коготки на лапках
                    for (int c = 0; c < 4; c++)
                    {
                        DrawRect(ex - 10 + c * 8, ey + 15, 3, 3, Color.White);
                    }
                }
                else // Крокодил
                {
                    int cx = (int)e.X;
                    int cy = (int)e.Y;
                    int faceDir = e.Dir > 0 ? 1 : -1;

                    // Тело (тёмно-зелёное с синим оттенком - чтобы не сливалось с травой)
                    Color crocBody = new Color(20, 100, 80);
                    Color crocLight = new Color(30, 140, 100);
                    Color crocDark = new Color(15, 70, 60);

                    // Тело (зелёное, длинное)
                    DrawRect(cx - 25, cy - 5, 50, 15, crocBody);
                    
                    // Хвост
                    for (int t = 0; t < 5; t++)
                    {
                        int tw = 20 - t * 3;
                        DrawRect(cx - 25 - t * 6, cy - 3 - t, tw, 8 - t, crocBody);
                    }
                    
                    // Голова (удлинённая пасть)
                    int headX = cx + (faceDir > 0 ? 20 : -30);
                    DrawRect(headX, cy - 8, 25, 16, crocBody);
                    
                    // Глаз (жёлтый с вертикальным зрачком)
                    int eyeX = headX + (faceDir > 0 ? 8 : 12);
                    int eyeY = cy - 10;
                    DrawCircle(eyeX, eyeY, 6, new Color(255, 200, 0));
                    DrawRect(eyeX - 2, eyeY - 4, 4, 8, Color.Black); // Вертикальный зрачок
                    
                    // Нос (два отверстия)
                    int noseX = headX + (faceDir > 0 ? 20 : 0);
                    DrawRect(noseX - 2, cy - 6, 4, 3, Color.Black);
                    DrawRect(noseX + 4, cy - 6, 4, 3, Color.Black);
                    
                    // === СТРАШНЫЕ ОСТРЫЕ ЗУБЫ (много!) ===
                    int mouthY = cy;
                    // Верхняя челюсть - 8 острых зубов
                    for (int t = 0; t < 8; t++)
                    {
                        int toothX = headX + (faceDir > 0 ? 2 + t * 3 : 20 - t * 3);
                        // Треугольный зуб (рисуем как прямоугольник)
                        DrawRect(toothX, mouthY - 2, 4, 6, Color.White);
                        // Остриё зуба
                        DrawRect(toothX + 1, mouthY + 4, 2, 4, Color.White);
                    }
                    
                    // Нижняя челюсть
                    DrawRect(headX + 2, cy + 6, 20, 6, crocLight);
                    
                    // Нижние зубы - 7 зубов (в промежутках)
                    for (int t = 0; t < 7; t++)
                    {
                        int toothX = headX + (faceDir > 0 ? 5 + t * 3 : 17 - t * 3);
                        DrawRect(toothX, cy + 6, 4, 5, Color.White);
                        DrawRect(toothX + 1, cy + 2, 2, 4, Color.White);
                    }
                    
                    // Ножки (4 короткие лапы с перепонками)
                    // Передняя левая
                    DrawRect(cx - 15, cy + 8, 10, 8, crocDark);
                    DrawRect(cx - 18, cy + 16, 16, 4, crocLight); // Перепонка
                    
                    // Передняя правая
                    DrawRect(cx - 3, cy + 8, 10, 8, crocDark);
                    DrawRect(cx - 6, cy + 16, 16, 4, crocLight); // Перепонка
                    
                    // Задняя левая
                    DrawRect(cx + 8, cy + 8, 10, 8, crocDark);
                    DrawRect(cx + 5, cy + 16, 16, 4, crocLight); // Перепонка
                    
                    // Задняя правая
                    DrawRect(cx + 18, cy + 8, 10, 8, crocDark);
                    DrawRect(cx + 15, cy + 16, 16, 4, crocLight); // Перепонка
                    
                    // Когти на лапах
                    for (int c = 0; c < 4; c++)
                    {
                        DrawRect(cx - 18 + c * 8, cy + 20, 3, 3, Color.White);
                    }
                    
                    // Чешуя на спине (бугорки) - тёмные
                    for (int s = 0; s < 6; s++)
                    {
                        DrawCircle(cx - 20 + s * 8, cy - 8, 4, crocDark);
                    }
                    
                    // Контур тела (тёмная обводка для видимости)
                    DrawRect(cx - 25, cy - 6, 50, 2, crocDark);
                    DrawRect(cx - 25, cy + 10, 50, 2, crocDark);
                }
            }
            
            // Игрок
            DrawPlayer();
            
            _spriteBatch.End();
            
            // UI
            _spriteBatch.Begin();
            DrawText($"XP: {_xp} | Уровень: {_level}", 15, 15, Color.Goldenrod, 1.5f);
            DrawText($"Ключи: {_keysCount}", 15, 45, Color.Gold, 1.5f);
            DrawText($"Алмазы: {_diamonds} | Камни: {_rocks}", 15, 75, Color.Cyan, 1.2f);
            if (_hasShovel)
                DrawText("Лопата: есть! (K - копать)", 15, 100, Color.Orange, 1.2f);
            DrawText("A/D - Бег | Пробел - Прыжок | K - Копать грунт | Escape - Выход", 15, 680, Color.White, 1f);
            
            // Полоска XP
            DrawRect(15, 660, 200, 12, new Color(50, 50, 50));
            DrawRect(15, 660, (int)((_xp / 100f) * 200), 12, Color.Lime);
            DrawRect(15, 660, 200, 2, Color.White);
            DrawRect(15, 660, 2, 12, Color.White);
            DrawRect(15, 670, 200, 2, Color.White);
            DrawRect(213, 660, 2, 12, Color.White);
            
            // Меню
            if (_showMenu)
            {
                DrawRect(0, 0, 1280, 720, new Color(0, 0, 0, 180));
                DrawText("ФИОЛЕТОВЫЙ ЛОРД", 640 - 180, 260, Color.Purple, 3f);
                DrawText("Путь Выбора", 640 - 90, 310, Color.White, 1.8f);
                if (_menuBlink % 1 < 0.5f)
                    DrawText("НАЖМИТЕ ЛЮБУЮ КЛАВИШУ", 640 - 180, 380, Color.Yellow, 1.8f);
                DrawText("Управление: A/D - Бег | Пробел - Прыжок | K - Копать", 640 - 200, 450, Color.LightGray, 1.2f);
                DrawText("Собирайте кокосы и бананы для XP!", 640 - 170, 495, Color.Lime, 1.2f);
                DrawText("Избегайте дикобразов и крокодилов!", 640 - 170, 535, Color.Red, 1.2f);
                DrawText("Ищите лопату в сундуках!", 640 - 140, 575, Color.Orange, 1.2f);
                DrawText("Копайте землю и находите алмазы!", 640 - 160, 610, Color.Cyan, 1.2f);
            }
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
        private void DrawPlayer()
        {
            int x = (int)_playerPos.X;
            int y = (int)_playerPos.Y;

            // === СКЕЛЕТ ГЕРОЯ ===
            
            // Ноги (ботинки на земле)
            float legAngle = 0f;
            if (!_isGrounded)
            {
                // В прыжке ноги поджаты
                legAngle = _facingRight ? 0.4f : -0.4f;
            }
            else if (Math.Abs(_playerVel.X) > 10)
            {
                // Анимация бега
                legAngle = (float)Math.Sin(DateTime.Now.Millisecond / 60.0) * 0.5f;
            }

            // Ноги идут от талии вниз к земле (вертикально)
            // Левая нога (начинается ниже, чтобы ботинки были на земле)
            DrawLeg(x - 6, y + 5, -legAngle - 0.1f);
            // Правая нога
            DrawLeg(x + 6, y + 5, legAngle + 0.1f);

            // Туловище (прямоугольное, фиолетовое) - от талии вверх
            int torsoWidth = 24;
            int torsoHeight = 35;
            DrawRect(x - torsoWidth/2, y - torsoHeight - 10, torsoWidth, torsoHeight, new Color(138, 43, 226));
            
            // === КРАСИВЫЙ РЕМЕНЬ С ЗОЛОТОЙ БЛЯХОЙ ===
            // Ремень (коричневая полоса)
            DrawRect(x - torsoWidth/2, y - 8, torsoWidth, 10, new Color(101, 67, 33));
            // Золотая бляха (пряжка)
            DrawRect(x - 6, y - 10, 12, 14, new Color(255, 215, 0));
            // Блеск на бляхе
            DrawRect(x - 3, y - 7, 6, 8, new Color(255, 255, 150));
            // Отверстия для ремня
            for (int h = 0; h < 3; h++)
            {
                DrawRect(x - torsoWidth/2 + 4 + h * 5, y - 5, 3, 3, new Color(60, 40, 20));
            }
            // Язычок пряжки
            DrawRect(x, y - 8, 2, 8, new Color(200, 150, 0));

            // Шея
            DrawRect(x - 4, y - torsoHeight - 18, 8, 10, new Color(138, 43, 226));

            // Голова (круглая)
            DrawCircle(x, y - torsoHeight - 24, 16, new Color(138, 43, 226));

            // Капюшон над головой
            for (int i = 0; i < 6; i++)
            {
                int w = 36 - i * 2;
                DrawRect(x - w/2, y - torsoHeight - 38 - i, w, 2, new Color(100, 30, 180));
            }

            // Глаза на голове
            int faceX = _facingRight ? x + 4 : x - 12;
            DrawRect(faceX, y - torsoHeight - 28, 6, 6, Color.White);
            DrawRect(faceX + 8, y - torsoHeight - 28, 6, 6, Color.White);
            DrawRect(faceX + (_facingRight ? 2 : 0), y - torsoHeight - 26, 3, 3, Color.Black);
            DrawRect(faceX + 10 + (_facingRight ? 2 : 0), y - torsoHeight - 26, 3, 3, Color.Black);

            // Улыбка
            if (_isGrounded && Math.Abs(_playerVel.X) < 10)
            {
                DrawRect(faceX + 2, y - torsoHeight - 20, 10, 3, new Color(255, 220, 180));
            }

            // Руки от плеч (симметрично)
            int shoulderY = y - torsoHeight - 5;
            float armAngle = _facingRight ? 0.2f : -0.2f;

            if (_isGrounded && Math.Abs(_playerVel.X) > 10)
            {
                armAngle += (float)Math.Sin(DateTime.Now.Millisecond / 60.0) * 0.5f;
            }

            // Левая рука (слева от тела)
            DrawArm(x - 12, shoulderY, armAngle - 0.3f, false);
            // Правая рука (справа от тела)
            DrawArm(x + 12, shoulderY, armAngle + 0.3f, true);

            // === ЛОПАТА (если есть и игрок копает) ===
            if (_hasShovel && _isDigging)
            {
                // Точка копания
                Vector2 digPoint = new Vector2(
                    _playerPos.X + (_facingRight ? 40 : -40),
                    GROUND_Y + 10
                );
                
                // Анимация лопаты при копании - замах и удар о землю
                float digProgress = 1 - (_digTimer / 0.3f); // 0 до 1 во время копания
                float shovelAngle;
                int shovelY;
                
                // Фаза замаха (0-0.5) и фаза удара (0.5-1)
                if (digProgress < 0.5f)
                {
                    // Замах вверх
                    shovelAngle = (float)Math.Sin(digProgress * Math.PI) * 1.2f;
                    shovelY = y - torsoHeight - 10;
                }
                else
                {
                    // Удар вниз к точке копания
                    shovelAngle = (float)Math.Sin((1 - digProgress) * Math.PI) * 0.8f - 0.3f;
                    // Интерполяция к точке копания
                    shovelY = (int)(y - torsoHeight + MathHelper.Lerp(0, 40, (digProgress - 0.5f) * 2));
                }
                
                int shovelX = _facingRight ? x + 20 : x - 20;

                // Ручка лопаты (коричневая)
                float handleLength = 35f;
                float angle = shovelAngle + (_facingRight ? 0.5f : -0.5f);
                for (int i = 0; i < (int)(handleLength / 4); i++)
                {
                    float hx = shovelX + (float)Math.Sin(angle) * (i * 4);
                    float hy = shovelY + (float)Math.Cos(angle) * (i * 4);
                    DrawRect((int)hx - 3, (int)hy - 3, 6, 6, new Color(139, 90, 43));
                }

                // Лезвие лопаты (серое, металлическое) - следует к точке копания
                float bladeX = shovelX + (float)Math.Sin(angle) * handleLength;
                float bladeY = shovelY + (float)Math.Cos(angle) * handleLength;
                
                // В фазе удара лезвие ближе к точке копания
                if (digProgress > 0.5f)
                {
                    float t = (digProgress - 0.5f) * 2;
                    bladeX = MathHelper.Lerp(bladeX, digPoint.X, t);
                    bladeY = MathHelper.Lerp(bladeY, digPoint.Y, t);
                }
                
                DrawRect((int)bladeX - 8, (int)bladeY, 16, 12, new Color(150, 150, 160));
                DrawRect((int)bladeX - 6, (int)bladeY + 2, 12, 8, new Color(180, 180, 190));
                
                // === ЭФФЕКТ КОНТАКТА С ЗЕМЛЁЙ ===
                if (digProgress > 0.7f && digProgress < 0.95f)
                {
                    // Лопата в земле - рисуем землю вокруг лезвия
                    int dirtSize = 20 + (int)((digProgress - 0.7f) * 100);
                    
                    // Куски земли вокруг лопаты
                    for (int d = 0; d < 6; d++)
                    {
                        int dx = (int)bladeX + _random.Next(-dirtSize, dirtSize);
                        int dy = (int)bladeY + _random.Next(0, dirtSize);
                        int ds = 5 + _random.Next(0, 10);
                        DrawCircle(dx, dy, ds, new Color(139, 90, 43, 220));
                    }
                    
                    // Частицы земли от контакта
                    if (_random.NextDouble() < 0.5)
                    {
                        int px = (int)bladeX + _random.Next(-15, 15);
                        int py = (int)bladeY + _random.Next(5, 20);
                        DrawRect(px, py, 6, 6, new Color(120, 70, 30, 200));
                    }
                    
                    // Блеск металла лопаты в земле
                    DrawRect((int)bladeX - 4, (int)bladeY + 4, 8, 4, new Color(220, 220, 230));
                }

                // Эффект копания (частицы земли) - только в момент контакта
                if (digProgress > 0.75f && digProgress < 0.9f)
                {
                    for (int p = 0; p < 8; p++)
                    {
                        int dirtX = (int)bladeX + _random.Next(-25, 25);
                        int dirtY = (int)bladeY + _random.Next(0, 15);
                        int size = 4 + _random.Next(0, 6);
                        DrawRect(dirtX, dirtY, size, size, new Color(139, 90, 43, 200));
                    }
                }
            }
        }

        private void DrawArm(int x, int y, float angle, bool isRight)
        {
            // Длина руки
            float armLength = 20f;

            // Плечо
            DrawCircle(x, y, 8, new Color(138, 43, 226));

            // Рукава (рисуем руку вниз под углом)
            for (int i = 0; i < (int)(armLength / 3); i++)
            {
                float px = x + (float)Math.Sin(angle) * (i * 3);
                float py = y + (float)Math.Cos(angle) * (i * 3);
                int width = 10 - i / 2;
                DrawRect((int)px - width/2, (int)py - width/2, width, width, new Color(138, 43, 226));
            }

            // Белая перчатка
            float handX = x + (float)Math.Sin(angle) * armLength;
            float handY = y + (float)Math.Cos(angle) * armLength;

            // Ладонь
            DrawCircle((int)handX, (int)handY, 7, Color.White);

            // 3 пальца
            for (int f = 0; f < 3; f++)
            {
                float fingerAngle = angle - 0.3f + f * 0.3f;
                for (int s = 0; s < 2; s++)
                {
                    float fx = handX + (float)Math.Sin(fingerAngle) * (5 + s * 4);
                    float fy = handY + (float)Math.Cos(fingerAngle) * (5 + s * 4);
                    DrawRect((int)fx - 2, (int)fy - 2, 4, 4, Color.White);
                }
            }
        }

        private void DrawLeg(int x, int y, float angle)
        {
            // Длина ноги - рисуем вертикально вниз до земли
            float legLength = 45f;

            // Нога идёт вниз от бедра (angle отклоняется немного при ходьбе)
            for (int i = 0; i < (int)(legLength / 3); i++)
            {
                float px = x + (float)Math.Sin(angle) * (i * 2);
                float py = y + i * 3; // Вертикально вниз
                int width = 12 - i / 2;
                DrawRect((int)px - width/2, (int)py - width/2, width, width, new Color(138, 43, 226));
            }

            // Коричневый ботинок внизу (на земле)
            float bootX = x + (float)Math.Sin(angle) * legLength;
            float bootY = y + legLength;

            // Основной ботинок (горизонтально на земле)
            DrawRect((int)bootX - 10, (int)bootY - 6, 20, 12, new Color(101, 67, 33));

            // Шнурки
            DrawRect((int)bootX - 6, (int)bootY - 3, 12, 2, Color.White);

            // 3 пальца на ботинке
            for (int t = 0; t < 3; t++)
            {
                float toeX = bootX + 8 + t * 4;
                float toeY = bootY + (float)Math.Sin(angle) * 3;
                DrawRect((int)toeX - 2, (int)toeY - 2, 4, 4, new Color(101, 67, 33));
            }
        }
        
        private void DrawSun(int x, int y)
        {
            float time = (float)(DateTime.Now.Millisecond / 1000.0);
            
            // Длинные жёлтые лучи с изменяющейся длиной (излучение энергии)
            for (int i = 0; i < 16; i++)
            {
                float baseAngle = i * (float)Math.PI / 8;
                
                // Пульсация длины луча (от 60 до 120 пикселей)
                float pulse = (float)Math.Sin(time * 3 + i * 0.5) * 0.5f + 0.5f;
                float rayLength = 60f + pulse * 60f;
                
                // Угол с небольшим вращением
                float angle = (float)(time * 0.5 + baseAngle);
                
                // Позиция конца луча
                float rx = x + (float)Math.Cos(angle) * rayLength;
                float ry = y + (float)Math.Sin(angle) * rayLength;
                
                // Позиция начала луча (от центра)
                float startX = x + (float)Math.Cos(angle) * 40;
                float startY = y + (float)Math.Sin(angle) * 40;
                
                // Рисуем луч как линию из сегментов
                int segments = (int)(rayLength / 10);
                for (int s = 0; s < segments; s++)
                {
                    float segX = startX + (float)Math.Cos(angle) * (s * 10);
                    float segY = startY + (float)Math.Sin(angle) * (s * 10);
                    
                    // Луч сужается к концу и становится прозрачнее
                    float alpha = 1 - (float)s / segments;
                    int size = Math.Max(2, 12 - s);
                    
                    Color rayColor = new Color(255, 255, 0, (int)(alpha * 200));
                    DrawRect((int)segX - size/2, (int)segY - size/2, size, size, rayColor);
                }
                
                // Яркая точка на конце луча (вспышка энергии)
                if (pulse > 0.7f)
                {
                    DrawCircle((int)rx, (int)ry, 5, new Color(255, 255, 200, 180));
                }
            }
            
            // Центр солнца (яркое ядро)
            DrawCircle(x, y, 40, new Color(255, 220, 0));
            
            // Внутреннее свечение ядра
            DrawCircle(x, y, 25, new Color(255, 255, 100));
            
            // Самое яркое ядро
            DrawCircle(x, y, 12, new Color(255, 255, 200));
            
            // Внешний ореол
            for (int h = 0; h < 3; h++)
            {
                int haloRadius = 45 + h * 8;
                float haloAlpha = 0.3f - h * 0.1f;
                DrawCircle(x, y, haloRadius, new Color(255, 200, 0, (int)(haloAlpha * 255)));
            }
        }
        
        private void DrawRect(int x, int y, int w, int h, Color c) =>
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, w, h), c);
        
        private void DrawCircle(int x, int y, int r, Color c)
        {
            for (int dy = -r; dy <= r; dy += 3)
            {
                int w = (int)(2 * Math.Sqrt(Math.Max(0, r * r - dy * dy)));
                _spriteBatch.Draw(_pixel, new Rectangle(x - w/2, y + dy, w, 3), c);
            }
        }
        
        private void DrawText(string text, int x, int y, Color c, float scale)
        {
            int cx = x;
            foreach (char ch in text.ToUpper())
            {
                if (ch == ' ') { cx += (int)(14 * scale); continue; }
                DrawChar(ch, cx, y, c, scale);
                cx += (int)(14 * scale);
            }
        }
        
        private void DrawChar(char ch, int x, int y, Color c, float scale)
        {
            // Улучшенный битмап шрифт 7x9 для русских букв
            int[] bitmap = GetCharBitmap(ch);
            if (bitmap == null) return;
            
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if ((bitmap[row] & (1 << (6 - col))) != 0)
                    {
                        _spriteBatch.Draw(_pixel, 
                            new Rectangle(x + (int)(col * scale), y + (int)(row * scale), 
                                (int)(scale + 0.5f), (int)(scale + 0.5f)), c);
                    }
                }
            }
        }
        
        private int[] GetCharBitmap(char ch)
        {
            // Улучшенный шрифт 7x9
            switch (ch)
            {
                case 'А': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'Б': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111110 };
                case 'В': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111110 };
                case 'Г': return new[] { 0b1111111, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'Д': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111111 };
                case 'Е': return new[] { 0b1111111, 0b1000000, 0b1000000, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1111111 };
                case 'Ё': return new[] { 0b1000001, 0b0000000, 0b1111111, 0b1000000, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1111111 };
                case 'Ж': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'З': return new[] { 0b0111110, 0b1000001, 0b0000001, 0b0000001, 0b0011110, 0b0000001, 0b0000001, 0b1000001, 0b0111110 };
                case 'И': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1010101, 0b1010101, 0b1101011, 0b1000001, 0b1000001, 0b1000001 };
                case 'Й': return new[] { 0b1000001, 0b0101010, 0b1000001, 0b1010101, 0b1010101, 0b1101011, 0b1000001, 0b1000001, 0b1000001 };
                case 'К': return new[] { 0b1000001, 0b1000010, 0b1000100, 0b1111000, 0b1000100, 0b1000010, 0b1000001, 0b1000001, 0b1000001 };
                case 'Л': return new[] { 0b0001110, 0b0010001, 0b0100001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111111 };
                case 'М': return new[] { 0b1000001, 0b1101011, 0b1010101, 0b1010101, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'Н': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'О': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case 'П': return new[] { 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'Р': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'С': return new[] { 0b0011100, 0b0100010, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b0100010, 0b0011100 };
                case 'Т': return new[] { 0b1111111, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000 };
                case 'У': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b0111110, 0b0000001, 0b0000001, 0b0000001, 0b0000001, 0b0111110 };
                case 'Ф': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1010101, 0b1111111, 0b1010101, 0b1000001, 0b0100010, 0b0011100 };
                case 'Х': return new[] { 0b1000001, 0b1000001, 0b0100010, 0b0100010, 0b0010100, 0b0100010, 0b0100010, 0b1000001, 0b1000001 };
                case 'Ц': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1111111, 0b0000001, 0b0000001, 0b0000001, 0b0000001, 0b0000001 };
                case 'Ч': return new[] { 0b0000001, 0b0000001, 0b0000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'Ш': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1010101, 0b1010101, 0b1111111, 0b1000001, 0b1000001, 0b1000001 };
                case 'Щ': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1010101, 0b1010101, 0b1111111, 0b1000001, 0b1000001, 0b0000001 };
                case 'Ъ': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'Ы': return new[] { 0b1000001, 0b1000001, 0b1010101, 0b1010101, 0b1110111, 0b1000001, 0b1000001, 0b1000001, 0b1111110 };
                case 'Ь': return new[] { 0b1000000, 0b1000000, 0b1000000, 0b1111110, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111110 };
                case 'Э': return new[] { 0b0011100, 0b0100010, 0b0000001, 0b0001110, 0b0000001, 0b0000001, 0b0100010, 0b1000001, 0b0011100 };
                case 'Ю': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'Я': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1111110, 0b1010000, 0b1001000, 0b1000100, 0b1000010, 0b1000001 };
                // Латиница
                case 'A': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'B': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1111110 };
                case 'C': return new[] { 0b0011100, 0b0100010, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b0100010, 0b0011100 };
                case 'D': return new[] { 0b1111100, 0b1000010, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000010, 0b1111100 };
                case 'E': return new[] { 0b1111111, 0b1000000, 0b1000000, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1111111 };
                case 'F': return new[] { 0b1111111, 0b1000000, 0b1000000, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'G': return new[] { 0b0011100, 0b0100010, 0b1000000, 0b1001111, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case 'H': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1111111, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'I': return new[] { 0b0011100, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0011100 };
                case 'J': return new[] { 0b0000001, 0b0000001, 0b0000001, 0b0000001, 0b0000001, 0b0000001, 0b1000001, 0b1000001, 0b0111110 };
                case 'K': return new[] { 0b1000001, 0b1000010, 0b1000100, 0b1111000, 0b1000100, 0b1000010, 0b1000001, 0b1000001, 0b1000001 };
                case 'L': return new[] { 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1111111 };
                case 'M': return new[] { 0b1000001, 0b1101011, 0b1010101, 0b1010101, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001 };
                case 'N': return new[] { 0b1000001, 0b1100001, 0b1010001, 0b1001001, 0b1000101, 0b1000011, 0b1000001, 0b1000001, 0b1000001 };
                case 'O': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case 'P': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1000000, 0b1000000, 0b1000000, 0b1000000, 0b1000000 };
                case 'Q': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1010101, 0b1001001, 0b0100010, 0b0011100, 0b0000001 };
                case 'R': return new[] { 0b1111110, 0b1000001, 0b1000001, 0b1111110, 0b1010000, 0b1001000, 0b1000100, 0b1000010, 0b1000001 };
                case 'S': return new[] { 0b0011100, 0b0100010, 0b1000000, 0b0111100, 0b0000001, 0b0000001, 0b1000001, 0b0100010, 0b0011100 };
                case 'T': return new[] { 0b1111111, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000 };
                case 'U': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case 'V': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0100010, 0b0010100, 0b0001000 };
                case 'W': return new[] { 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1010101, 0b1010101, 0b1101011, 0b1000001, 0b1000001 };
                case 'X': return new[] { 0b1000001, 0b1000001, 0b0100010, 0b0100010, 0b0010100, 0b0100010, 0b0100010, 0b1000001, 0b1000001 };
                case 'Y': return new[] { 0b1000001, 0b1000001, 0b0100010, 0b0100010, 0b0010100, 0b0001000, 0b0001000, 0b0001000, 0b0001000 };
                case 'Z': return new[] { 0b1111111, 0b0000001, 0b0000010, 0b0000100, 0b0001000, 0b0010000, 0b0100000, 0b1000000, 0b1111111 };
                // Цифры
                case '0': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case '1': return new[] { 0b0001000, 0b0011000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0011100 };
                case '2': return new[] { 0b0111110, 0b1000001, 0b0000001, 0b0000010, 0b0000100, 0b0001000, 0b0010000, 0b0100000, 0b1111111 };
                case '3': return new[] { 0b0111110, 0b1000001, 0b0000001, 0b0000001, 0b0011110, 0b0000001, 0b0000001, 0b1000001, 0b0111110 };
                case '4': return new[] { 0b0000100, 0b0001100, 0b0010100, 0b0100100, 0b1000100, 0b1111111, 0b0000100, 0b0000100, 0b0000100 };
                case '5': return new[] { 0b1111111, 0b1000000, 0b1000000, 0b1111110, 0b0000001, 0b0000001, 0b0000001, 0b1000001, 0b0111110 };
                case '6': return new[] { 0b0011100, 0b0100000, 0b1000000, 0b1111110, 0b1000001, 0b1000001, 0b1000001, 0b1000001, 0b0011100 };
                case '7': return new[] { 0b1111111, 0b0000001, 0b0000010, 0b0000100, 0b0001000, 0b0010000, 0b0010000, 0b0010000, 0b0010000 };
                case '8': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b0111100, 0b1000001, 0b1000001, 0b0100010, 0b0011100 };
                case '9': return new[] { 0b0011100, 0b0100010, 0b1000001, 0b1000001, 0b0111111, 0b0000001, 0b0000001, 0b0000010, 0b0111100 };
                // Символы
                case ':': return new[] { 0b0000000, 0b0000000, 0b0001000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0001000, 0b0000000 };
                case '!': return new[] { 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0000000, 0b0000000, 0b0001000, 0b0000000 };
                case '-': return new[] { 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b1111111, 0b0000000, 0b0000000, 0b0000000, 0b0000000 };
                case '|': return new[] { 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000, 0b0001000 };
                case '.': return new[] { 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0001000 };
                case ',': return new[] { 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0001000, 0b0010000 };
                case '(': return new[] { 0b0000100, 0b0001000, 0b0010000, 0b0100000, 0b0100000, 0b0100000, 0b0010000, 0b0001000, 0b0000100 };
                case ')': return new[] { 0b0100000, 0b0010000, 0b0001000, 0b0000100, 0b0000100, 0b0000100, 0b0001000, 0b0010000, 0b0100000 };
                case '/': return new[] { 0b0000001, 0b0000010, 0b0000100, 0b0001000, 0b0010000, 0b0100000, 0b1000000, 0b1000000, 0b1000000 };
                case '+': return new[] { 0b0000000, 0b0000000, 0b0001000, 0b0001000, 0b1111111, 0b0001000, 0b0001000, 0b0000000, 0b0000000 };
                case '=': return new[] { 0b0000000, 0b0000000, 0b1111111, 0b0000000, 0b0000000, 0b1111111, 0b0000000, 0b0000000, 0b0000000 };
                case '*': return new[] { 0b0010100, 0b0101010, 0b0010100, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000 };
                case '#': return new[] { 0b0010100, 0b1111111, 0b0010100, 0b1111111, 0b0010100, 0b0000000, 0b0000000, 0b0000000, 0b0000000 };
                case '&': return new[] { 0b0011000, 0b0100100, 0b0100100, 0b0011000, 0b0101000, 0b0100100, 0b0011001, 0b0000000, 0b0000000 };
                case '@': return new[] { 0b0011100, 0b0100010, 0b1001111, 0b1010101, 0b1011101, 0b1000000, 0b0011100, 0b0000000, 0b0000000 };
                case '%': return new[] { 0b1100000, 0b1100001, 0b0000010, 0b0000100, 0b0001000, 0b0010000, 0b0100011, 0b0000011, 0b0000000 };
                case '$': return new[] { 0b0001000, 0b0011100, 0b0101000, 0b0111100, 0b0001001, 0b0101000, 0b0011100, 0b0001000, 0b0000000 };
                case '?': return new[] { 0b0011100, 0b0100010, 0b0000001, 0b0000010, 0b0000100, 0b0000000, 0b0000100, 0b0000000, 0b0000000 };
                case '"': return new[] { 0b0100010, 0b0100010, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000 };
                case '\'': return new[] { 0b0001000, 0b0001000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000, 0b0000000 };
                case '[': return new[] { 0b0011100, 0b0100000, 0b0100000, 0b0100000, 0b0100000, 0b0100000, 0b0100000, 0b0100000, 0b0011100 };
                case ']': return new[] { 0b0011100, 0b0000010, 0b0000010, 0b0000010, 0b0000010, 0b0000010, 0b0000010, 0b0000010, 0b0011100 };
                default: return null;
            }
        }
    }
    
    public class Platform { public int X, Y, W, H; public bool IsRiver; }
    public class Palm { public int X, Y; }
    public class Item { public int X, Y; public bool IsCoconut, IsStrawberry, Collected; }
    public class Enemy { public float X, Y; public int Type; public float Dir = 1, DirTimer; }
    public class Mountain { public int X, Y, Height = 80, Width = 100; }
    public class Volcano { public float X, Y; }
    public class Seagull { public float X, Y, Speed = 50, Timer; }
    public class House { public int X, Y; public Color Color; }
    public class Smoke { public float X, Y, Size = 10, Life; }
    public class BloodDrop { public float X, Y, VX, VY, Size, Life; }
    public class JungleHill { public int X, Y, Height, Width; }
    public class Bush { public int X, Y; }
    public class Cloud { public float X, Y, Speed, Scale; }
    public class TreasureChest { public int X, Y; public bool Opened; public bool HasShovel; }
    public class Key { public int X, Y; public bool Collected; }
    public class BackgroundHill { public int X, Y, Height, Width; }
    public class Fog { public float X, Y, Width, Height; }
    public class Rock { public int X, Y, Size, Type; public bool Collected; } // Type: 0=круглый, 1=овальный, 2=угловатый
    public class Diamond { public int X, Y, Size; public bool Collected; }
    public class DugHole { public float X, Y, Life, Size; } // Выкопанная яма
    public class Shovel { public float X, Y, Rotation; } // Лопата для отрисовки
    public class DirtParticle { public float X, Y, VX, VY, Size, Life, Rotation; } // Частица земли
    public class DirtBlock { public float X, Y, Size; public bool Collected; } // Блок грунта
}
