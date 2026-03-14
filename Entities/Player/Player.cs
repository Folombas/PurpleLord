// ============================================================================
// Player.cs - Класс игрока / Player class
// Фиолетовый Лорд - главный герой
// Purple Lord - main character
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PurpleLordPlatformer.Core;
using PurpleLordPlatformer.Managers;
using PurpleLordPlatformer.Systems.Physics;
using PurpleLordPlatformer.Systems.Animation;

namespace PurpleLordPlatformer.Entities.Player
{
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Glitch,
        Focus
    }

    public class Player : GameObject
    {
        // Физические параметры / Physics parameters
        public const float Gravity = 1500f;
        public const float JumpForce = -650f;
        public const float MoveSpeed = 300f;
        public const float MaxFallSpeed = 800f;
        public const float Friction = 0.85f;
        public const float Acceleration = 2000f;

        // Параметры двойного прыжка / Double jump parameters
        private int _maxJumps = 2;
        private int _currentJumps = 0;
        private bool _canDoubleJump = true;

        // Режим фокуса / Focus mode
        private bool _isFocusing = false;
        private float _focusMeter = 100f;
        private float _focusDrainRate = 20f;
        private float _focusRegenRate = 10f;
        private const float MinFocusMeter = 0f;
        private const float MaxFocusMeter = 100f;

        // Состояние / State
        private PlayerState _currentState = PlayerState.Idle;
        private PlayerState _previousState = PlayerState.Idle;
        private bool _isGrounded = false;
        private float _facingDirection = 1f;

        // Анимация / Animation
        private Animator _animator;
        private float _idleTimer = 0f;
        private const float IdleGlitchTime = 3f;

        // Частицы / Particles
        private ParticleSystem _trailParticles;

        // Ввод / Input
        private InputManager _input;

        public Player(Vector2 position) : base(position)
        {
            Width = 32;
            Height = 48;
            Velocity = Vector2.Zero;
            _input = new InputManager();
            InitializeAnimator();
        }

        private void InitializeAnimator()
        {
            _animator = new Animator();
            // Анимации будут добавлены при загрузке контента
            // Animations will be added on content load
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(delta);
            ApplyPhysics(delta);
            UpdateState();
            UpdateAnimations(delta);
            UpdateParticles(gameTime);
            UpdateFocus(delta);

            base.Update(gameTime);
        }

        private void HandleInput(float delta)
        {
            _input.Update(new GameTime());

            // Горизонтальное движение / Horizontal movement
            float horizontal = _input.GetHorizontalAxis();

            if (horizontal != 0)
            {
                Velocity.X += horizontal * Acceleration * delta;
                _facingDirection = Math.Sign(horizontal);
            }

            // Прыжок / Jump
            if (_input.IsKeyJustPressed(Keys.Space) || 
                _input.IsGamePadButtonJustPressed(Buttons.A))
            {
                if (_isGrounded)
                {
                    Jump();
                }
                else if (_canDoubleJump && _currentJumps < _maxJumps)
                {
                    DoubleJump();
                }
            }

            // Режим фокуса / Focus mode
            if (_input.IsKeyJustPressed(Keys.F) ||
                _input.IsGamePadButtonJustPressed(Buttons.LeftShoulder))
            {
                ToggleFocus();
            }
        }

        private void Jump()
        {
            Velocity.Y = JumpForce;
            _currentJumps = 1;
            _isGrounded = false;
            OnJump?.Invoke();
        }

        private void DoubleJump()
        {
            Velocity.Y = JumpForce * 0.8f;
            _currentJumps = 2;
            _canDoubleJump = false;
            OnDoubleJump?.Invoke();
        }

        private void ToggleFocus()
        {
            if (_focusMeter > 10f && !_isFocusing)
            {
                _isFocusing = true;
                OnFocusActivated?.Invoke();
            }
            else if (_isFocusing)
            {
                _isFocusing = false;
                OnFocusDeactivated?.Invoke();
            }
        }

        private void ApplyPhysics(float delta)
        {
            // Гравитация / Gravity
            if (!_isGrounded)
            {
                Velocity.Y += Gravity * delta;
                Velocity.Y = MathHelper.Min(Velocity.Y, MaxFallSpeed);
            }

            // Трение / Friction
            if (_isGrounded)
            {
                Velocity.X *= Friction;
            }
            else
            {
                Velocity.X *= 0.95f; // Меньше трения в воздухе / Less friction in air
            }

            // Применение скорости / Apply velocity
            Position += Velocity * delta;
        }

        private void UpdateState()
        {
            _previousState = _currentState;

            if (_isFocusing)
            {
                _currentState = PlayerState.Focus;
            }
            else if (!_isGrounded)
            {
                if (Velocity.Y < 0)
                    _currentState = PlayerState.Jump;
                else
                    _currentState = PlayerState.Fall;
            }
            else if (Math.Abs(Velocity.X) < 10)
            {
                _currentState = PlayerState.Idle;
            }
            else
            {
                _currentState = PlayerState.Run;
            }
        }

        private void UpdateAnimations(float delta)
        {
            if (_currentState == PlayerState.Idle)
            {
                _idleTimer += delta;
                if (_idleTimer > IdleGlitchTime)
                {
                    _currentState = PlayerState.Glitch;
                    _idleTimer = 0;
                }
            }
            else
            {
                _idleTimer = 0;
            }

            _animator?.SetState(_currentState.ToString());
        }

        private void UpdateParticles(GameTime gameTime)
        {
            if (_trailParticles != null)
            {
                _trailParticles.Update(gameTime);
            }

            // Создание частиц при беге / Create particles while running
            if (_currentState == PlayerState.Run && Math.Abs(Velocity.X) > 50)
            {
                EmitTrailParticles();
            }
        }

        private void EmitTrailParticles()
        {
            // TODO: Реализовать систему частиц / TODO: Implement particle system
        }

        private void UpdateFocus(float delta)
        {
            if (_isFocusing)
            {
                _focusMeter -= _focusDrainRate * delta;
                if (_focusMeter <= MinFocusMeter)
                {
                    _focusMeter = MinFocusMeter;
                    _isFocusing = false;
                    OnFocusDeactivated?.Invoke();
                }
            }
            else
            {
                _focusMeter += _focusRegenRate * delta;
                _focusMeter = MathHelper.Min(_focusMeter, MaxFocusMeter);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Отрисовка игрока / Draw player
            Color color = _isFocusing ? Color.Cyan : Color.White;
            
            if (_animator?.CurrentTexture != null)
            {
                spriteBatch.Draw(
                    _animator.CurrentTexture,
                    Position,
                    null,
                    color,
                    0f,
                    _animator.Origin,
                    1f,
                    SpriteEffects.None,
                    0f);
            }
            else
            {
                // Placeholder - фиолетовый прямоугольник / Placeholder - purple rectangle
                Rectangle rect = new Rectangle(
                    (int)(Position.X - Width / 2),
                    (int)(Position.Y - Height / 2),
                    (int)Width,
                    (int)Height);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, Color.Purple);
            }

            // Отрисовка частиц / Draw particles
            _trailParticles?.Draw(spriteBatch);
        }

        public void SetGrounded(bool grounded)
        {
            _isGrounded = grounded;
            if (_isGrounded)
            {
                _currentJumps = 0;
                _canDoubleJump = true;
            }
        }

        #region Events | События

        public event Action OnJump;
        public event Action OnDoubleJump;
        public event Action OnFocusActivated;
        public event Action OnFocusDeactivated;
        public event Action OnLand;

        #endregion

        #region Properties | Свойства

        public PlayerState CurrentState => _currentState;
        public bool IsGrounded => _isGrounded;
        public bool IsFocusing => _isFocusing;
        public float FocusMeter => _focusMeter;
        public float FocusPercent => _focusMeter / MaxFocusMeter * 100f;
        public float FacingDirection => _facingDirection;

        #endregion
    }
}
