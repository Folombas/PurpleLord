// ============================================================================
// Door.cs - Дверь/Портал / Door/Portal
// Выход с уровня, выбор пути
// Level exit, path choice
// ============================================================================

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;

namespace PurpleLordPlatformer.Entities.Objects
{
    public enum DoorType
    {
        Exit,           // Выход с уровня / Level exit
        Locked,         // Заблокирована (FOMO) / Locked (FOMO)
        Secret,         // Секретная дверь / Secret door
        Checkpoint      // Точка сохранения / Checkpoint
    }

    public class Door : GameObject
    {
        private DoorType _type;
        private Texture2D _texture;
        private Color _tintColor = Color.White;
        private bool _isOpen = false;
        private bool _isUnlocked = false;
        private string _targetLevelId = "";
        private string _lockReason = "";
        
        private float _pulseTimer = 0f;
        private float _pulseSpeed = 3f;

        public Door(Vector2 position, DoorType type, string targetLevelId = "")
            : base(position)
        {
            _type = type;
            _targetLevelId = targetLevelId;
            Width = 64;
            Height = 96;
            Tag = "Door";
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _pulseTimer += delta * _pulseSpeed;

            // Анимация пульсации для закрытых дверей / Pulse animation for closed doors
            if (!_isOpen && _type == DoorType.Locked)
            {
                float alpha = 0.5f + (float)Math.Sin(_pulseTimer) * 0.2f;
                _tintColor = new Color(1f, 1f, 1f, alpha);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color drawColor = _isOpen ? Color.Green : 
                             (_type == DoorType.Locked ? Color.Red : _tintColor);

            if (_texture != null)
            {
                spriteBatch.Draw(
                    _texture,
                    Position,
                    null,
                    drawColor,
                    0f,
                    new Vector2(Width / 2, Height / 2),
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                DrawPlaceholder(spriteBatch, drawColor);
            }

            // Отрисовка замка для закрытых дверей / Draw lock for closed doors
            if (_type == DoorType.Locked && !_isOpen)
            {
                DrawLockIcon(spriteBatch);
            }
        }

        private void DrawPlaceholder(SpriteBatch spriteBatch, Color color)
        {
            // Рисуем прямоугольник двери / Draw door rectangle
            Rectangle rect = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            
            // Рама двери / Door frame
            GraphicsUtils.DrawRectangle(spriteBatch, rect, color, 4);
            
            // Внутренняя часть / Inner part
            Rectangle inner = new Rectangle(
                rect.X + 4,
                rect.Y + 4,
                rect.Width - 8,
                rect.Height - 8);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, inner, new Color(color.R, color.G, color.B, 100));
        }

        private void DrawLockIcon(SpriteBatch spriteBatch)
        {
            Vector2 lockPos = new Vector2(Position.X, Position.Y - Height / 4);
            
            // Замок - маленький круг с дужкой / Lock - small circle with shackle
            float lockSize = 10f;
            
            // Дужка замка / Lock shackle
            spriteBatch.Draw(
                GraphicsUtils.WhiteTexture,
                new Vector2(lockPos.X, lockPos.Y - lockSize),
                null,
                Color.Gold,
                0f,
                new Vector2(2f, lockSize),
                new Vector2(1f, 1f),
                SpriteEffects.None,
                0f);
            
            // Тело замка / Lock body
            Rectangle lockRect = new Rectangle(
                (int)(lockPos.X - lockSize),
                (int)(lockPos.Y - lockSize / 2),
                (int)(lockSize * 2),
                (int)lockSize);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, lockRect, Color.Gold);
        }

        public void Open()
        {
            if (_type == DoorType.Locked && !_isUnlocked)
                return;

            _isOpen = true;
            OnOpen?.Invoke(this);
        }

        public void Unlock()
        {
            _isUnlocked = true;
            _type = DoorType.Exit;
        }

        public void Interact(Player.Player player)
        {
            if (_isOpen && !string.IsNullOrEmpty(_targetLevelId))
            {
                OnEnter?.Invoke(this, _targetLevelId);
            }
            else if (_type == DoorType.Locked && !_isUnlocked)
            {
                OnLocked?.Invoke(this, _lockReason);
            }
        }

        public event Action<Door> OnOpen;
        public event Action<Door, string> OnEnter;
        public event Action<Door, string> OnLocked;

        public DoorType Type => _type;
        public bool IsOpen => _isOpen;
        public bool IsUnlocked => _isUnlocked;
        public string TargetLevelId => _targetLevelId;
        public string LockReason 
        { 
            get => _lockReason;
            set => _lockReason = value;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
    }
}
