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
        
        // Игрок
        private Player _player;
        
        // Камера
        private Camera2D _camera;
        
        // Платформы
        private List<Rectangle> _platforms;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Настройка графики
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            
            Window.Title = "Purple Lord - Platformer";
        }

        protected override void Initialize()
        {
            // Инициализация камеры
            _camera = new Camera2D(GraphicsDevice.Viewport);
            
            // Создание платформ
            _platforms = new List<Rectangle>
            {
                new Rectangle(0, 650, 1280, 50),      // Пол
                new Rectangle(200, 550, 200, 30),     // Платформа 1
                new Rectangle(500, 450, 200, 30),     // Платформа 2
                new Rectangle(800, 350, 200, 30),     // Платформа 3
                new Rectangle(300, 250, 150, 30),     // Платформа 4
            };
            
            // Создание игрока
            _player = new Player(new Vector2(100, 550), _platforms);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обновление игрока
            _player.Update(gameTime);

            // Обновление камеры (следит за игроком, но не уходит в отрицательные значения)
            float cameraX = Math.Max(0, _player.Position.X - 640);
            float cameraY = Math.Max(0, _player.Position.Y - 360);
            _camera.Position = new Vector2(cameraX, cameraY);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(135, 206, 235)); // Небо
            
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            
            // Отрисовка платформ
            foreach (var platform in _platforms)
            {
                _spriteBatch.Draw(platform, Color.DarkGreen);
            }
            
            // Отрисовка игрока
            _player.Draw(_spriteBatch, gameTime);
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
