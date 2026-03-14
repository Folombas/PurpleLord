using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PurpleLord.Entities
{
    public class Player : GameObject
    {
        private KeyboardState _previousKeyboardState;
        private List<Rectangle> _platforms;
        
        private float _moveSpeed = 300f;
        private float _jumpForce = -500f;
        private float _gravity = 1200f;
        private bool _isGrounded = false;
        private int _jumpCount = 0;
        private int _maxJumps = 2;
        
        public Player(Vector2 position, List<Rectangle> platforms) : base(position)
        {
            _platforms = platforms;
        }
        
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            
            float moveX = 0;
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)) moveX = -1;
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)) moveX = 1;
            
            Velocity = new Vector2(moveX * _moveSpeed, Velocity.Y);
            Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);
            
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
            
            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.X > 1280 - Width) Position = new Vector2(1280 - Width, Position.Y);
            if (Position.Y > 1000)
            {
                Position = new Vector2(100, 550);
                Velocity = Vector2.Zero;
                _jumpCount = 0;
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
                    
                    float minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));
                    
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
            var rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            spriteBatch.Draw(rect, new Color(138, 43, 226));
            
            var eyeRect = new Rectangle((int)Position.X + 8, (int)Position.Y + 10, 6, 6);
            spriteBatch.Draw(eyeRect, Color.White);
            eyeRect = new Rectangle((int)Position.X + 18, (int)Position.Y + 10, 6, 6);
            spriteBatch.Draw(eyeRect, Color.White);
        }
    }
}
