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
        
        // Спокойное состояние
        private float _idleTimer = 0f;
        private float _lookDirectionTimer = 0f;
        private bool _lookingLeft = false;
        private bool _isIdle = true;
        
        // Система XP
        public int XP { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int XPToNextLevel { get; set; } = 100;
        
        // Состояние при получении урона
        private bool _isHurt = false;
        private float _hurtTimer = 0f;
        private float _hurtDuration = 0.3f;
        private Vector2 _hurtVelocity = Vector2.Zero;

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

            // === ИНИЦИАЛИЗАЦИЯ ПРЕДЫДУЩЕГО СОСТОЯНИЯ (чтобы не было авто-прыжка) ===
            if (_previousKeyboardState == null)
                _previousKeyboardState = keyboardState;

            // === ОБРАБОТКА ПОЛУЧЕНИЯ УРОНА ===
            if (_isHurt)
            {
                _hurtTimer -= deltaTime;
                Position += _hurtVelocity * deltaTime;
                Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);

                if (_hurtTimer <= 0)
                {
                    _isHurt = false;
                    _hurtVelocity = Vector2.Zero;
                }
                else
                {
                    _previousKeyboardState = keyboardState;
                    return; // Не управляем во время получения урона
                }
            }

            // === ДВИЖЕНИЕ ===
            float moveX = 0;
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                moveX = -1;
                _facingRight = false;
                _isIdle = false;
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                moveX = 1;
                _facingRight = true;
                _isIdle = false;
            }

            // Горизонтальная скорость
            Velocity = new Vector2(moveX * _moveSpeed, Velocity.Y);

            // Гравитация
            Velocity = new Vector2(Velocity.X, Velocity.Y + _gravity * deltaTime);

            // === ПРЫЖОК (только по нажатию, не авто) ===
            bool jumpPressed = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W);
            bool jumpJustPressed = jumpPressed &&
                (!_previousKeyboardState.HasValue || 
                (!_previousKeyboardState.Value.IsKeyDown(Keys.Space) &&
                !_previousKeyboardState.Value.IsKeyDown(Keys.W)));

            if (jumpJustPressed)
            {
                if (_isGrounded)
                {
                    Velocity = new Vector2(Velocity.X, _jumpForce);
                    _jumpCount = 1;
                    _isGrounded = false;
                    _isIdle = false;
                }
                else if (_jumpCount < _maxJumps)
                {
                    Velocity = new Vector2(Velocity.X, _jumpForce);
                    _jumpCount++;
                    _isIdle = false;
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

            // === АНИМАЦИЯ И СПОКОЙНОЕ СОСТОЯНИЕ ===
            if (moveX != 0 && _isGrounded)
            {
                _walkAnimationTimer += deltaTime * 10f;
                _bounceOffset = (float)Math.Sin(_walkAnimationTimer) * 3f;
                _idleTimer = 0f;
                _isIdle = false;
            }
            else if (_isGrounded && !_isHurt)
            {
                _idleTimer += deltaTime;
                _walkAnimationTimer = 0f;
                _bounceOffset = 0f;
                
                // В спокойном состоянии смотрим по сторонам
                if (_idleTimer > 2f)
                {
                    _lookDirectionTimer += deltaTime;
                    if (_lookDirectionTimer > 1.5f)
                    {
                        _lookingLeft = !_lookingLeft;
                        _lookDirectionTimer = 0f;
                    }
                }
            }
            else
            {
                _idleTimer = 0f;
                _isIdle = false;
            }

            _previousKeyboardState = keyboardState;
        }

        private KeyboardState? _previousKeyboardState;

        /// <summary>
        /// Получить урон от столкновения с врагом
        /// </summary>
        public void TakeDamage(Vector2 damageSource, int damageAmount)
        {
            if (_isHurt) return;
            
            _isHurt = true;
            _hurtTimer = _hurtDuration;
            XP = Math.Max(0, XP - damageAmount);
            
            // Отталкивание от источника урона
            float direction = Position.X < damageSource.X ? -1 : 1;
            _hurtVelocity = new Vector2(direction * 400f, -300f);
            
            // Проверка на повышение уровня (если XP уменьшился достаточно)
            CheckLevel();
        }

        /// <summary>
        /// Собрать предмет и получить XP
        /// </summary>
        public void CollectItem(int xpAmount)
        {
            XP += xpAmount;
            CheckLevel();
        }

        /// <summary>
        /// Проверить повышение уровня
        /// </summary>
        private void CheckLevel()
        {
            while (XP >= XPToNextLevel)
            {
                Level++;
                XP -= XPToNextLevel;
                XPToNextLevel = (int)(XPToNextLevel * 1.5f);
            }
        }
        
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
            
            // Мигание при получении урона
            if (_isHurt && (int)(_hurtTimer * 20) % 2 == 0)
            {
                auraColor = new Color(255, 100, 100, 150);
            }

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

            // === ЛИЦО ===
            // В спокойном состоянии смотрим по сторонам
            bool drawFaceRight = _isIdle ? !_lookingLeft : _facingRight;
            int faceX = drawFaceRight ? x + 8 : x - 20;
            int faceY = y - 10;

            // Белки глаз
            spriteBatch.Draw(new Rectangle(faceX, faceY, 10, 10), eyeColor);
            spriteBatch.Draw(new Rectangle(faceX + 12, faceY, 10, 10), eyeColor);

            // Зрачки
            int pupilOffset = drawFaceRight ? 3 : -3;
            spriteBatch.Draw(new Rectangle(faceX + 2 + pupilOffset, faceY + 2, 4, 4), pupilColor);
            spriteBatch.Draw(new Rectangle(faceX + 14 + pupilOffset, faceY + 2, 4, 4), pupilColor);
            
            // === УЛЫБКА (в спокойном состоянии) ===
            if (_isIdle && _isGrounded && !_isHurt)
            {
                // Рисуем улыбку
                int smileY = faceY + 14;
                for (int i = 0; i < 10; i++)
                {
                    int smileOffset = (int)(Math.Pow(i - 5, 2) / 5);
                    spriteBatch.Draw(
                        new Rectangle(faceX + 2 + i, smileY + smileOffset, 2, 2),
                        skinColor
                    );
                }
            }
            else if (!_isIdle && _isGrounded)
            {
                // Нейтральное выражение при движении
                spriteBatch.Draw(new Rectangle(faceX + 4, faceY + 14, 8, 3), skinColor);
            }
            else
            {
                // Открытый рот в прыжке
                spriteBatch.Draw(new Rectangle(faceX + 4, faceY + 13, 8, 6), skinColor);
            }
            
            // Брови (выражают эмоции)
            if (_isHurt)
            {
                // Грустные брови
                spriteBatch.Draw(new Rectangle(faceX, faceY - 3, 8, 2), bodyDark);
                spriteBatch.Draw(new Rectangle(faceX + 10, faceY - 1, 8, 2), bodyDark);
            }
            else if (_isIdle)
            {
                // Спокойные брови
                spriteBatch.Draw(new Rectangle(faceX, faceY - 2, 8, 2), bodyDark);
                spriteBatch.Draw(new Rectangle(faceX + 10, faceY - 2, 8, 2), bodyDark);
            }

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
                if (_isIdle)
                {
                    // В спокойном состоянии ноги расслаблены
                    spriteBatch.Draw(new Rectangle(x - 12, y + Radius - 5, 8, 10), bodyDark);
                    spriteBatch.Draw(new Rectangle(x + 4, y + Radius - 5, 8, 10), bodyDark);
                }
                else
                {
                    float legOffset = (float)Math.Sin(_walkAnimationTimer) * 8f;
                    spriteBatch.Draw(new Rectangle(x - 12, y + Radius - 5, 8, 12 + (int)legOffset), bodyDark);
                    spriteBatch.Draw(new Rectangle(x + 4, y + Radius - 5, 8, 12 - (int)legOffset), bodyDark);
                }
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
