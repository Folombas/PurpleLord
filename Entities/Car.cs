// ============================================================================
// Car.cs - Автомобиль с колёсами и выхлопным дымом / Car with wheels and exhaust
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLord.Entities
{
    /// <summary>
    /// Автомобиль с круглыми колёсами и выхлопным дымом.
    /// Car with round wheels and exhaust smoke.
    /// </summary>
    public class Car
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; } = 1; // 1 = вправо, -1 = влево
        public float Width => 120;
        public float Height => 50;
        public float WheelRadius => 18;
        public float WheelRotation { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Обновление автомобиля.
        /// Update car.
        /// </summary>
        public void Update(GameTime gameTime, float dt)
        {
            if (!IsActive) return;
            
            X += Speed * Direction * dt;
            
            // Вращение колёс при движении
            WheelRotation += (Speed * dt) / WheelRadius;
            
            // Генерация выхлопного дыма
            if (Math.Abs(Speed) > 10)
            {
                // Добавляем частицы выхлопа
                return; // Логика дыма будет в Game1
            }
        }
        
        /// <summary>
        /// Отрисовка автомобиля.
        /// Draw car.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsActive) return;
            
            // Цвет кузова
            Color bodyColor = new Color(180, 50, 50); // Красный автомобиль
            Color windowColor = new Color(100, 150, 200); // Голубые окна
            Color wheelColor = Color.DarkGray;
            
            // Основной корпус (прямоугольник со скруглёнными краями)
            Rectangle bodyRect = new Rectangle(
                (int)(X - Width / 2),
                (int)(Y - Height / 2),
                (int)Width,
                (int)(Height * 0.7f)
            );
            spriteBatch.Draw(_pixel, bodyRect, bodyColor);
            
            // Верхняя часть (кабина)
            Rectangle cabinRect = new Rectangle(
                (int)(X - Width / 6),
                (int)(Y - Height),
                (int)(Width * 0.6f),
                (int)(Height * 0.5f)
            );
            spriteBatch.Draw(_pixel, cabinRect, bodyColor);
            
            // Окна
            Rectangle windowRect = new Rectangle(
                (int)(X - Width / 8),
                (int)(Y - Height + 5),
                (int)(Width * 0.45f),
                (int)(Height * 0.35f)
            );
            spriteBatch.Draw(_pixel, windowRect, windowColor);
            
            // Полоска на кузове
            Rectangle stripeRect = new Rectangle(
                (int)(X - Width / 2 + 5),
                (int)(Y - Height / 4),
                (int)(Width - 10),
                (int)(Height * 0.15f)
            );
            spriteBatch.Draw(_pixel, stripeRect, new Color(255, 200, 50));
            
            // Выхлопная труба (сзади)
            float exhaustX = Direction > 0 ? X - Width / 2 + 5 : X + Width / 2 - 5;
            Rectangle exhaustRect = new Rectangle(
                (int)exhaustX,
                (int)(Y - 5),
                8,
                12
            );
            spriteBatch.Draw(_pixel, exhaustRect, Color.Gray);
            
            // Круглые колёса
            float wheelY = Y + Height / 2 - WheelRadius;
            float frontWheelX = X + Width / 4;
            float backWheelX = X - Width / 4;
            
            // Заднее колесо
            DrawWheel(spriteBatch, backWheelX, wheelY, WheelRadius, WheelRotation, wheelColor);
            
            // Переднее колесо
            DrawWheel(spriteBatch, frontWheelX, wheelY, WheelRadius, WheelRotation, wheelColor);
            
            // Фары
            Color headlightColor = Direction > 0 ? new Color(255, 255, 200) : new Color(100, 0, 0);
            Color taillightColor = Direction > 0 ? new Color(200, 0, 0) : new Color(255, 255, 200);
            
            // Передняя фара
            Rectangle headlightRect = new Rectangle(
                (int)(X + (Direction > 0 ? Width / 2 - 3 : -Width / 2)),
                (int)(Y - 5),
                6,
                10
            );
            spriteBatch.Draw(_pixel, headlightRect, headlightColor);
            
            // Задний фонарь
            Rectangle taillightRect = new Rectangle(
                (int)(X + (Direction > 0 ? -Width / 2 : Width / 2 - 3)),
                (int)(Y - 5),
                6,
                10
            );
            spriteBatch.Draw(_pixel, taillightRect, taillightColor);
        }
        
        /// <summary>
        /// Отрисовка круглого колеса.
        /// Draw round wheel.
        /// </summary>
        private void DrawWheel(SpriteBatch spriteBatch, float x, float y, float radius, float rotation, Color color)
        {
            // Рисуем колесо как круг из пикселей
            int diameter = (int)(radius * 2);
            
            // Основной круг колеса
            for (int dy = -diameter / 2; dy <= diameter / 2; dy++)
            {
                for (int dx = -diameter / 2; dx <= diameter / 2; dx++)
                {
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius)
                    {
                        spriteBatch.Draw(_pixel, new Vector2(x + dx, y + dy), color);
                    }
                }
            }
            
            // Спицы колеса (для эффекта вращения)
            float spokeLength = radius * 0.7f;
            for (int i = 0; i < 5; i++)
            {
                float angle = rotation + i * MathHelper.TwoPi / 5;
                Vector2 spokeEnd = new Vector2(
                    x + (float)Math.Cos(angle) * spokeLength,
                    y + (float)Math.Sin(angle) * spokeLength
                );
                
                // Рисуем линию спицы
                Vector2 lineDir = spokeEnd - new Vector2(x, y);
                float lineLength = lineDir.Length();
                lineDir.Normalize();
                
                for (float t = 0; t < lineLength; t += 1)
                {
                    Vector2 point = new Vector2(x, y) + lineDir * t;
                    spriteBatch.Draw(_pixel, new Rectangle((int)point.X - 1, (int)point.Y - 1, 2, 2), Color.LightGray);
                }
            }
            
            // Центр колеса
            int centerSize = 4;
            Rectangle centerRect = new Rectangle(
                (int)(x - centerSize / 2),
                (int)(y - centerSize / 2),
                centerSize,
                centerSize
            );
            spriteBatch.Draw(_pixel, centerRect, Color.Silver);
        }
        
        // Вспомогательная текстура для рисования
        private static Texture2D _pixel;
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(graphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
        }
    }
    
    /// <summary>
    /// Частица выхлопного дыма.
    /// Exhaust smoke particle.
    /// </summary>
    public class ExhaustParticle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VX { get; set; }
        public float VY { get; set; }
        public float Size { get; set; } = 10;
        public float Life { get; set; } = 2;
        public Color Color { get; set; } = new Color(100, 100, 100, 150);
        
        /// <summary>
        /// Обновление частицы.
        /// Update particle.
        /// </summary>
        public void Update(float dt)
        {
            X += VX * dt;
            Y += VY * dt;
            Size += 5 * dt; // Частица расширяется
            Life -= dt;
            
            // Изменение цвета к более прозрачному
            if (Life < 1)
            {
                Color = new Color(Color.R, Color.G, Color.B, (byte)(Color.A * Life));
            }
        }
        
        /// <summary>
        /// Отрисовка частицы.
        /// Draw particle.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Life <= 0) return;
            
            // Рисуем круглую частицу дыма
            int radius = (int)(Size / 2);
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius)
                    {
                        spriteBatch.Draw(_pixel, new Vector2(X + dx, Y + dy), Color);
                    }
                }
            }
        }
        
        // Вспомогательная текстура
        private static Texture2D _pixel;
        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(graphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
        }
    }
}
