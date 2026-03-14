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
        
        private float _moveSpeed = 250f;
        private float _jumpForce = -500f;
        private float _gravity = 1200f;
        private bool _isGrounded = false;
        private int _jumpCount = 0;
        private int _maxJumps = 2;
        
        private float _walkAnimationTimer = 0f;
        private bool _facingRight = true;
        
        public Player(Vector2 position, List<Rectangle> platforms) : base(position)
        {
            _platforms = platforms;
            Width = 36;
            Height = 56;
        }
        
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            
            // === ДВИЖЕНИЕ ===
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
            
            // Применяем горизонтальную скорость
            Velocity = new Vector2(moveX * _moveSpeed, Velocity.Y);
            
            // Гравитация
            Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);
            
            // === ПРЫЖОК ===
            bool jumpPressed = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W);
            bool jumpJustPressed = jumpPressed && 
                !_previousKeyboardState.IsKeyDown(Keys.Space) && 
                !_previousKeyboardState.IsKeyDown(Keys.W);
            
            if (jumpJustPressed)
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
            
            // === ПРИМЕНЯЕМ СКОРОСТЬ ===
            Position = new Vector2(Position.X + Velocity.X * deltaTime, Position.Y + Velocity.Y * deltaTime);
            
            // === КОЛЛИЗИИ ===
            CheckCollisions();
            
            // === ГРАНИЦЫ МИРА ===
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.Y > 1000)
            {
                Position = new Vector2(100, 520);
                Velocity = Vector2.Zero;
                _jumpCount = 0;
            }
            
            // === АНИМАЦИЯ ===
            if (moveX != 0 && _isGrounded)
                _walkAnimationTimer += deltaTime * 8f;
            else
                _walkAnimationTimer = 0f;
            
            _previousKeyboardState = keyboardState;
        }
        
        private void CheckCollisions()
        {
            _isGrounded = false;
            
            Rectangle playerBounds = Bounds;
            
            foreach (var platform in _platforms)
            {
                if (playerBounds.Intersects(platform))
                {
                    // Вычисляем перекрытия с каждой стороны
                    float overlapLeft = platform.Right - playerBounds.Left;
                    float overlapRight = playerBounds.Right - platform.Left;
                    float overlapTop = platform.Bottom - playerBounds.Top;
                    float overlapBottom = playerBounds.Bottom - platform.Top;
                    
                    // Находим минимальное перекрытие
                    float minOverlap = Math.Min(
                        Math.Min(overlapLeft, overlapRight),
                        Math.Min(overlapTop, overlapBottom)
                    );
                    
                    // Определяем направление коллизии и исправляем позицию
                    if (minOverlap == overlapTop && Velocity.Y >= 0)
                    {
                        // Приземление на платформу
                        Position = new Vector2(Position.X, platform.Top - Height);
                        Velocity = new Vector2(Velocity.X, 0);
                        _isGrounded = true;
                        _jumpCount = 0;
                    }
                    else if (minOverlap == overlapBottom && Velocity.Y < 0)
                    {
                        // Удар головой
                        Position = new Vector2(Position.X, platform.Bottom);
                        Velocity = new Vector2(Velocity.X, 0);
                    }
                    else if (minOverlap == overlapLeft && Velocity.X > 0)
                    {
                        // Столкновение слева
                        Position = new Vector2(platform.Right, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                    else if (minOverlap == overlapRight && Velocity.X < 0)
                    {
                        // Столкновение справа
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
            
            Color bodyColor = new Color(138, 43, 226);
            Color robeColor = new Color(100, 30, 180);
            Color skinColor = new Color(255, 220, 180);
            Color eyeColor = Color.White;
            Color pupilColor = Color.Black;
            
            // Анимация покачивания при ходьбе
            float bounce = (float)Math.Sin(_walkAnimationTimer) * 2f;
            
            // === ТЕЛО (мантия) ===
            spriteBatch.Draw(new Rectangle(x + 6, y + 22 + (int)bounce, 24, 34), robeColor);
            
            // === ГОЛОВА ===
            spriteBatch.Draw(new Rectangle(x + 8, y + 2, 20, 20), skinColor);
            
            // === КАПЮШОН ===
            spriteBatch.Draw(new Rectangle(x + 6, y, 24, 12), robeColor);
            
            // === ГЛАЗА ===
            int eyeOffset = _facingRight ? 2 : -2;
            spriteBatch.Draw(new Rectangle(x + 12 + eyeOffset, y + 8, 5, 5), eyeColor);
            spriteBatch.Draw(new Rectangle(x + 20 + eyeOffset, y + 8, 5, 5), eyeColor);
            
            // Зрачки
            int pupilOffset = _facingRight ? 1 : -1;
            spriteBatch.Draw(new Rectangle(x + 13 + pupilOffset + eyeOffset, y + 9, 2, 2), pupilColor);
            spriteBatch.Draw(new Rectangle(x + 21 + pupilOffset + eyeOffset, y + 9, 2, 2), pupilColor);
            
            // === НОГИ (анимация) ===
            float legOffset = (float)Math.Sin(_walkAnimationTimer) * 6f;
            spriteBatch.Draw(new Rectangle(x + 8, y + 54, 8, 10 + (int)legOffset), robeColor);
            spriteBatch.Draw(new Rectangle(x + 20, y + 54, 8, 10 - (int)legOffset), robeColor);
            
            // === РУКИ ===
            float armOffset = (float)Math.Sin(_walkAnimationTimer) * 4f;
            spriteBatch.Draw(new Rectangle(x + 2, y + 24 + (int)armOffset, 6, 14), robeColor);
            spriteBatch.Draw(new Rectangle(x + 28, y + 24 - (int)armOffset, 6, 14), robeColor);
            
            // === АУРА (свечение) ===
            Color auraColor = new Color(180, 100, 255, 60);
            spriteBatch.Draw(new Rectangle(x - 4, y - 4, Width + 8, Height + 8), auraColor);
        }
    }
}
