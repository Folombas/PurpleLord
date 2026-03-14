using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLord.Core;

namespace PurpleLord.Entities
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        
        private List<Platform> _platforms;
        
        // Параметры
        private float _moveSpeed = 280f;
        private float _jumpForce = -520f;
        private float _gravity = 1400f;
        
        // Состояние
        private bool _isGrounded = false;
        private int _jumpCount = 0;
        private int _maxJumps = 2;
        
        // Анимация
        private float _walkAnimationTimer = 0f;
        private bool _facingRight = true;
        private float _bounceOffset = 0f;
        
        // Размеры (шарообразный)
        public int Radius = 28;
        public int Width => Radius * 2;
        public int Height => Radius * 2;
        
        public Rectangle Bounds => new Rectangle(
            (int)Position.X - Radius,
            (int)Position.Y - Radius * 2,
            Width,
            Height
        );
        
        public Player(Vector2 position)
        {
            Position = position;
        }
        
        public void Update(GameTime gameTime, List<Platform> platforms)
        {
            _platforms = platforms;
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
            
            // Горизонтальная скорость
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
            if (Position.X < Radius) Position = new Vector2(Radius, Position.Y);
            if (Position.X > 3000 - Radius) Position = new Vector2(3000 - Radius, Position.Y);
            if (Position.Y > 1000)
            {
                Position = new Vector2(150, 520);
                Velocity = Vector2.Zero;
                _jumpCount = 0;
            }
            
            // === АНИМАЦИЯ ===
            if (moveX != 0 && _isGrounded)
            {
                _walkAnimationTimer += deltaTime * 10f;
                _bounceOffset = (float)Math.Sin(_walkAnimationTimer) * 3f;
            }
            else
            {
                _walkAnimationTimer = 0f;
                _bounceOffset = 0f;
            }
            
            _previousKeyboardState = keyboardState;
        }
        
        private KeyboardState _previousKeyboardState;
        
        private void CheckCollisions()
        {
            _isGrounded = false;
            Rectangle playerBounds = Bounds;
            
            foreach (var platform in _platforms)
            {
                if (playerBounds.Intersects(platform.Bounds))
                {
                    float overlapLeft = platform.Bounds.Right - playerBounds.Left;
                    float overlapRight = playerBounds.Right - platform.Bounds.Left;
                    float overlapTop = platform.Bounds.Bottom - playerBounds.Top;
                    float overlapBottom = playerBounds.Bottom - platform.Bounds.Top;
                    
                    float minOverlap = Math.Min(
                        Math.Min(overlapLeft, overlapRight),
                        Math.Min(overlapTop, overlapBottom)
                    );
                    
                    if (minOverlap == overlapTop && Velocity.Y >= 0)
                    {
                        Position = new Vector2(Position.X, platform.Bounds.Top - Height + 10);
                        Velocity = new Vector2(Velocity.X, 0);
                        _isGrounded = true;
                        _jumpCount = 0;
                    }
                    else if (minOverlap == overlapBottom && Velocity.Y < 0)
                    {
                        Position = new Vector2(Position.X, platform.Bounds.Bottom);
                        Velocity = new Vector2(Velocity.X, 0);
                    }
                    else if (minOverlap == overlapLeft && Velocity.X > 0)
                    {
                        Position = new Vector2(platform.Bounds.Right, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                    else if (minOverlap == overlapRight && Velocity.X < 0)
                    {
                        Position = new Vector2(platform.Bounds.Left - Width, Position.Y);
                        Velocity = new Vector2(0, Velocity.Y);
                    }
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int x = (int)Position.X;
            int y = (int)Position.Y + (int)_bounceOffset;
            
            // Цвета
            Color bodyColor = new Color(138, 43, 226);
            Color bodyDark = new Color(100, 30, 180);
            Color skinColor = new Color(255, 220, 180);
            Color eyeColor = Color.White;
            Color pupilColor = Color.Black;
            Color auraColor = new Color(180, 100, 255, 80);
            
            // === АУРА (свечение вокруг) ===
            spriteBatch.Draw(
                new Rectangle(x - Radius - 5, y - Radius * 2 - 5, Width + 10, Height + 10),
                auraColor
            );
            
            // === ТЕЛО (шарообразное) ===
            // Рисуем круг через множество прямоугольников
            for (int dy = -Radius; dy <= Radius; dy += 4)
            {
                int rowWidth = (int)(2 * Math.Sqrt(Radius * Radius - dy * dy));
                int xOffset = rowWidth / 2;
                
                // Градиент тела
                Color rowColor = dy < 0 ? bodyColor : bodyDark;
                
                spriteBatch.Draw(
                    new Rectangle(x - xOffset, y + dy - Radius, rowWidth, 4),
                    rowColor
                );
            }
            
            // === ЛИЦО (на правой стороне шара) ===
            int faceX = _facingRight ? x + 8 : x - 20;
            int faceY = y - 10;
            
            // Белки глаз
            spriteBatch.Draw(new Rectangle(faceX, faceY, 10, 10), eyeColor);
            spriteBatch.Draw(new Rectangle(faceX + 12, faceY, 10, 10), eyeColor);
            
            // Зрачки
            int pupilOffset = _facingRight ? 3 : -3;
            spriteBatch.Draw(new Rectangle(faceX + 2 + pupilOffset, faceY + 2, 4, 4), pupilColor);
            spriteBatch.Draw(new Rectangle(faceX + 14 + pupilOffset, faceY + 2, 4, 4), pupilColor);
            
            // Капюшон (сверху)
            for (int i = 0; i < 8; i++)
            {
                int hoodWidth = Radius * 2 - i * 2;
                spriteBatch.Draw(
                    new Rectangle(x - hoodWidth / 2, y - Radius * 2 - i, hoodWidth, 2),
                    bodyDark
                );
            }
            
            // === НОГИ (анимация) ===
            if (_isGrounded)
            {
                float legOffset = (float)Math.Sin(_walkAnimationTimer) * 8f;
                spriteBatch.Draw(new Rectangle(x - 12, y + Radius - 5, 8, 12 + (int)legOffset), bodyDark);
                spriteBatch.Draw(new Rectangle(x + 4, y + Radius - 5, 8, 12 - (int)legOffset), bodyDark);
            }
            else
            {
                // В прыжке ноги поджаты
                spriteBatch.Draw(new Rectangle(x - 10, y + Radius - 8, 6, 8), bodyDark);
                spriteBatch.Draw(new Rectangle(x + 4, y + Radius - 8, 6, 8), bodyDark);
            }
        }
    }
}
