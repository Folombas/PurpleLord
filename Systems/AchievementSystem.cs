// ============================================================================
// AchievementSystem.cs - Система достижений / Achievement system
// Отслеживание прогресса и наград игрока
// Track player progress and rewards
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PurpleLordPlatformer.Systems
{
    public enum AchievementType
    {
        FirstBlood,           // Первый убитый враг
        KnowledgeSeeker,      // Собрано 10 знаний
        FocusMaster,          // Использован фокус 50 раз
        SpeedRunner,          // Быстрое прохождение
        Collector,            // Собрано 80% знаний на уровне
        Perfectionist,        // Без смертей на уровне
        Explorer,             // Посещены все уровни
        TechMaster,           // Собраны все технологии
        DoubleJumpMaster,     // 100 двойных прыжков
        BossSlayer            // Убит босс
    }

    public class Achievement
    {
        public AchievementType Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime UnlockedDate { get; set; }
        public int Progress { get; set; }
        public int RequiredProgress { get; set; }
        public int Points { get; set; }

        public Achievement(AchievementType type, string id, string name, 
            string description, int requiredProgress = 1, int points = 10)
        {
            Type = type;
            Id = id;
            Name = name;
            Description = description;
            RequiredProgress = requiredProgress;
            Points = points;
            IsUnlocked = false;
            Progress = 0;
        }

        public void AddProgress(int amount = 1)
        {
            if (IsUnlocked) return;

            Progress += amount;
            if (Progress >= RequiredProgress)
            {
                Unlock();
            }
        }

        public void Unlock()
        {
            if (!IsUnlocked)
            {
                IsUnlocked = true;
                UnlockedDate = DateTime.Now;
                OnUnlock?.Invoke(this);
            }
        }

        public event Action<Achievement> OnUnlock;

        public float ProgressPercent => RequiredProgress > 0 
            ? (float)Progress / RequiredProgress * 100f : 0f;
    }

    public class AchievementSystem
    {
        private Dictionary<AchievementType, Achievement> _achievements 
            = new Dictionary<AchievementType, Achievement>();
        private int _totalPoints = 0;

        public AchievementSystem()
        {
            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            // Первый убитый враг / First enemy killed
            AddAchievement(new Achievement(
                AchievementType.FirstBlood,
                "first_blood",
                "First Blood",
                "Убейте первого врага",
                1, 10));

            // Собиратель знаний / Knowledge collector
            AddAchievement(new Achievement(
                AchievementType.KnowledgeSeeker,
                "knowledge_seeker",
                "Knowledge Seeker",
                "Соберите 10 предметов знаний",
                10, 20));

            // Мастер фокуса / Focus master
            AddAchievement(new Achievement(
                AchievementType.FocusMaster,
                "focus_master",
                "Focus Master",
                "Используйте режим фокуса 50 раз",
                50, 15));

            // Коллекционер / Collector
            AddAchievement(new Achievement(
                AchievementType.Collector,
                "collector",
                "Collector",
                "Соберите 80% знаний на уровне",
                80, 25));

            // Исследователь / Explorer
            AddAchievement(new Achievement(
                AchievementType.Explorer,
                "explorer",
                "Explorer",
                "Посетите все 4 уровня",
                4, 30));

            // Убийца боссов / Boss slayer
            AddAchievement(new Achievement(
                AchievementType.BossSlayer,
                "boss_slayer",
                "Boss Slayer",
                "Победите босса",
                1, 50));

            // Мастер двойного прыжка / Double jump master
            AddAchievement(new Achievement(
                AchievementType.DoubleJumpMaster,
                "double_jump_master",
                "Double Jump Master",
                "Сделайте 100 двойных прыжков",
                100, 20));
        }

        private void AddAchievement(Achievement achievement)
        {
            _achievements[achievement.Type] = achievement;
            achievement.OnUnlock += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            _totalPoints += achievement.Points;
            // TODO: Показать уведомление / Show notification
            System.Diagnostics.Debug.WriteLine(
                $"Achievement unlocked: {achievement.Name} (+{achievement.Points} points)");
        }

        public void UpdateProgress(AchievementType type, int amount = 1)
        {
            if (_achievements.ContainsKey(type))
            {
                _achievements[type].AddProgress(amount);
            }
        }

        public Achievement GetAchievement(AchievementType type)
        {
            return _achievements.ContainsKey(type) ? _achievements[type] : null;
        }

        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(_achievements.Values);
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            var result = new List<Achievement>();
            foreach (var achievement in _achievements.Values)
            {
                if (achievement.IsUnlocked)
                    result.Add(achievement);
            }
            return result;
        }

        public int TotalPoints => _totalPoints;
        public int UnlockedCount => GetUnlockedAchievements().Count;
        public int TotalCount => _achievements.Count;
        public float CompletionPercent => _achievements.Count > 0
            ? (float)UnlockedCount / _achievements.Count * 100f : 0f;

        public void Save()
        {
            // TODO: Сохранение в файл / Save to file
        }

        public void Load()
        {
            // TODO: Загрузка из файла / Load from file
        }
    }

    // Трекер статистики для достижений / Statistics tracker for achievements
    public class StatsTracker
    {
        private Dictionary<string, int> _stats = new Dictionary<string, int>();
        private AchievementSystem _achievementSystem;

        public StatsTracker(AchievementSystem achievementSystem)
        {
            _achievementSystem = achievementSystem;
        }

        public void Increment(string statName, int amount = 1)
        {
            if (!_stats.ContainsKey(statName))
                _stats[statName] = 0;
            
            _stats[statName] += amount;

            // Проверка достижений / Check achievements
            CheckAchievements(statName, _stats[statName]);
        }

        public int GetStat(string statName)
        {
            return _stats.ContainsKey(statName) ? _stats[statName] : 0;
        }

        private void CheckAchievements(string statName, int value)
        {
            switch (statName)
            {
                case "EnemiesKilled":
                    if (value >= 1)
                        _achievementSystem.UpdateProgress(AchievementType.FirstBlood);
                    break;
                case "KnowledgeCollected":
                    if (value >= 10)
                        _achievementSystem.UpdateProgress(AchievementType.KnowledgeSeeker);
                    break;
                case "FocusUsed":
                    if (value >= 50)
                        _achievementSystem.UpdateProgress(AchievementType.FocusMaster);
                    break;
                case "DoubleJumps":
                    if (value >= 100)
                        _achievementSystem.UpdateProgress(AchievementType.DoubleJumpMaster);
                    break;
                case "BossesDefeated":
                    if (value >= 1)
                        _achievementSystem.UpdateProgress(AchievementType.BossSlayer);
                    break;
            }
        }
    }
}
