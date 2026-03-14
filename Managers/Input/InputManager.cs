// ============================================================================
// InputManager.cs - Менеджер ввода / Input manager
// Обрабатывает ввод с клавиатуры, мыши и геймпада
// Handles keyboard, mouse and gamepad input
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PurpleLordPlatformer.Managers
{
    /// <summary>
    /// Менеджер ввода - централизованная обработка всех устройств ввода.
    /// Input manager - centralized handling of all input devices.
    /// </summary>
    public class InputManager
    {
        // Состояния клавиатуры / Keyboard states
        private KeyboardState _currentKeyboard;
        private KeyboardState _previousKeyboard;
        
        // Состояния мыши / Mouse states
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        
        // Состояния геймпада / Gamepad states
        private GamePadState _currentGamePad;
        private GamePadState _previousGamePad;
        private PlayerIndex _playerIndex = PlayerIndex.One;

        /// <summary>
        /// Обновление состояния ввода. Вызывается каждый кадр.
        /// Update input state. Called every frame.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _previousKeyboard = _currentKeyboard;
            _currentKeyboard = Keyboard.GetState();
            
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();
            
            _previousGamePad = _currentGamePad;
            _currentGamePad = GamePad.GetState(_playerIndex);
        }

        #region Keyboard | Клавиатура

        /// <summary>
        /// Проверка: клавиша нажата в текущем кадре.
        /// Check: key is pressed in current frame.
        /// </summary>
        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key);
        }

        /// <summary>
        /// Проверка: клавиша была нажата в этом кадре (ранее не была нажата).
        /// Check: key was pressed this frame (was not pressed before).
        /// </summary>
        public bool IsKeyJustPressed(Keys key)
        {
            return _currentKeyboard.IsKeyDown(key) && 
                   _previousKeyboard.IsKeyUp(key);
        }

        /// <summary>
        /// Проверка: клавиша была отпущена в этом кадре.
        /// Check: key was released this frame.
        /// </summary>
        public bool IsKeyJustReleased(Keys key)
        {
            return _currentKeyboard.IsKeyUp(key) && 
                   _previousKeyboard.IsKeyDown(key);
        }

        #endregion

        #region Mouse | Мышь

        /// <summary>
        /// Текущая позиция мыши в экранных координатах.
        /// Current mouse position in screen coordinates.
        /// </summary>
        public Vector2 MousePosition => new Vector2(
            _currentMouse.X, _currentMouse.Y);

        /// <summary>
        /// Проверка: левая кнопка мыши нажата.
        /// Check: left mouse button is pressed.
        /// </summary>
        public bool IsLeftButtonDown()
        {
            return _currentMouse.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Проверка: левая кнопка мыши только что нажата.
        /// Check: left mouse button was just pressed.
        /// </summary>
        public bool IsLeftButtonJustPressed()
        {
            return _currentMouse.LeftButton == ButtonState.Pressed &&
                   _previousMouse.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Проверка: правая кнопка мыши нажата.
        /// Check: right mouse button is pressed.
        /// </summary>
        public bool IsRightButtonDown()
        {
            return _currentMouse.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Колесо прокрутки - смещение за кадр.
        /// Scroll wheel - delta per frame.
        /// </summary>
        public int ScrollWheelValue => _currentMouse.ScrollWheelValue;
        
        /// <summary>
        /// Колесо прокрутки - изменение за кадр.
        /// Scroll wheel - change per frame.
        /// </summary>
        public int ScrollWheelDelta => 
            _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;

        #endregion

        #region Gamepad | Геймпад

        /// <summary>
        /// Проверка: кнопка геймпада нажата.
        /// Check: gamepad button is pressed.
        /// </summary>
        public bool IsGamePadButtonDown(Buttons button)
        {
            return _currentGamePad.IsButtonDown(button);
        }

        /// <summary>
        /// Проверка: кнопка геймпада только что нажата.
        /// Check: gamepad button was just pressed.
        /// </summary>
        public bool IsGamePadButtonJustPressed(Buttons button)
        {
            return _currentGamePad.IsButtonDown(button) &&
                   _previousGamePad.IsButtonUp(button);
        }

        /// <summary>
        /// Получить значение левого стика.
        /// Get left stick value.
        /// </summary>
        public Vector2 LeftStick => _currentGamePad.ThumbSticks.Left;

        /// <summary>
        /// Получить значение правого стика.
        /// Get right stick value.
        /// </summary>
        public Vector2 RightStick => _currentGamePad.ThumbSticks.Right;

        /// <summary>
        /// Получить значение левого триггера (0-1).
        /// Get left trigger value (0-1).
        /// </summary>
        public float LeftTrigger => _currentGamePad.Triggers.Left;

        /// <summary>
        /// Получить значение правого триггера (0-1).
        /// Get right trigger value (0-1).
        /// </summary>
        public float RightTrigger => _currentGamePad.Triggers.Right;

        /// <summary>
        /// Подключён ли геймпад.
        /// Is gamepad connected.
        /// </summary>
        public bool IsGamePadConnected => _currentGamePad.IsConnected;

        #endregion

        #region Input Axes | Оси ввода

        /// <summary>
        /// Получить горизонтальное движение (-1 влево, 1 вправо).
        /// Get horizontal movement (-1 left, 1 right).
        /// </summary>
        public float GetHorizontalAxis()
        {
            float keyboardInput = 0f;
            if (_currentKeyboard.IsKeyDown(Keys.A) || 
                _currentKeyboard.IsKeyDown(Keys.Left))
                keyboardInput -= 1f;
            if (_currentKeyboard.IsKeyDown(Keys.D) || 
                _currentKeyboard.IsKeyDown(Keys.Right))
                keyboardInput += 1f;
            
            float gamepadInput = _currentGamePad.ThumbSticks.Left.X;
            
            return MathHelper.Clamp(keyboardInput + gamepadInput, -1f, 1f);
        }

        /// <summary>
        /// Получить вертикальное движение (-1 вниз, 1 вверх).
        /// Get vertical movement (-1 down, 1 up).
        /// </summary>
        public float GetVerticalAxis()
        {
            float keyboardInput = 0f;
            if (_currentKeyboard.IsKeyDown(Keys.W) || 
                _currentKeyboard.IsKeyDown(Keys.Up))
                keyboardInput -= 1f;
            if (_currentKeyboard.IsKeyDown(Keys.S) || 
                _currentKeyboard.IsKeyDown(Keys.Down))
                keyboardInput += 1f;
            
            float gamepadInput = -_currentGamePad.ThumbSticks.Left.Y;
            
            return MathHelper.Clamp(keyboardInput + gamepadInput, -1f, 1f);
        }

        #endregion
    }
}
