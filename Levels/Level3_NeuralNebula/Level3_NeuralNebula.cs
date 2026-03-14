// ============================================================================
// Level3_NeuralNebula.cs - Уровень 3: Туман Нейросетей
// Level 3: Neural Network Nebula - Abstract, flowing neural-themed level
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Levels;
using PurpleLordPlatformer.Entities.Objects;
using PurpleLordPlatformer.Entities.Enemies;
using PurpleLordPlatformer.Core;

namespace PurpleLordPlatformer.Levels
{
    public class Level3_NeuralNebula : Level
    {
        private List<NeuralPlatform> _neuralPlatforms = new List<NeuralPlatform>();
        private float _networkTimer = 0f;
        private ParticleSystem.NeuralParticleEffect _particleEffect;

        public override string SceneId => "level3_neural";
        public override string SceneName => "Neural Network Nebula";
        public override Color BackgroundColor => new Color(30, 10, 50); // Deep purple

        public override void Initialize(Game game, ContentManager contentManager,
            Managers.InputManager inputManager, Managers.AudioManager audioManager,
            Managers.EffectManager effectManager, UI.UIManager uiManager)
        {
            base.Initialize(game, contentManager, inputManager, audioManager, effectManager, uiManager);
            _stats = new LevelStats { LevelName = "Neural Network Nebula" };
        }

        protected override void CreateLevel()
        {
            _playerSpawnPoint = new Vector2(100, 500);
            _levelBounds = new Rectangle(0, 0, 3500, 1080);

            CreatePlatforms();
            CreateKnowledgeItems();
            CreateEnemies();
            CreateDoors();
        }

        private void CreatePlatforms()
        {
            // Нейро-платформы (появляются и исчезают) / Neural platforms (appear and disappear)
            _neuralPlatforms.Add(new NeuralPlatform(new Vector2(300, 500), 150, 30)
            { TriggerRange = 200f, Delay = 0.5f });

            _neuralPlatforms.Add(new NeuralPlatform(new Vector2(550, 450), 150, 30)
            { TriggerRange = 200f, Delay = 0.5f });

            _neuralPlatforms.Add(new NeuralPlatform(new Vector2(800, 400), 150, 30)
            { TriggerRange = 200f, Delay = 0.5f });

            // Платформы-веса / Weight platforms (триггеры)
            var weightPlatform1 = new WeightTriggerPlatform(new Vector2(1100, 500), 200, 40);
            weightPlatform1.OnActivate += () => ActivateNeuralPath(0);
            _platforms.Add(weightPlatform1);

            // Синапсы-платформы / Synapse platforms
            CreateSynapseChain(1400, 350, 5);

            // Активационные платформы / Activation platforms
            for (int i = 0; i < 3; i++)
            {
                _neuralPlatforms.Add(new NeuralPlatform(
                    new Vector2(1800 + i * 200, 300 - (i % 2) * 100), 120, 25)
                { TriggerRange = 150f, Delay = 0.3f, ActivationType = ActivationType.Sequential });
            }

            // Финальная платформа / Final platform
            var finalPlatform = new Platform(new Vector2(2800, 200), 400, 50, PlatformType.Solid);
            finalPlatform.TintColor = new Color(200, 50, 200);
            AddPlatform(finalPlatform);
        }

        private void CreateSynapseChain(float startX, float startY, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float x = startX + i * 120;
                float y = startY + (float)Math.Sin(i * 0.8) * 50;
                
                _neuralPlatforms.Add(new NeuralPlatform(new Vector2(x, y), 80, 20)
                { 
                    TriggerRange = 100f, 
                    Delay = 0.2f,
                    ActivationType = ActivationType.Chain,
                    ChainIndex = i
                });
            }
        }

        private void ActivateNeuralPath(int pathIndex)
        {
            // Активация пути нейронов / Activate neural path
            for (int i = 0; i < _neuralPlatforms.Count; i++)
            {
                if (i >= pathIndex * 3 && i < pathIndex * 3 + 3)
                {
                    _neuralPlatforms[i].ForceActivate();
                }
            }
        }

        private void CreateKnowledgeItems()
        {
            // ML/AI технологии / ML/AI technologies

            AddKnowledge(new KnowledgeItem(new Vector2(300, 450), KnowledgeType.NeuralNet,
                "TensorFlow", "Фреймворк от Google для машинного обучения.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(550, 400), KnowledgeType.NeuralNet,
                "PyTorch", "Фреймворк от Facebook. Динамические графы.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(800, 350), KnowledgeType.NeuralNet,
                "Keras", "Высокоуровневый API для нейросетей.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1100, 450), KnowledgeType.NeuralNet,
                "Scikit-learn", "ML библиотека для Python. Классика.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1400, 300), KnowledgeType.NeuralNet,
                "OpenCV", "Компьютерное зрение. Распознавание образов.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(1800, 250), KnowledgeType.NeuralNet,
                "NLP", "Обработка естественного языка. Transformers, BERT.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2200, 200), KnowledgeType.NeuralNet,
                "GPT", "Генеративные трансформеры. LLM модели.")
            { Width = 32, Height = 32 });

            AddKnowledge(new KnowledgeItem(new Vector2(2800, 150), KnowledgeType.NeuralNet,
                "Neural Architecture", "Построение и оптимизация архитектур нейросетей.")
            { Width = 32, Height = 32 });

            _stats.KnowledgeTotal = _knowledgeItems.Count;
        }

        private void CreateEnemies()
        {
            // Нейроны-призраки / Ghost neurons
            for (int i = 0; i < 5; i++)
            {
                var neuron = new NeuronEnemy(new Vector2(500 + i * 400, 300 + (i % 2) * 200));
                neuron.SetPatrolParams(new Vector2(100, 80), 1f + i * 0.2f);
                AddEnemy(neuron);
            }

            // Стационарные нейроны-ловушки / Stationary trap neurons
            AddEnemy(new NeuronEnemy(new Vector2(1100, 400)) { IsTrap = true });
            AddEnemy(new NeuronEnemy(new Vector2(2000, 250)) { IsTrap = true });
        }

        private void CreateDoors()
        {
            // Выход на бонусный уровень / Exit to bonus level
            var exitDoor = new Door(new Vector2(3000, 100), DoorType.Exit, "bonus_junior");
            exitDoor.Width = 64;
            exitDoor.Height = 96;
            _doors.Add(exitDoor);
            exitDoor.OnEnter += OnDoorEnter;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _networkTimer += delta;

            // Обновление нейро-платформ / Update neural platforms
            foreach (var platform in _neuralPlatforms)
            {
                platform.Update(gameTime, _player?.Position);
            }

            // Отрисовка связей между нейронами / Draw connections between neurons
            DrawNeuralConnections();
        }

        private void DrawNeuralConnections()
        {
            // Визуализация нейронных связей / Visualize neural connections
            // Будет реализовано в Draw
        }

        protected override void LoadLevelMusic()
        {
            // Ambient electronic / neural soundscape
        }

        protected override void DrawLevelBackground(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Глубокий фиолетовый фон / Deep purple background
            Rectangle screen = new Rectangle(0, 0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            
            // Градиент / Gradient
            for (int y = 0; y < screen.Height; y += 10)
            {
                float t = (float)y / screen.Height;
                Color color = Color.Lerp(new Color(30, 10, 50), new Color(60, 20, 80), t);
                Rectangle strip = new Rectangle(0, y, screen.Width, 10);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, strip, color);
            }

            // Нейронные связи на фоне / Neural connections in background
            DrawBackgroundNeuralNetwork(spriteBatch);

            // Бинарный снег / Binary snow
            DrawBinarySnow(spriteBatch, gameTime);
        }

        private void DrawBackgroundNeuralNetwork(SpriteBatch spriteBatch)
        {
            int nodeCount = 20;
            Vector2[] nodes = new Vector2[nodeCount];
            
            // Генерация узлов / Generate nodes
            for (int i = 0; i < nodeCount; i++)
            {
                float x = (i % 5) * 400 + (float)Math.Sin(_networkTimer * 0.5 + i) * 50;
                float y = (i / 5) * 250 + (float)Math.Cos(_networkTimer * 0.3 + i) * 30;
                nodes[i] = new Vector2(x - Camera.Position.X * 0.3f, y);
            }

            // Отрисовка связей / Draw connections
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = i + 1; j < nodeCount; j++)
                {
                    float dist = Vector2.Distance(nodes[i], nodes[j]);
                    if (dist < 200)
                    {
                        float alpha = 1f - dist / 200f;
                        GraphicsUtils.DrawLine(spriteBatch, nodes[i], nodes[j], 
                            new Color(150, 50, 200, (int)(alpha * 100)), 2);
                    }
                }
            }

            // Отрисовка узлов / Draw nodes
            for (int i = 0; i < nodeCount; i++)
            {
                bool pulse = (DateTime.Now.Millisecond + i * 100) % 1000 < 300;
                Color nodeColor = pulse ? new Color(200, 100, 255) : new Color(100, 50, 150);
                Rectangle nodeRect = new Rectangle(
                    (int)nodes[i].X - 5,
                    (int)nodes[i].Y - 5,
                    10, 10);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, nodeRect, nodeColor);
            }
        }

        private void DrawBinarySnow(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Бинарный код падает как снег / Binary code falls like snow
            SpriteFont font = UIManager.DefaultFont;
            if (font == null) return;

            Random random = new Random((int)(_networkTimer * 1000));
            for (int i = 0; i < 30; i++)
            {
                float x = (i * 73) % GraphicsDevice.Viewport.Width;
                float y = (_networkTimer * 50 + i * 100) % GraphicsDevice.Viewport.Height;
                string binary = random.Next(0, 2) == 0 ? "0" : "1";
                spriteBatch.DrawString(font, binary, new Vector2(x, y), 
                    new Color(0, 255, 100, 100));
            }
        }

        private void OnDoorEnter(Entities.Objects.Door door, string targetLevelId)
        {
            System.Diagnostics.Debug.WriteLine($"Entering door to: {targetLevelId}");
        }
    }

    // Нейро-платформа (появляется/исчезает) / Neural platform (appears/disappears)
    public class NeuralPlatform
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float TriggerRange { get; set; } = 200f;
        public float Delay { get; set; } = 0.5f;
        public ActivationType ActivationType { get; set; } = ActivationType.Proximity;
        public int ChainIndex { get; set; } = 0;

        private bool _isActive = false;
        private float _activeTimer = 0f;
        private float _visibleTimer = 0f;
        private const float VisibleDuration = 3f;

        public NeuralPlatform(Vector2 position, float width, float height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public void Update(GameTime gameTime, Vector2? playerPosition = null)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isActive)
            {
                _activeTimer += delta;
                if (_activeTimer >= VisibleDuration)
                {
                    _isActive = false;
                    _visibleTimer = Delay;
                }
            }
            else
            {
                _visibleTimer -= delta;
                
                // Проверка активации / Check activation
                if (playerPosition.HasValue && _visibleTimer <= 0)
                {
                    float dist = Vector2.Distance(Position, playerPosition.Value);
                    if (dist < TriggerRange)
                    {
                        _isActive = true;
                        _activeTimer = 0f;
                    }
                }
            }
        }

        public void ForceActivate()
        {
            _isActive = true;
            _activeTimer = 0f;
        }

        public bool IsActive => _isActive;
        public float Alpha => _isActive ? 1f : MathHelper.Clamp(_visibleTimer / Delay, 0f, 0.3f);

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alpha < 0.1f) return;

            Color color = _isActive ? 
                new Color(200, 100, 255, 255) : 
                new Color(100, 50, 150, (int)(Alpha * 100));

            Rectangle rect = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, rect, color);

            // Неоновая обводка / Neon border
            if (_isActive)
            {
                GraphicsUtils.DrawRectangle(spriteBatch, rect, Color.Cyan, 2);
            }
        }

        public Rectangle Bounds => new Rectangle(
            (int)(Position.X - Width / 2),
            (int)(Position.Y - Height / 2),
            (int)Width,
            (int)Height);
    }

    public enum ActivationType
    {
        Proximity,    // Активация по близости / Proximity activation
        Chain,        // Цепочная активация / Chain activation
        Sequential    // Последовательная активация / Sequential activation
    }

    // Платформа-триггер веса / Weight trigger platform
    public class WeightTriggerPlatform : Platform
    {
        private bool _isActivated = false;
        private float _weightTimer = 0f;
        private const float ActivationTime = 1f;

        public event Action OnActivate;

        public WeightTriggerPlatform(Vector2 position, float width, float height)
            : base(position, width, height, PlatformType.Solid)
        {
            TintColor = new Color(100, 50, 150);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Проверка веса игрока на платформе / Check player weight on platform
            // (упрощённо - всегда активирована для демо)
            if (!_isActivated)
            {
                _weightTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_weightTimer >= ActivationTime)
                {
                    _isActivated = true;
                    OnActivate?.Invoke();
                    TintColor = Color.Green;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            // Индикатор активации / Activation indicator
            float progress = _weightTimer / ActivationTime;
            if (!_isActivated && progress > 0)
            {
                Rectangle indicator = new Rectangle(
                    (int)(Position.X - Width / 2),
                    (int)(Position.Y - Height / 2 - 10),
                    (int)(Width * progress),
                    5);
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, indicator, Color.Lime);
            }
        }
    }

    // Нейрон-враг / Neuron enemy
    public class NeuronEnemy : Enemy
    {
        public bool IsTrap { get; set; } = false;
        private float _pulseTimer = 0f;

        public NeuronEnemy(Vector2 position)
            : base(position, EnemyType.Neuron, EnemyBehavior.Flying)
        {
            Width = 40;
            Height = 40;
        }

        public NeuronEnemy(Vector2 position, EnemyBehavior behavior)
            : base(position, EnemyType.Neuron, behavior)
        {
            Width = 40;
            Height = 40;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _pulseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 3f;
        }

        protected override void DrawPlaceholder(SpriteBatch spriteBatch)
        {
            // Тело нейрона / Neuron body
            float pulse = (float)Math.Sin(_pulseTimer) * 0.3f + 0.7f;
            Color bodyColor = IsTrap ? 
                new Color(255, 50, 100, (int)(200 * pulse)) : 
                new Color(200, 100, 255, (int)(200 * pulse));

            // Круглое тело / Round body
            for (float r = Width / 2; r > 0; r -= 3)
            {
                float alpha = r / (Width / 2);
                Rectangle circle = new Rectangle(
                    (int)(Position.X - r),
                    (int)(Position.Y - r),
                    (int)(r * 2),
                    (int)(r * 2));
                spriteBatch.Draw(GraphicsUtils.WhiteTexture, circle, 
                    new Color(bodyColor.R, bodyColor.G, bodyColor.B, (int)(bodyColor.A * alpha)));
            }

            // Отростки-аксоны / Axon branches
            for (int i = 0; i < 6; i++)
            {
                float angle = i * MathHelper.Pi / 3 + _pulseTimer * 0.5f;
                Vector2 end = Position + new Vector2(
                    (float)Math.Cos(angle) * 30,
                    (float)Math.Sin(angle) * 30);
                GraphicsUtils.DrawLine(spriteBatch, Position, end, bodyColor, 3);
            }

            // Ядро / Nucleus
            Rectangle nucleus = new Rectangle(
                (int)(Position.X - 8),
                (int)(Position.Y - 8),
                16, 16);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, nucleus, 
                IsTrap ? Color.Red : Color.Cyan);
        }
    }
}
