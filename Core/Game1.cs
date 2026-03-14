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
        
        // Солнце
        private Vector2 _sunPosition = new Vector2(100, 80);
        
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Выход по Escape
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
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

            // Дым из труб
            UpdateSmoke(delta);

            base.Update(gameTime);
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

            // Горы (на заднем плане)
            DrawMountains(2400, GROUND_Y - 40);
            DrawMountains(2700, GROUND_Y - 40);

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

            // === ПЛАТФОРМЫ ===
            foreach (var platform in _platforms)
                platform.Draw(_spriteBatch);

            // === ИГРОК ===
            _player.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            // === UI (без камеры) ===
            DrawUI();

            base.Draw(gameTime);
        }

        private void DrawSun(Vector2 position)
        {
            int radius = 40;
            
            // Лучи солнца
            for (int i = 0; i < 12; i++)
            {
                float angle = i * MathHelper.Pi / 6;
                float rayX = position.X + (float)Math.Cos(angle) * (radius + 15);
                float rayY = position.Y + (float)Math.Sin(angle) * (radius + 15);
                _spriteBatch.Draw(
                    new Rectangle((int)rayX - 2, (int)rayY - 2, 4, 4),
                    new Color(255, 255, 200)
                );
            }
            
            // Центр солнца (закрашенный круг)
            for (int y = -radius; y <= radius; y += 3)
            {
                for (int x = -radius; x <= radius; x += 3)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        _spriteBatch.Draw(
                            new Rectangle((int)position.X + x, (int)position.Y + y, 3, 3),
                            _sunColor
                        );
                    }
                }
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
}
