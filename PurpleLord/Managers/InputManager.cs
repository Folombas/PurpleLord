// ============================================================================
// InputManager.cs - Менеджер ввода (клавиатура, геймпад, мышь)
// Input Manager (keyboard, gamepad, mouse)
// ============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PurpleLord.Managers;

/// <summary>
/// Централизованное управление вводом от всех устройств
/// Centralized input management from all devices
/// </summary>
public class InputManager
{
    private KeyboardState _currentKeyboard;
    private KeyboardState _previousKeyboard;
    
    private GamePadState _currentGamepad;
    private GamePadState _previousGamepad;
    
    private MouseState _currentMouse;
    private MouseState _previousMouse;

    /// <summary>
    /// Текущая позиция мыши
    /// Current mouse position
    /// </summary>
    public Vector2 MousePosition => new(_currentMouse.X, _currentMouse.Y);

    /// <summary>
    /// Дельта движения мыши
    /// Mouse movement delta
    /// </summary>
    public Vector2 MouseDelta => new(
        _currentMouse.X - _previousMouse.X,
        _currentMouse.Y - _previousMouse.Y);

    /// <summary>
    /// Обновление состояния ввода
    /// Update input state
    /// </summary>
    public void Update()
    {
        _previousKeyboard = _currentKeyboard;
        _previousGamepad = _currentGamepad;
        _previousMouse = _currentMouse;

        _currentKeyboard = Keyboard.GetState();
        _currentGamepad = GamePad.GetState(PlayerIndex.One);
        _currentMouse = Mouse.GetState();
    }

    #region Keyboard Input

    /// <summary>
    /// Проверка: клавиша нажата в текущем кадре
    /// Check: key is pressed in current frame
    /// </summary>
    public bool IsKeyDown(Keys key) => _currentKeyboard.IsKeyDown(key);

    /// <summary>
    /// Проверка: клавиша была нажата в этом кадре (just pressed)
    /// Check: key was just pressed in this frame
    /// </summary>
    public bool IsKeyPressed(Keys key) => 
        _currentKeyboard.IsKeyDown(key) && _previousKeyboard.IsKeyUp(key);

    /// <summary>
    /// Проверка: клавиша была отпущена в этом кадре (just released)
    /// Check: key was just released in this frame
    /// </summary>
    public bool IsKeyReleased(Keys key) => 
        _currentKeyboard.IsKeyUp(key) && _previousKeyboard.IsKeyDown(key);

    #endregion

    #region Gamepad Input

    /// <summary>
    /// Проверка: кнопка геймпада нажата
    /// Check: gamepad button is pressed
    /// </summary>
    public bool IsButtonDown(Buttons button) => _currentGamepad.IsButtonDown(button);

    /// <summary>
    /// Проверка: кнопка геймпада только что нажата
    /// Check: gamepad button was just pressed
    /// </summary>
    public bool IsButtonPressed(Buttons button) => 
        _currentGamepad.IsButtonDown(button) && _previousGamepad.IsButtonUp(button);

    /// <summary>
    /// Проверка: кнопка геймпада только что отпущена
    /// Check: gamepad button was just released
    /// </summary>
    public bool IsButtonReleased(Buttons button) => 
        _currentGamepad.IsButtonUp(button) && _previousGamepad.IsButtonDown(button);

    /// <summary>
    /// Получение значения левого стика
    /// Get left stick value
    /// </summary>
    public Vector2 LeftStick => _currentGamepad.ThumbSticks.Left;

    /// <summary>
    /// Получение значения правого стика
    /// Get right stick value
    /// </summary>
    public Vector2 RightStick => _currentGamepad.ThumbSticks.Right;

    /// <summary>
    /// Получение значения левого триггера
    /// Get left trigger value
    /// </summary>
    public float LeftTrigger => _currentGamepad.Triggers.Left;

    /// <summary>
    /// Получение значения правого триггера
    /// Get right trigger value
    /// </summary>
    public float RightTrigger => _currentGamepad.Triggers.Right;

    #endregion

    #region Mouse Input

    /// <summary>
    /// Проверка: кнопка мыши нажата
    /// Check: mouse button is pressed
    /// </summary>
    public bool IsMouseButtonDown(MouseButton button) => 
        _currentMouse.LeftButton == ButtonState.Pressed && button == MouseButton.Left ||
        _currentMouse.RightButton == ButtonState.Pressed && button == MouseButton.Right ||
        _currentMouse.MiddleButton == ButtonState.Pressed && button == MouseButton.Middle;

    /// <summary>
    /// Проверка: кнопка мыши только что нажата
    /// Check: mouse button was just pressed
    /// </summary>
    public bool IsMouseButtonPressed(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => _currentMouse.LeftButton == ButtonState.Pressed && 
                               _previousMouse.LeftButton == ButtonState.Released,
            MouseButton.Right => _currentMouse.RightButton == ButtonState.Pressed && 
                                _previousMouse.RightButton == ButtonState.Released,
            MouseButton.Middle => _currentMouse.MiddleButton == ButtonState.Pressed && 
                                 _previousMouse.MiddleButton == ButtonState.Released,
            _ => false
        };
    }

    /// <summary>
    /// Получение прокрутки колёсика мыши
    /// Get mouse wheel scroll
    /// </summary>
    public int MouseScrollDelta => _currentMouse.ScrollWheelValue - _previousMouse.ScrollWheelValue;

    #endregion

    /// <summary>
    /// Сброс ввода (для паузы или перехода между сценами)
    /// Reset input (for pause or scene transitions)
    /// </summary>
    public void Reset()
    {
        _previousKeyboard = _currentKeyboard;
        _previousGamepad = _currentGamepad;
        _previousMouse = _currentMouse;
    }
}

/// <summary>
/// Перечисление кнопок мыши
/// Mouse button enumeration
/// </summary>
public enum MouseButton
{
    Left,
    Right,
    Middle
}
