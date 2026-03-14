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
        
        // Цвета
        private Color _skyColor = new Color(135, 206, 235);
        private Color _seaColor = new Color(30, 144, 255);
        private Color _mountainColor = new Color(100, 100, 150);
        private Color _grassColor = new Color(34, 139, 34);
        private Color _dirtColor = new Color(139, 90, 43);
        private Color _palmTrunkColor = new Color(101, 67, 33);
        private Color _palmLeafColor = new Color(0, 100, 0);

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

            // Создание платформ (земля с травой)
            _platforms = new List<Rectangle>
            {
                new Rectangle(0, 620, 2000, 100),      // Основная земля
                new Rectangle(300, 520, 150, 25),      // Платформа 1
                new Rectangle(550, 450, 180, 25),      // Платформа 2
                new Rectangle(850, 380, 150, 25),      // Платформа 3
                new Rectangle(1100, 500, 200, 25),     // Платформа 4
                new Rectangle(1400, 620, 800, 100),    // Вторая земля
            };

            _player = new Player(new Vector2(100, 550), _platforms);

            // Облака
            _clouds = new List<Cloud>();
            var random = new Random();
            for (int i = 0; i < 8; i++)
            {
                _clouds.Add(new Cloud(
                    new Vector2(random.Next(0, 2000), random.Next(50, 250)),
                    0.3f + random.NextSingle() * 0.3f
                ));
            }

            // Чайки
            _seagulls = new List<Seagull>();
            for (int i = 0; i < 5; i++)
            {
                _seagulls.Add(new Seagull(
                    new Vector2(random.Next(200, 1800), random.Next(100, 300)),
                    1f + random.NextSingle() * 2f
                ));
            }

            // Пальмы
            _palmTrees = new List<PalmTree>
            {
                new PalmTree(new Vector2(150, 520)),
                new PalmTree(new Vector2(1600, 520)),
                new PalmTree(new Vector2(1850, 520)),
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);

            // Камера следует за игроком
            float cameraX = Math.Max(0, _player.Position.X - 640);
            float cameraY = Math.Max(0, Math.Min(_player.Position.Y - 360, 200));
            _camera.Position = new Vector2(cameraX, cameraY);

            // Обновление декораций
            foreach (var cloud in _clouds)
                cloud.Update(gameTime);
            
            foreach (var seagull in _seagulls)
                seagull.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_skyColor);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            // === ЗАДНИЙ ПЛАН ===
            
            // Море на горизонте
            _spriteBatch.Draw(
                new Rectangle(1800, 580, 1000, 40),
                _seaColor
            );

            // Горы вдалеке
            DrawMountains(1600, 580);
            DrawMountains(2000, 580);

            // === ОБЛАКА ===
            foreach (var cloud in _clouds)
                cloud.Draw(_spriteBatch);

            // === ЧАЙКИ ===
            foreach (var seagull in _seagulls)
                seagull.Draw(_spriteBatch);

            // === ПАЛЬМЫ ===
            foreach (var palm in _palmTrees)
                palm.Draw(_spriteBatch);

            // === ПЛАТФОРМЫ (земля с травой) ===
            foreach (var platform in _platforms)
            {
                // Трава (верхний слой)
                _spriteBatch.Draw(
                    new Rectangle(platform.X, platform.Y, platform.Width, 8),
                    _grassColor
                );
                
                // Земля (нижний слой)
                _spriteBatch.Draw(
                    new Rectangle(platform.X, platform.Y + 8, platform.Width, platform.Height - 8),
                    _dirtColor
                );

                // Детали травы (маленькие прямоугольники сверху)
                for (int i = 0; i < platform.Width; i += 15)
                {
                    _spriteBatch.Draw(
                        new Rectangle(platform.X + i, platform.Y - 3, 4, 6),
                        _grassColor
                    );
                }
            }

            // === ИГРОК ===
            _player.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMountains(int startX, int baseY)
        {
            // Рисуем несколько горных пиков
            for (int i = 0; i < 5; i++)
            {
                int peakX = startX + i * 150;
                int peakHeight = 80 + (i % 3) * 30;
                
                // Треугольная гора
                var points = new[]
                {
                    new Vector2(peakX, baseY),
                    new Vector2(peakX + 75, baseY - peakHeight),
                    new Vector2(peakX + 150, baseY)
                };
                
                DrawTriangle(points[0], points[1], points[2], _mountainColor);
            }
        }

        private void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            // Простая отрисовка треугольника через линии
            DrawLine(p1, p2, color, 1);
            DrawLine(p2, p3, color, 1);
            DrawLine(p3, p1, color, 1);
            
            // Заполнение (упрощённое)
            int minY = (int)Math.Min(Math.Min(p1.Y, p2.Y), p3.Y);
            int maxY = (int)Math.Max(Math.Max(p1.Y, p2.Y), p3.Y);
            
            for (int y = minY; y < maxY; y += 2)
            {
                _spriteBatch.Draw(new Rectangle((int)p1.X, y, 150, 2), color);
            }
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            _spriteBatch.Draw(
                GetWhiteTexture(),
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(edge.Length(), thickness),
                SpriteEffects.None,
                0f
            );
        }

        private Texture2D _whiteTexture;
        private Texture2D GetWhiteTexture()
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
                _whiteTexture.SetData(new[] { Color.White });
            }
            return _whiteTexture;
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
            Speed = 10f + scale * 10f;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Position.X > 2500)
                Position.X = -200;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color cloudColor = new Color(255, 255, 255, 200);
            
            // Рисуем несколько кругов для облака
            spriteBatch.Draw(
                new Rectangle((int)Position.X, (int)Position.Y, 60, 30),
                cloudColor
            );
            spriteBatch.Draw(
                new Rectangle((int)Position.X + 25, (int)Position.Y - 15, 50, 40),
                cloudColor
            );
            spriteBatch.Draw(
                new Rectangle((int)Position.X + 60, (int)Position.Y, 55, 30),
                cloudColor
            );
        }
    }

    public class Seagull
    {
        public Vector2 Position;
        public float Speed;
        private float _wingAngle;
        private float _wingSpeed;

        public Seagull(Vector2 position, float speed)
        {
            Position = position;
            Speed = speed;
            _wingSpeed = 8f;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.X -= Speed * delta;
            _wingAngle += _wingSpeed * delta;
            
            if (Position.X < -50)
                Position.X = 2000;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color white = Color.White;
            float wingOffset = (float)Math.Sin(_wingAngle) * 8f;
            
            // Тело чайки (маленькая линия)
            spriteBatch.Draw(
                new Rectangle((int)Position.X, (int)Position.Y, 12, 2),
                white
            );
            
            // Крылья (V-образная форма)
            spriteBatch.Draw(
                new Rectangle((int)Position.X - 3, (int)Position.Y - 4 + (int)wingOffset, 6, 2),
                white
            );
        }
    }

    public class PalmTree
    {
        public Vector2 Position;
        public float Height = 80f;

        public PalmTree(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color trunkColor = new Color(101, 67, 33);
            Color leafColor = new Color(0, 120, 0);

            // Ствол пальмы
            spriteBatch.Draw(
                new Rectangle((int)Position.X, (int)Position.Y, 12, (int)Height),
                trunkColor
            );

            // Листья пальмы (несколько изогнутых линий)
            float topX = Position.X + 6;
            float topY = Position.Y;

            // Рисуем 7 листьев веером
            for (int i = 0; i < 7; i++)
            {
                float angle = MathHelper.PiOver2 + (i - 3) * 0.3f;
                float length = 35f + (i % 3) * 10f;
                
                // Лист (удлинённый прямоугольник)
                spriteBatch.Draw(
                    new Rectangle((int)topX, (int)topY, (int)length, 4),
                    leafColor
                );
            }
        }
    }
}
