// ============================================================================
// NPC.cs - Непроигрываемый персонаж / Non-Playable Character
// Персонажи для диалогов и квестов
// Characters for dialogues and quests
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PurpleLordPlatformer.Entities;
using PurpleLordPlatformer.Entities.Player;
using PurpleLordPlatformer.Systems;

namespace PurpleLordPlatformer.Entities.Objects
{
    public enum NPCType
    {
        Mentor,       // Наставник / Mentor
        Vendor,       // Торговец / Vendor
        QuestGiver,   // Дающий квесты / Quest giver
        Informant,    // Информатор / Informant
        Random        // Случайный NPC / Random NPC
    }

    public class NPC : GameObject
    {
        protected NPCType _type;
        protected string _npcName = "";
        protected Texture2D _texture;
        protected Color _tintColor = Color.White;
        
        private List<DialogueLine> _dialogues = new List<DialogueLine>();
        private bool _isInteracting = false;
        private float _idleTimer = 0f;
        private Vector2 _idleStartPosition;

        public event Action<NPC> OnInteractionStart;
        public event Action<NPC> OnInteractionEnd;

        public NPC(Vector2 position, NPCType type, string name) : base(position)
        {
            _type = type;
            _npcName = name;
            _idleStartPosition = position;
            Width = 40;
            Height = 60;
            Tag = "NPC";

            InitializeDialogues();
        }

        protected virtual void InitializeDialogues()
        {
            // Переопределяется в подклассах / Overridden in subclasses
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _idleTimer += delta;

            // Idle анимация / Idle animation
            if (!_isInteracting)
            {
                Position.Y = _idleStartPosition.Y + (float)Math.Sin(_idleTimer * 2) * 5;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, Position, null, _tintColor, 0f,
                    new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
            }
            else
            {
                DrawNPCPlaceholder(spriteBatch);
            }

            // Имя NPC / NPC name
            if (UIManager.DefaultFont != null)
            {
                Vector2 nameSize = UIManager.DefaultFont.MeasureString(_npcName);
                spriteBatch.DrawString(UIManager.DefaultFont, _npcName,
                    new Vector2(Position.X - nameSize.X / 2, Position.Y - Height / 2 - 25),
                    Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
            }

            // Индикатор взаимодействия / Interaction indicator
            if (!_isInteracting)
            {
                DrawInteractionIndicator(spriteBatch);
            }
        }

        protected virtual void DrawNPCPlaceholder(SpriteBatch spriteBatch)
        {
            // Тело / Body
            Rectangle body = new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height / 2),
                (int)Width,
                (int)Height);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, body, GetTypeColor());

            // Голова / Head
            Rectangle head = new Rectangle(
                (int)(Position.X - 15),
                (int)(Position.Y - Height / 2 - 20),
                30, 30);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, head, Color.PeachPuff);

            // Глаза / Eyes
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(Position.X - 8), (int)(Position.Y - Height / 2 - 15), 5, 5),
                Color.Black);
            spriteBatch.Draw(GraphicsUtils.WhiteTexture,
                new Rectangle((int)(Position.X + 3), (int)(Position.Y - Height / 2 - 15), 5, 5),
                Color.Black);

            // Тип NPC / NPC type indicator
            DrawTypeIndicator(spriteBatch);
        }

        protected Color GetTypeColor()
        {
            switch (_type)
            {
                case NPCType.Mentor: return new Color(100, 100, 255); // Blue
                case NPCType.Vendor: return new Color(255, 200, 50);   // Gold
                case NPCType.QuestGiver: return new Color(255, 100, 100); // Red
                case NPCType.Informant: return new Color(100, 255, 100);  // Green
                default: return Color.White;
            }
        }

        protected virtual void DrawTypeIndicator(SpriteBatch spriteBatch)
        {
            // Значок над головой / Icon above head
            string icon = _type switch
            {
                NPCType.Mentor => "?",
                NPCType.Vendor => "$",
                NPCType.QuestGiver => "!",
                NPCType.Informant => "i",
                _ => ""
            };

            if (!string.IsNullOrEmpty(icon) && UIManager.DefaultFont != null)
            {
                Vector2 iconSize = UIManager.DefaultFont.MeasureString(icon);
                spriteBatch.DrawString(UIManager.DefaultFont, icon,
                    new Vector2(Position.X - iconSize.X / 2, 
                        Position.Y - Height / 2 - 45),
                    Color.Yellow, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }
        }

        private void DrawInteractionIndicator(SpriteBatch spriteBatch)
        {
            float pulse = (float)Math.Sin(_idleTimer * 5) * 0.5f + 0.5f;
            
            // Круг вокруг NPC / Circle around NPC
            int segments = 16;
            float radius = Width;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * MathHelper.Pi * 2 / segments;
                float angle2 = (i + 1) * MathHelper.Pi * 2 / segments;

                Vector2 pos1 = Position + new Vector2(
                    (float)Math.Cos(angle1) * radius,
                    (float)Math.Sin(angle1) * radius);
                Vector2 pos2 = Position + new Vector2(
                    (float)Math.Cos(angle2) * radius,
                    (float)Math.Sin(angle2) * radius);

                Color color = new Color(255, 255, 255, (int)(pulse * 150));
                GraphicsUtils.DrawLine(spriteBatch, pos1, pos2, color, 2);
            }
        }

        public virtual void Interact(Player player)
        {
            if (_isInteracting) return;

            _isInteracting = true;
            OnInteractionStart?.Invoke(this);
        }

        public virtual void EndInteraction()
        {
            _isInteracting = false;
            OnInteractionEnd?.Invoke(this);
        }

        public void AddDialogue(DialogueLine line)
        {
            _dialogues.Add(line);
        }

        public void SetDialogues(List<DialogueLine> dialogues)
        {
            _dialogues = dialogues;
        }

        public List<DialogueLine> GetDialogues() => _dialogues;

        public bool IsInteracting => _isInteracting;
        public NPCType Type => _type;
        public string Name => _npcName;

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
    }

    // Конкретные NPC / Specific NPCs
    public class MentorNPC : NPC
    {
        public MentorNPC(Vector2 position) : base(position, NPCType.Mentor, "Мудрый Разработчик")
        {
        }

        protected override void InitializeDialogues()
        {
            _dialogues = new List<DialogueLine>
            {
                new DialogueLine("Мудрый Разработчик", "Приветствую, Фиолетовый Лорд!"),
                new DialogueLine("Мудрый Разработчик", "Вижу, ты ищешь свой путь в IT..."),
                new DialogueLine("Мудрый Разработчик", "Помни: важно не знать всё, а выбрать одно и изучить глубоко."),
                new DialogueLine("Player", "Но как выбрать?"),
                new DialogueLine("Мудрый Разработчик", "Слушай своё сердце. То, что приносит радость — твой путь.")
            };
        }
    }

    public class VendorNPC : NPC
    {
        public VendorNPC(Vector2 position) : base(position, NPCType.Vendor, "Торговец Знаниями")
        {
        }

        protected override void InitializeDialogues()
        {
            _dialogues = new List<DialogueLine>
            {
                new DialogueLine("Торговец Знаниями", "Привет! У меня есть редкие технологии!"),
                new DialogueLine("Торговец Знаниями", "Но они не продаются за деньги..."),
                new DialogueLine("Торговец Знаниями", "Принеси мне знания из других миров, и я поделюсь своими.")
            };
        }
    }
}
