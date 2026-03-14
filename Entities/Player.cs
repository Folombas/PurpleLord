using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLord.Core;

namespace PurpleLord.Entities
{
    public class Player : GameObject
    {
        private KeyboardState _previousKeyboardState;
        private List<Rectangle> _platforms;
        
        private float _moveSpeed = 300f;
        private float _jumpForce = -550f;
        private float _gravity = 1500f;
        private bool _isGrounded = false;
        private int _jumpCount = 0;
        private int _maxJumps = 2;
        
        // Анимация
        private float _walkAnimationTimer = 0f;
        private bool _facingRight = true;
        
        public Player(Vector2 position, List<Rectangle> platforms) : base(position)
        {
            _platforms = platforms;
            Width = 40;
            Height = 60;
        }
        
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            
            // Движение
            float moveX = 0;
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                moveX = -1;
                _facingRight = false;
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                moveX = 1;
                _facingRight = true;
            }
            
            Velocity = new Vector2(moveX * _moveSpeed, Velocity.Y);
            Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);
            
            // Прыжок
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) &&
                !_previousKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.W))
            {
                if (_isGrounded)
                {
                    Velocity = new Vector2(Velocity.X, _jumpForce);
                    _jumpCount = 1;
                    _isGrounded = false;
                }
                else if (_jumpCount < _maxJumps)
                {
                    Velocity = new Vector2(Velocity.X, _jumpForce);
                    _jumpCount++;
                }
            }
            
            Position = new Vector2(Position.X + Velocity.X * deltaTime, Position.Y + Velocity.Y * deltaTime);
            CheckCollisions();
            
            // Границы мира
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y > 1000)
            {
                Position = new Vector2(100, 550);
                Velocity = Vector2.Zero;
                _jumpCount = 0;
            }
            
            // Анимация ходьбы
            if (moveX != 0 && _isGrounded)
            {
                _walkAnimationTimer += deltaTime * 10f;
            }
            else
            {
                _walkAnimationTimer = 0f;
            }
            
            _previousKeyboardState = keyboardState;
        }
        
        private void CheckCollisions()
        {
            _isGrounded = false;
            
            foreach (var platform in _platforms)
            {
                if (Bounds.Intersects(platform))
                {
                    float overlapLeft = platform.Right - Bounds.Left;
                    float overlapRight = Bounds.Right - platform.Left;
                    float overlapTop = platform.Bottom - Bounds.Top;
                    float overlapBottom = Bounds.Bottom - platform.Top;
                    
                    float minOverlap = Math.Min(
                        Math.Min(overlapLeft, overlapRight),
                        Math.Min(overlapTop, overlapBottom)
                    );
                    
                    if (minOverlap == overlapTop && Velocity.Y >= 0)
                    {
                        Position = new Vector2(Position.X, platform.Top - Height);
                        Velocity = new Vector2(Velocity.X, 0);
                        _isGrounded = true;
                        _jumpCount = 0;
                    }
                    else if (minOverlap == overlapBottom && Velocity.Y < 0)
                    {
                        Position = new Vector2(Position.X, platform.Bottom);
                        Velocity = new Vector2(Velocity.X, 0);
                    }
                    else if (minOverlap == overlapLeft && Velocity.X > 0)
                    {
                        Position = new Vector2(platform.Right, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                    else if (minOverlap == overlapRight && Velocity.X < 0)
                    {
                        Position = new Vector2(platform.Left - Width, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                }
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int x = (int)Position.X;
            int y = (int)Position.Y;
            
            // Цвет игрока - фиолетовый лорд
            Color bodyColor = new Color(138, 43, 226);
            Color robeColor = new Color(100, 30, 180);
            Color skinColor = new Color(255, 220, 180);
            Color eyeColor = Color.White;
            Color pupilColor = Color.Black;
            
            // Тело (мантия)
            spriteBatch.Draw(new Rectangle(x + 8, y + 25, 24, 35), robeColor);
            
            // Голова
            spriteBatch.Draw(new Rectangle(x + 10, y + 5, 20, 20), skinColor);
            
            // Капюшон
            spriteBatch.Draw(new Rectangle(x + 8, y + 3, 24, 12), robeColor);
            
            // Глаза (направлены в сторону движения)
            int eyeOffset = _facingRight ? 2 : -2;
            spriteBatch.Draw(new Rectangle(x + 14 + eyeOffset, y + 12, 5, 5), eyeColor);
            spriteBatch.Draw(new Rectangle(x + 22 + eyeOffset, y + 12, 5, 5), eyeColor);
            
            // Зрачки
            int pupilOffset = _facingRight ? 1 : -1;
            spriteBatch.Draw(new Rectangle(x + 15 + pupilOffset + eyeOffset, y + 13, 2, 2), pupilColor);
            spriteBatch.Draw(new Rectangle(x + 23 + pupilOffset + eyeOffset, y + 13, 2, 2), pupilColor);
            
            // Ноги (анимация ходьбы)
            float legOffset = (float)Math.Sin(_walkAnimationTimer) * 5f;
            spriteBatch.Draw(new Rectangle(x + 10, y + 58, 8, 10 + (int)legOffset), robeColor);
            spriteBatch.Draw(new Rectangle(x + 22, y + 58, 8, 10 - (int)legOffset), robeColor);
            
            // Руки
            float armOffset = (float)Math.Sin(_walkAnimationTimer) * 3f;
            spriteBatch.Draw(new Rectangle(x + 4, y + 28 + (int)armOffset, 6, 15), robeColor);
            spriteBatch.Draw(new Rectangle(x + 30, y + 28 - (int)armOffset, 6, 15), robeColor);
            
            // Эффект свечения (аура)
            Color auraColor = new Color(180, 100, 255, 80);
            spriteBatch.Draw(new Rectangle(x - 3, y - 3, Width + 6, Height + 6), auraColor);
        }
    }
}
