// ============================================================================
// QuestSystem.cs - Система заданий / Quest system
// Управление квестами и задачами игрока
// Manage player quests and tasks
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PurpleLordPlatformer.Systems
{
    public enum QuestType
    {
        Collect,        // Собрать предметы / Collect items
        Kill,           // Убить врагов / Kill enemies
        Reach,          // Достичь места / Reach location
        Talk,           // Поговорить с NPC / Talk to NPC
        Explore,        // Исследовать область / Explore area
        Puzzle          // Решить головоломку / Solve puzzle
    }

    public enum QuestState
    {
        Available,      // Доступен для взятия / Available to take
        Active,         // Выполняется / In progress
        Completed,      // Завершён / Completed
        Failed          // Провален / Failed
    }

    public class Quest
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public QuestType Type { get; set; }
        public QuestState State { get; set; } = QuestState.Available;
        
        public int CurrentProgress { get; set; }
        public int RequiredProgress { get; set; }
        
        public string TargetId { get; set; } = ""; // ID цели (предмет, враг, NPC)
        public string TargetLocation { get; set; } = ""; // Целевая локация
        
        public List<QuestReward> Rewards { get; set; } = new List<QuestReward>();
        
        public string GiverNPC { get; set; } = ""; // Кто даёт квест
        
        // Для отслеживания в HUD / For HUD tracking
        public bool IsTracked { get; set; } = true;

        public Quest(QuestType type, string title, string description, int required)
        {
            Type = type;
            Title = title;
            Description = description;
            RequiredProgress = required;
            CurrentProgress = 0;
        }

        public void AddProgress(int amount = 1)
        {
            if (State != QuestState.Active) return;

            CurrentProgress += amount;
            if (CurrentProgress >= RequiredProgress)
            {
                Complete();
            }
        }

        public void Start()
        {
            State = QuestState.Active;
            OnQuestStarted?.Invoke(this);
        }

        public void Complete()
        {
            State = QuestState.Completed;
            OnQuestCompleted?.Invoke(this);
        }

        public void Fail()
        {
            State = QuestState.Failed;
            OnQuestFailed?.Invoke(this);
        }

        public float ProgressPercent => RequiredProgress > 0 
            ? (float)CurrentProgress / RequiredProgress * 100f : 0f;

        public event Action<Quest> OnQuestStarted;
        public event Action<Quest> OnQuestCompleted;
        public event Action<Quest> OnQuestFailed;

        public string GetProgressString()
        {
            return Type switch
            {
                QuestType.Collect => $"{CurrentProgress}/{RequiredProgress} собрано",
                QuestType.Kill => $"{CurrentProgress}/{RequiredProgress} убито",
                QuestType.Reach => CurrentProgress > 0 ? "Достигнуто" : "В пути",
                QuestType.Talk => CurrentProgress > 0 ? "Готово" : "Не начато",
                _ => $"{CurrentProgress}/{RequiredProgress}"
            };
        }
    }

    public class QuestReward
    {
        public string Type { get; set; } = "knowledge"; // knowledge, unlock, etc.
        public string Id { get; set; } = "";
        public int Amount { get; set; } = 1;
        public string Description { get; set; } = "";
    }

    public class QuestSystem
    {
        private List<Quest> _quests = new List<Quest>();
        private List<Quest> _completedQuests = new List<Quest>();

        public event Action<Quest> OnQuestAdded;
        public event Action<Quest> OnQuestUpdated;
        public event Action<Quest> OnQuestCompleted;

        public void AddQuest(Quest quest)
        {
            _quests.Add(quest);
            quest.OnQuestCompleted += HandleQuestCompleted;
            OnQuestAdded?.Invoke(quest);
        }

        private void HandleQuestCompleted(Quest quest)
        {
            _quests.Remove(quest);
            _completedQuests.Add(quest);
            OnQuestCompleted?.Invoke(quest);
        }

        public void UpdateQuestProgress(string questId, int amount = 1)
        {
            var quest = GetQuest(questId);
            if (quest != null)
            {
                quest.AddProgress(amount);
                OnQuestUpdated?.Invoke(quest);
            }
        }

        public void UpdateQuestProgress(QuestType type, int amount = 1)
        {
            foreach (var quest in _quests)
            {
                if (quest.Type == type && quest.State == QuestState.Active)
                {
                    quest.AddProgress(amount);
                    OnQuestUpdated?.Invoke(quest);
                }
            }
        }

        public void StartQuest(string questId)
        {
            var quest = GetQuest(questId);
            if (quest != null && quest.State == QuestState.Available)
            {
                quest.Start();
            }
        }

        public Quest? GetQuest(string questId)
        {
            return _quests.Find(q => q.Id == questId);
        }

        public List<Quest> GetActiveQuests()
        {
            return _quests.FindAll(q => q.State == QuestState.Active);
        }

        public List<Quest> GetAvailableQuests()
        {
            return _quests.FindAll(q => q.State == QuestState.Available);
        }

        public List<Quest> GetCompletedQuests()
        {
            return _completedQuests;
        }

        public Quest? GetTrackedQuest()
        {
            foreach (var quest in _quests)
            {
                if (quest.IsTracked && quest.State == QuestState.Active)
                    return quest;
            }
            return null;
        }

        public void SetTrackedQuest(string questId)
        {
            foreach (var quest in _quests)
            {
                quest.IsTracked = (quest.Id == questId);
            }
        }
    }

    // Пресеты квестов для уровней / Quest presets for levels
    public static class QuestPresets
    {
        public static Quest CreateFrontendQuest()
        {
            var quest = new Quest(QuestType.Collect, 
                "Основы Фронтенда",
                "Соберите базовые знания фронтенд-разработки",
                5);
            quest.Id = "frontend_basics";
            quest.TargetId = "knowledge_frontend";
            quest.Rewards.Add(new QuestReward 
            { 
                Type = "knowledge", 
                Id = "css_mastery", 
                Amount = 1,
                Description = "CSS Mastery unlocked"
            });
            return quest;
        }

        public static Quest CreateBackendQuest()
        {
            var quest = new Quest(QuestType.Collect,
                "Путь Бэкендера",
                "Изучите основы серверной разработки",
                8);
            quest.Id = "backend_basics";
            quest.TargetId = "knowledge_backend";
            return quest;
        }

        public static Quest CreateNeuralQuest()
        {
            var quest = new Quest(QuestType.Explore,
                "Тайны Нейросетей",
                "Исследуйте туман нейронных связей",
                1);
            quest.Id = "neural_exploration";
            quest.TargetLocation = "neural_nebula_center";
            return quest;
        }

        public static Quest CreateKillBugsQuest()
        {
            var quest = new Quest(QuestType.Kill,
                "Охота на Баги",
                "Уничтожьте багов в Лесу Фронтенда",
                10);
            quest.Id = "bug_hunt";
            quest.TargetId = "enemy_bug";
            quest.Rewards.Add(new QuestReward
            {
                Type = "achievement",
                Id = "bug_slayer",
                Amount = 1,
                Description = "Bug Slayer achievement"
            });
            return quest;
        }
    }
}
