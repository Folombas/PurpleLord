// ============================================================================
// SceneManager.cs - Менеджер сцен / Scene manager
// Управляет переключением между сценами игры
// Manages switching between game scenes
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Managers;

namespace PurpleLordPlatformer.Scenes
{
    /// <summary>
    /// Делегат для события смены сцены.
    /// Delegate for scene change event.
    /// </summary>
    public delegate void SceneChangedHandler(Scene oldScene, Scene newScene);

    /// <summary>
    /// Менеджер сцен - управляет жизненным циклом сцен.
    /// Scene manager - manages scene lifecycle.
    /// </summary>
    public class SceneManager : IDisposable
    {
        // Ссылка на игру / Game reference
        private readonly Game _game;
        
        // Менеджеры / Managers
        private readonly ContentManager _contentManager;
        private readonly InputManager _inputManager;
        private readonly AudioManager _audioManager;
        private readonly EffectManager _effectManager;
        private readonly UIManager _uiManager;
        
        // Стек сцен (для паузы и возврата) / Scene stack (for pause and return)
        private readonly Stack<Scene> _sceneStack = new Stack<Scene>();
        
        // Текущая активная сцена / Current active scene
        private Scene _currentScene;
        
        // Событие смены сцены / Scene change event
        public event SceneChangedHandler OnSceneChanged;

        /// <summary>
        /// Конструктор менеджера сцен.
        /// Scene manager constructor.
        /// </summary>
        public SceneManager(Game game, ContentManager contentManager, 
            InputManager inputManager, AudioManager audioManager, 
            EffectManager effectManager, UIManager uiManager)
        {
            _game = game;
            _contentManager = contentManager;
            _inputManager = inputManager;
            _audioManager = audioManager;
            _effectManager = effectManager;
            _uiManager = uiManager;
        }

        /// <summary>
        /// Загрузить новую сцену (заменяет текущую).
        /// Load new scene (replaces current).
        /// </summary>
        public void LoadScene<T>() where T : Scene, new()
        {
            Scene oldScene = _currentScene;
            
            // Выгрузить текущую сцену / Unload current scene
            _currentScene?.Unload();
            
            // Создать и загрузить новую сцену / Create and load new scene
            _currentScene = new T();
            _currentScene.Initialize(_game, _contentManager, _inputManager, 
                _audioManager, _effectManager, _uiManager);
            _currentScene.LoadContent();
            
            // Добавить в стек / Add to stack
            _sceneStack.Clear();
            _sceneStack.Push(_currentScene);
            
            // Вызвать событие / Trigger event
            OnSceneChanged?.Invoke(oldScene, _currentScene);
        }

        /// <summary>
        /// Push сцену в стек (для паузы, диалогов).
        /// Push scene to stack (for pause, dialogs).
        /// </summary>
        public void PushScene<T>() where T : Scene, new()
        {
            Scene oldScene = _currentScene;
            
            // Приостановить текущую сцену / Pause current scene
            _currentScene?.Pause();
            
            // Создать и загрузить новую сцену / Create and load new scene
            _currentScene = new T();
            _currentScene.Initialize(_game, _contentManager, _inputManager, 
                _audioManager, _effectManager, _uiManager);
            _currentScene.LoadContent();
            
            // Добавить в стек / Add to stack
            _sceneStack.Push(_currentScene);
            
            // Вызвать событие / Trigger event
            OnSceneChanged?.Invoke(oldScene, _currentScene);
        }

        /// <summary>
        /// Pop сцену из стека (возврат к предыдущей).
        /// Pop scene from stack (return to previous).
        /// </summary>
        public void PopScene()
        {
            if (_sceneStack.Count <= 1)
                return;
            
            Scene oldScene = _currentScene;
            
            // Выгрузить текущую сцену / Unload current scene
            _currentScene?.Unload();
            _sceneStack.Pop();
            
            // Восстановить предыдущую сцену / Restore previous scene
            _currentScene = _sceneStack.Peek();
            _currentScene.Resume();
            
            // Вызвать событие / Trigger event
            OnSceneChanged?.Invoke(oldScene, _currentScene);
        }

        /// <summary>
        /// Обновление текущей сцены.
        /// Update current scene.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _currentScene?.Update(gameTime);
        }

        /// <summary>
        /// Отрисовка текущей сцены.
        /// Draw current scene.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _currentScene?.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// Release resources.
        /// </summary>
        public void Dispose()
        {
            _currentScene?.Unload();
            _sceneStack.Clear();
        }

        #region Properties | Свойства

        /// <summary>
        /// Текущая активная сцена.
        /// Current active scene.
        /// </summary>
        public Scene CurrentScene => _currentScene;

        /// <summary>
        /// Количество сцен в стеке.
        /// Number of scenes in stack.
        /// </summary>
        public int SceneCount => _sceneStack.Count;

        /// <summary>
        /// Есть ли сцены в стеке.
        /// Are there scenes in stack.
        /// </summary>
        public bool HasScenes => _sceneStack.Count > 0;

        #endregion
    }
}
