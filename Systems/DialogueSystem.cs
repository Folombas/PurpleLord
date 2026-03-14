// ============================================================================
// DialogueSystem.cs - Система диалогов / Dialogue system
// Отображение диалогов с NPC
// Display dialogues with NPCs
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PurpleLordPlatformer.Systems
{
    public class DialogueSystem
    {
        private Queue<DialogueLine> _dialogueQueue = new Queue<DialogueLine>();
        private DialogueLine? _currentLine;
        private bool _isTyping = false;
        private float _typewriterTimer = 0f;
        private float _typewriterSpeed = 0.05f; // секунды на символ
        private int _visibleCharCount = 0;
        private float _cursorTimer = 0f;
        private bool _cursorVisible = true;

        public event Action<DialogueLine?> OnDialogueStart;
        public event Action<DialogueLine?> OnDialogueEnd;
        public event Action OnLineComplete;

        public void StartDialogue(List<DialogueLine> lines)
        {
            _dialogueQueue.Clear();
            foreach (var line in lines)
            {
                _dialogueQueue.Enqueue(line);
            }
            StartNextLine();
        }

        public void StartDialogue(string speaker, string text, float duration = 0f)
        {
            var line = new DialogueLine(speaker, text, duration);
            StartDialogue(new List<DialogueLine> { line });
        }

        private void StartNextLine()
        {
            if (_dialogueQueue.Count > 0)
            {
                _currentLine = _dialogueQueue.Dequeue();
                _isTyping = true;
                _visibleCharCount = 0;
                _typewriterTimer = 0f;
                OnDialogueStart?.Invoke(_currentLine);
            }
            else
            {
                _currentLine = null;
                OnDialogueEnd?.Invoke(null);
            }
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isTyping && _currentLine.HasValue)
            {
                _typewriterTimer += delta;
                
                while (_typewriterTimer >= _typewriterSpeed && 
                       _visibleCharCount < _currentLine.Value.Text.Length)
                {
                    _visibleCharCount++;
                    _typewriterTimer -= _typewriterSpeed;
                }

                if (_visibleCharCount >= _currentLine.Value.Text.Length)
                {
                    _isTyping = false;
                    OnLineComplete?.Invoke();
                }
            }

            // Мигание курсора / Cursor blinking
            _cursorTimer += delta;
            if (_cursorTimer >= 0.5f)
            {
                _cursorTimer = 0f;
                _cursorVisible = !_cursorVisible;
            }
        }

        public void Advance()
        {
            if (_isTyping)
            {
                // Показать весь текст сразу / Show full text immediately
                _visibleCharCount = _currentLine?.Text.Length ?? 0;
                _isTyping = false;
                OnLineComplete?.Invoke();
            }
            else
            {
                StartNextLine();
            }
        }

        public void Skip()
        {
            _dialogueQueue.Clear();
            _currentLine = null;
            OnDialogueEnd?.Invoke(null);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Rectangle bounds)
        {
            if (!_currentLine.HasValue) return;

            var line = _currentLine.Value;

            // Фон диалога / Dialogue background
            spriteBatch.Draw(GraphicsUtils.WhiteTexture, bounds, new Color(0, 0, 0, 200));

            // Рамка / Border
            GraphicsUtils.DrawRectangle(spriteBatch, bounds, Color.White, 2);

            float padding = 20f;
            float y = bounds.Y + padding;

            // Имя говорящего / Speaker name
            if (!string.IsNullOrEmpty(line.Speaker))
            {
                Color speakerColor = GetSpeakerColor(line.Speaker);
                Vector2 speakerSize = font.MeasureString(line.Speaker);
                spriteBatch.DrawString(font, line.Speaker,
                    new Vector2(bounds.X + padding, y),
                    speakerColor, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
                y += speakerSize.Y + 5f;
            }

            // Текст / Text
            string visibleText = line.Text.Substring(0, _visibleCharCount);
            
            // Wrap text
            List<string> wrappedLines = WrapText(font, visibleText, bounds.Width - padding * 2);
            
            foreach (string textLine in wrappedLines)
            {
                spriteBatch.DrawString(font, textLine,
                    new Vector2(bounds.X + padding, y),
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                y += font.LineSpacing;
            }

            // Курсор продолжения / Continue cursor
            if (!_isTyping && _cursorVisible)
            {
                string cursor = "▼";
                Vector2 cursorSize = font.MeasureString(cursor);
                spriteBatch.DrawString(font, cursor,
                    new Vector2(bounds.Right - padding - cursorSize.X, 
                        bounds.Bottom - padding - cursorSize.Y),
                    Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        private Color GetSpeakerColor(string speaker)
        {
            // Разные цвета для разных говорящих / Different colors for different speakers
            switch (speaker.ToLower())
            {
                case "player": return Color.Cyan;
                case "narrator": return Color.Gray;
                case "mentor": return Color.Gold;
                case "bug": return Color.LightGreen;
                default: return Color.White;
            }
        }

        private List<string> WrapText(SpriteFont font, string text, float maxWidth)
        {
            var lines = new List<string>();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
                if (font.MeasureString(testLine).X <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    if (currentLine.Length > 0)
                        lines.Add(currentLine);
                    currentLine = word;
                }
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine);

            return lines;
        }

        public bool IsDialogueActive => _currentLine.HasValue || _dialogueQueue.Count > 0;
        public bool IsTyping => _isTyping;
        public DialogueLine? CurrentLine => _currentLine;
    }

    public struct DialogueLine
    {
        public string Speaker { get; set; }
        public string Text { get; set; }
        public float Duration { get; set; } // 0 = ждать ввода / wait for input

        public DialogueLine(string speaker, string text, float duration = 0f)
        {
            Speaker = speaker;
            Text = text;
            Duration = duration;
        }
    }

    // Примеры диалогов / Example dialogues
    public static class DialoguePresets
    {
        public static List<DialogueLine> GetIntroDialogue()
        {
            return new List<DialogueLine>
            {
                new DialogueLine("Narrator", "Вы — Фиолетовый Лорд, новичок в мире IT..."),
                new DialogueLine("Narrator", "Ваш путь начинается в Лесу Фронтенда."),
                new DialogueLine("Player", "С чего мне начать?"),
                new DialogueLine("Mentor", "Собирай знания, но помни: нельзя объять необъятное."),
                new DialogueLine("Mentor", "Выбери свой путь и следуй ему.")
            };
        }

        public static List<DialogueLine> GetLevelCompleteDialogue()
        {
            return new List<DialogueLine>
            {
                new DialogueLine("Narrator", "Уровень завершён!"),
                new DialogueLine("Narrator", "Но сколько знаний осталось несобранными..."),
                new DialogueLine("Player", "Я не могу собрать всё..."),
                new DialogueLine("Mentor", "Именно. Выбор — это часть пути.")
            };
        }
    }
}
