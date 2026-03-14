// ============================================================================
// Camera2D.cs - 2D Камера / 2D Camera
// Камера для 2D платформера с поддержкой зума и слежения
// Camera for 2D platformer with zoom and tracking support
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Core
{
    /// <summary>
    /// 2D камера с поддержкой слежения за объектом, зума и тряски.
    /// 2D camera with object tracking, zoom and shake support.
    /// </summary>
    public class Camera2D
    {
        // Позиция камеры / Camera position
        public Vector2 Position { get; set; }
        
        // Целевая позиция для плавного слежения / Target position for smooth tracking
        public Vector2 TargetPosition { get; set; }
        
        // Уровень зума / Zoom level
        public float Zoom { get; set; } = 1f;
        
        // Целевой зум / Target zoom
        public float TargetZoom { get; set; } = 1f;
        
        // Вращение камеры / Camera rotation
        public float Rotation { get; set; } = 0f;
        
        // Скорость слежения / Tracking speed
        public float TrackingSpeed { get; set; } = 0.1f;
        
        // Скорость зума / Zoom speed
        public float ZoomSpeed { get; set; } = 0.05f;
        
        // Параметры тряски / Shake parameters
        private float _shakeMagnitude = 0f;
        private float _shakeDuration = 0f;
        private float _shakeTimer = 0f;
        
        // Смещение тряски / Shake offset
        private Vector2 _shakeOffset = Vector2.Zero;
        
        // Границы камеры / Camera bounds
        public Rectangle? Bounds { get; set; }
        
        // Viewport / Viewport
        private Viewport _viewport;
        
        // Матрица трансформации / Transform matrix
        private Matrix _transformMatrix;

        /// <summary>
        /// Конструктор камеры.
        /// Camera constructor.
        /// </summary>
        public Camera2D(Viewport viewport)
        {
            _viewport = viewport;
            Position = Vector2.Zero;
            TargetPosition = Vector2.Zero;
        }

        /// <summary>
        /// Обновление камеры.
        /// Camera update.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Плавное слежение за целью / Smooth tracking to target
            if (Position != TargetPosition)
            {
                Position = Vector2.Lerp(Position, TargetPosition, TrackingSpeed);
            }
            
            // Плавный зум / Smooth zoom
            if (Zoom != TargetZoom)
            {
                Zoom = MathHelper.Lerp(Zoom, TargetZoom, ZoomSpeed);
                Zoom = MathHelper.Max(Zoom, 0.1f); // Минимальный зум / Minimum zoom
                Zoom = MathHelper.Min(Zoom, 5f);   // Максимальный зум / Maximum zoom
            }
            
            // Обновление тряски / Update shake
            if (_shakeDuration > 0f)
            {
                _shakeTimer -= delta;
                if (_shakeTimer <= 0f)
                {
                    // Генерация нового случайного смещения / Generate new random offset
                    _shakeOffset = new Vector2(
                        MathHelper.Lerp(-_shakeMagnitude, _shakeMagnitude, 
                            (float)Microsoft.Xna.Framework.Game1.Random.NextDouble()),
                        MathHelper.Lerp(-_shakeMagnitude, _shakeMagnitude, 
                            (float)Microsoft.Xna.Framework.Game1.Random.NextDouble()));
                    _shakeTimer = 0.05f; // Обновление каждые 50мс / Update every 50ms
                }
                
                _shakeDuration -= delta;
                if (_shakeDuration <= 0f)
                {
                    _shakeOffset = Vector2.Zero;
                    _shakeMagnitude = 0f;
                }
            }
            
            // Применение границ / Apply bounds
            ApplyBounds();
            
            // Обновление матрицы трансформации / Update transform matrix
            UpdateTransformMatrix();
        }

        /// <summary>
        /// Применение границ камеры.
        /// Apply camera bounds.
        /// </summary>
        private void ApplyBounds()
        {
            if (Bounds.HasValue)
            {
                Rectangle bounds = Bounds.Value;
                float halfWidth = _viewport.Width / (2f * Zoom);
                float halfHeight = _viewport.Height / (2f * Zoom);
                
                Position.X = MathHelper.Clamp(Position.X, 
                    bounds.Left + halfWidth, bounds.Right - halfWidth);
                Position.Y = MathHelper.Clamp(Position.Y, 
                    bounds.Top + halfHeight, bounds.Bottom - halfHeight);
            }
        }

        /// <summary>
        /// Обновление матрицы трансформации.
        /// Update transform matrix.
        /// </summary>
        private void UpdateTransformMatrix()
        {
            Matrix translation = Matrix.CreateTranslation(-Position.X + _shakeOffset.X, 
                -Position.Y + _shakeOffset.Y, 0);
            Matrix rotation = Matrix.CreateRotationZ(Rotation);
            Matrix scale = Matrix.CreateScale(Zoom, Zoom, 1);
            Matrix viewportCenter = Matrix.CreateTranslation(
                _viewport.Width / 2f, _viewport.Height / 2f, 0);
            
            _transformMatrix = translation * rotation * scale * viewportCenter;
        }

        /// <summary>
        /// Получить матрицу трансформации для SpriteBatch.
        /// Get transform matrix for SpriteBatch.
        /// </summary>
        public Matrix GetTransformMatrix()
        {
            return _transformMatrix;
        }

        /// <summary>
        /// Установить цель слежения.
        /// Set tracking target.
        /// </summary>
        public void SetTarget(Vector2 target)
        {
            TargetPosition = target;
        }

        /// <summary>
        /// Мгновенно переместить камеру.
        /// Instantly move camera.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            Position = position;
            TargetPosition = position;
        }

        /// <summary>
        /// Установить зум.
        /// Set zoom.
        /// </summary>
        public void SetZoom(float zoom)
        {
            TargetZoom = zoom;
        }

        /// <summary>
        /// Начать тряску камеры.
        /// Start camera shake.
        /// </summary>
        public void Shake(float magnitude, float duration)
        {
            _shakeMagnitude = magnitude;
            _shakeDuration = duration;
            _shakeTimer = 0f;
        }

        /// <summary>
        /// Проверка видимости точки.
        /// Check if point is visible.
        /// </summary>
        public bool IsVisible(Vector2 position)
        {
            Vector2 screenPos = WorldToScreen(position);
            return screenPos.X >= 0 && screenPos.X <= _viewport.Width &&
                   screenPos.Y >= 0 && screenPos.Y <= _viewport.Height;
        }

        /// <summary>
        /// Проверка видимости прямоугольника.
        /// Check if rectangle is visible.
        /// </summary>
        public bool IsVisible(Rectangle rectangle)
        {
            Vector2 topLeft = WorldToScreen(new Vector2(rectangle.Left, rectangle.Top));
            Vector2 bottomRight = WorldToScreen(new Vector2(rectangle.Right, rectangle.Bottom));
            
            return bottomRight.X >= 0 && topLeft.X <= _viewport.Width &&
                   bottomRight.Y >= 0 && topLeft.Y <= _viewport.Height;
        }

        /// <summary>
        /// Преобразование мировых координат в экранные.
        /// Convert world coordinates to screen coordinates.
        /// </summary>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetTransformMatrix());
        }

        /// <summary>
        /// Преобразование экранных координат в мировые.
        /// Convert screen coordinates to world coordinates.
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            Matrix inverseTransform = Matrix.Invert(GetTransformMatrix());
            return Vector2.Transform(screenPosition, inverseTransform);
        }

        /// <summary>
        /// Получить видимую область камеры в мировых координатах.
        /// Get visible camera area in world coordinates.
        /// </summary>
        public Rectangle GetVisibleArea()
        {
            float halfWidth = _viewport.Width / (2f * Zoom);
            float halfHeight = _viewport.Height / (2f * Zoom);
            
            return new Rectangle(
                (int)(Position.X - halfWidth),
                (int)(Position.Y - halfHeight),
                (int)(_viewport.Width / Zoom),
                (int)(_viewport.Height / Zoom));
        }
    }
}
