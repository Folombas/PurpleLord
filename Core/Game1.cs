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
        private List<Rectangle> _platforms;
        
        // Декорации
        private List<Cloud> _clouds;
        private List<Seagull> _seagulls;
        private List<PalmTree> _palmTrees;
        private List<House> _houses;
        private List<SmokeParticle> _smokeParticles;
        
        // Солнце
        private Vector2 _sunPosition = new Vector2(100, 80);
        
        // Цвета
        private Color _skyColor = new Color(135, 206, 235);
        private Color _seaColor = new Color(30, 144, 255);
        private Color _mountainColor = new Color(100, 100, 150);
        private Color _grassColor = new Color(34, 139, 34);
        private Color _dirtColor = new Color(139, 90, 43);
        private Color _sunColor = new Color(255, 255, 0);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            Window.Title = "Purple Lord - Path of Choices";
        }

        protected override void Initialize()
        {
            _camera = new Camera2D(GraphicsDevice.Viewport);

            // === ПЛАТФОРМЫ (земля с травой) ===
            _platforms = new List<Rectangle>
            {
                // Основная земля (длинная)
                new Rectangle(0, 600, 800, 120),
                
                // Платформы в воздухе
                new Rectangle(200, 500, 120, 25),
                new Rectangle(400, 430, 150, 25),
                new Rectangle(600, 350, 120, 25),
                
                // Вторая земля справа
                new Rectangle(900, 600, 600, 120),
                
                // Платформы справа
                new Rectangle(1000, 500, 130, 25),
                new Rectangle(1200, 420, 140, 25),
                new Rectangle(1400, 340, 100, 25),
            };

            // Игрок НА земле, не в воздухе!
            _player = new Player(new Vector2(100, 520), _platforms);

            // === ОБЛАКА ===
            _clouds = new List<Cloud>();
            var random = new Random();
            for (int i = 0; i < 6; i++)
            {
                _clouds.Add(new Cloud(
                    new Vector2(random.Next(0, 1800), random.Next(30, 200)),
                    0.5f + random.NextSingle() * 0.5f
                ));
            }

            // === ЧАЙКИ ===
            _seagulls = new List<Seagull>();
            for (int i = 0; i < 4; i++)
            {
                _seagulls.Add(new Seagull(
                    new Vector2(random.Next(300, 1500), random.Next(80, 250)),
                    30f + random.NextSingle() * 30f
                ));
            }

            // === ПАЛЬМЫ ===
            _palmTrees = new List<PalmTree>
            {
                new PalmTree(new Vector2(50, 500)),
                new PalmTree(new Vector2(1350, 500)),
            };

            // === ДОМИКИ ===
            _houses = new List<House>
            {
                new House(new Vector2(700, 540), new Color(200, 180, 150)),
                new House(new Vector2(1100, 540), new Color(180, 160, 140)),
                new House(new Vector2(1300, 540), new Color(190, 170, 150)),
            };

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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);

            // Камера следует за игроком
            float cameraX = Math.Max(0, _player.Position.X - 500);
            _camera.Position = new Vector2(cameraX, 0);

            // Обновление декораций
            foreach (var cloud in _clouds)
                cloud.Update(delta);
            
            foreach (var seagull in _seagulls)
                seagull.Update(delta);

            // Обновление дыма
            UpdateSmoke(delta);

            base.Update(gameTime);
        }

        private void UpdateSmoke(float delta)
        {
            // Добавляем дым из труб домов
            foreach (var house in _houses)
            {
                if (random.NextSingle() < 0.3f)
                {
                    _smokeParticles.Add(new SmokeParticle(
                        new Vector2(house.Position.X + house.Width / 2 + random.Next(-5, 5), house.Position.Y - 20)
                    ));
                }
            }

            // Обновляем частицы
            for (int i = _smokeParticles.Count - 1; i >= 0; i--)
            {
                _smokeParticles[i].Update(delta);
                if (_smokeParticles[i].Life <= 0)
                    _smokeParticles.RemoveAt(i);
            }
        }

        private Random random = new Random();

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_skyColor);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            // === ЗАДНИЙ ПЛАН ===
            
            // Солнце (жёлтый круг)
            DrawSun(_sunPosition);

            // Море на горизонте
            _spriteBatch.Draw(
                new Rectangle(1600, 560, 800, 60),
                _seaColor
            );

            // Горы вдалеке
            DrawMountains(1400, 560);
            DrawMountains(1700, 560);

            // === ОБЛАКА ===
            foreach (var cloud in _clouds)
                cloud.Draw(_spriteBatch);

            // === ЧАЙКИ ===
            foreach (var seagull in _seagulls)
                seagull.Draw(_spriteBatch);

            // === ДОМИКИ ===
            foreach (var house in _houses)
                house.Draw(_spriteBatch);

            // === ДЫМ ===
            foreach (var smoke in _smokeParticles)
                smoke.Draw(_spriteBatch);

            // === ПАЛЬМЫ ===
            foreach (var palm in _palmTrees)
                palm.Draw(_spriteBatch);

            // === ПЛАТФОРМЫ (земля с травой) ===
            foreach (var platform in _platforms)
            {
                // Трава (верхний слой, 8 пикселей)
                _spriteBatch.Draw(
                    new Rectangle(platform.X, platform.Y, platform.Width, 8),
                    _grassColor
                );
                
                // Земля (нижний слой)
                _spriteBatch.Draw(
                    new Rectangle(platform.X, platform.Y + 8, platform.Width, platform.Height - 8),
                    _dirtColor
                );

                // Детали травы (маленькие кустики сверху)
                for (int i = 0; i < platform.Width; i += 12)
                {
                    _spriteBatch.Draw(
                        new Rectangle(platform.X + i, platform.Y - 4, 3, 8),
                        new Color(0, 180, 0)
                    );
                }
            }

            // === ИГРОК ===
            _player.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSun(Vector2 position)
        {
            int radius = 35;
            
            // Рисуем круг солнца через множество маленьких прямоугольников
            for (int angle = 0; angle < 360; angle += 5)
            {
                float rad = angle * (float)Math.PI / 180f;
                float x = position.X + (float)Math.Cos(rad) * radius;
                float y = position.Y + (float)Math.Sin(rad) * radius;
                
                // Лучи солнца
                _spriteBatch.Draw(
                    new Rectangle((int)x, (int)y, 3, 3),
                    new Color(255, 255, 200)
                );
            }
            
            // Центр солнца (полный круг)
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        _spriteBatch.Draw(
                            new Rectangle((int)position.X + x, (int)position.Y + y, 2, 2),
                            _sunColor
                        );
                    }
                }
            }
        }

        private void DrawMountains(int startX, int baseY)
        {
            for (int i = 0; i < 4; i++)
            {
                int peakX = startX + i * 120;
                int peakHeight = 60 + (i % 2) * 40;
                
                // Рисуем треугольную гору
                int width = 100;
                for (int y = 0; y < peakHeight; y++)
                {
                    int rowWidth = width * (peakHeight - y) / peakHeight;
                    _spriteBatch.Draw(
                        new Rectangle(peakX - rowWidth / 2, baseY - y, rowWidth, 2),
                        _mountainColor
                    );
                }
            }
        }
    }

    // === Классы декораций ===

    public class Cloud
    {
        public Vector2 Position;
        public float Speed;
        public float Scale;

        public Cloud(Vector2 position, float scale)
        {
            Position = position;
            Scale = scale;
            Speed = 15f + scale * 10f;
        }

        public void Update(float delta)
        {
            Position.X += Speed * delta;
            if (Position.X > 2200)
                Position.X = -200;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color cloudColor = new Color(255, 255, 255, 220);
            
            // Три овала для облака
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 70, 35), cloudColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 30, (int)Position.Y - 20, 55, 45), cloudColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 70, (int)Position.Y, 65, 35), cloudColor);
        }
    }

    public class Seagull
    {
        public Vector2 Position;
        public float Speed;
        private float _wingAngle;
        private float _startX;

        public Seagull(Vector2 position, float speed)
        {
            Position = position;
            _startX = position.X;
            Speed = speed;
        }

        public void Update(float delta)
        {
            _wingAngle += 12f * delta;
            
            // Чайка летает туда-сюда
            Position.X += (float)Math.Sin(_wingAngle * 0.5f) * Speed * delta;
            
            // Ограничиваем полёт
            if (Position.X < _startX - 200) Position.X = _startX - 200;
            if (Position.X > _startX + 200) Position.X = _startX + 200;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color white = Color.White;
            float wingOffset = (float)Math.Sin(_wingAngle) * 6f;
            
            // Тело
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 14, 3), white);
            
            // Крылья (V-образно)
            spriteBatch.Draw(new Rectangle((int)Position.X + 2, (int)Position.Y - 5 + (int)wingOffset, 10, 2), white);
        }
    }

    public class PalmTree
    {
        public Vector2 Position;
        public float Height = 90f;

        public PalmTree(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color trunkColor = new Color(101, 67, 33);
            Color leafColor = new Color(0, 140, 0);

            // Ствол (немного изогнутый)
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, 14, (int)Height), trunkColor);

            // Листья веером
            float topX = Position.X + 7;
            float topY = Position.Y;

            for (int i = 0; i < 7; i++)
            {
                float angle = 1.57f + (i - 3) * 0.35f;
                float length = 40f + (i % 3) * 10f;
                
                float endX = topX + (float)Math.Cos(angle) * length;
                float endY = topY - (float)Math.Sin(angle) * length;
                
                // Рисуем лист как линию из прямоугольников
                int steps = (int)(length / 8);
                for (int s = 0; s < steps; s++)
                {
                    float sx = topX + (float)Math.Cos(angle) * (s * 8);
                    float sy = topY - (float)Math.Sin(angle) * (s * 8);
                    int size = 6 - s / 2;
                    if (size < 2) size = 2;
                    spriteBatch.Draw(new Rectangle((int)sx, (int)sy, size, size), leafColor);
                }
            }
        }
    }

    public class House
    {
        public Vector2 Position;
        public int Width = 70;
        public int Height = 50;
        public Color WallColor;

        public House(Vector2 position, Color wallColor)
        {
            Position = position;
            WallColor = wallColor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color roofColor = new Color(139, 69, 19);
            Color windowColor = new Color(100, 150, 200);
            Color doorColor = new Color(101, 67, 33);

            // Стены
            spriteBatch.Draw(new Rectangle((int)Position.X, (int)Position.Y, Width, Height), WallColor);

            // Крыша (треугольная)
            DrawRoof(spriteBatch, Position.X, Position.Y, Width, roofColor);

            // Окно
            spriteBatch.Draw(new Rectangle((int)Position.X + 15, (int)Position.Y + 15, 15, 15), windowColor);
            spriteBatch.Draw(new Rectangle((int)Position.X + 40, (int)Position.Y + 15, 15, 15), windowColor);

            // Дверь
            spriteBatch.Draw(new Rectangle((int)Position.X + 27, (int)Position.Y + 30, 16, 20), doorColor);

            // Труба
            spriteBatch.Draw(new Rectangle((int)Position.X + 50, (int)Position.Y - 20, 8, 20), new Color(150, 75, 30));
        }

        private void DrawRoof(SpriteBatch spriteBatch, float x, float y, int width, Color color)
        {
            int roofHeight = 35;
            for (int row = 0; row < roofHeight; row++)
            {
                int rowWidth = width + 10 - (row * 2);
                int xOffset = row;
                spriteBatch.Draw(
                    new Rectangle((int)x - xOffset, (int)y - row, rowWidth, 2),
                    color
                );
            }
        }
    }

    public class SmokeParticle
    {
        public Vector2 Position;
        public float Life = 2f;
        public float MaxLife;
        public Vector2 Velocity;

        public SmokeParticle(Vector2 position)
        {
            Position = position;
            MaxLife = Life;
            Velocity = new Vector2(0, -15f);
        }

        public void Update(float delta)
        {
            Position += Velocity * delta;
            Velocity.X += (float)(new Random().NextDouble() - 0.5) * 10f * delta;
            Life -= delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float alpha = Life / MaxLife;
            Color smokeColor = new Color(200, 200, 200, (int)(alpha * 150));
            int size = (int)(8 + (1 - alpha) * 12);
            
            spriteBatch.Draw(
                new Rectangle((int)Position.X, (int)Position.Y, size, size),
                smokeColor
            );
        }
    }
}
