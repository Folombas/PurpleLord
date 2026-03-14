// ============================================================================
// SaveManager.cs - Менеджер сохранений / Save manager
// Сохранение и загрузка прогресса игры
// Save and load game progress
// ============================================================================

using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PurpleLordPlatformer.Core;
using PurpleLordPlatformer.Systems;

namespace PurpleLordPlatformer.Managers
{
    public class SaveData
    {
        // Основной прогресс / Main progress
        public string CurrentLevelId { get; set; } = "main_menu";
        public int TotalPlayTimeSeconds { get; set; }
        
        // Статистика по уровням / Level statistics
        public Dictionary<string, LevelSaveData> LevelData { get; set; } 
            = new Dictionary<string, LevelSaveData>();
        
        // Открытые технологии / Unlocked technologies
        public List<string> UnlockedTechnologies { get; set; } 
            = new List<string>();
        
        // Достижения / Achievements
        public List<string> UnlockedAchievements { get; set; } 
            = new List<string>();
        
        // Статистика / Statistics
        public StatisticsData Statistics { get; set; } = new StatisticsData();
        
        // Настройки / Settings
        public SettingsData Settings { get; set; } = new SettingsData();
        
        // Дата последнего сохранения / Last save date
        public DateTime LastSaveDate { get; set; } = DateTime.Now;
    }

    public class LevelSaveData
    {
        public string LevelId { get; set; }
        public bool IsCompleted { get; set; }
        public int KnowledgeCollected { get; set; }
        public int KnowledgeTotal { get; set; }
        public int Deaths { get; set; }
        public int TimeSpentSeconds { get; set; }
        public float BestCompletionPercent { get; set; }
        public Vector2 SavePosition { get; set; } // Позиция чекпоинта
    }

    public class StatisticsData
    {
        public int TotalDeaths { get; set; }
        public int TotalJumps { get; set; }
        public int TotalDoubleJumps { get; set; }
        public int EnemiesKilled { get; set; }
        public int KnowledgeCollected { get; set; }
        public int FocusActivations { get; set; }
        public int BossesDefeated { get; set; }
        public int LevelsCompleted { get; set; }
        public int TotalPlayTimeSeconds { get; set; }
    }

    public class SettingsData
    {
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 0.7f;
        public float SfxVolume { get; set; } = 1f;
        public bool Fullscreen { get; set; } = true;
        public int ScreenWidth { get; set; } = 1920;
        public int ScreenHeight { get; set; } = 1080;
        public string Language { get; set; } = "ru";
    }

    public class SaveManager
    {
        private const string SaveFileName = "savegame.json";
        private string SavePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PurpleLordPlatformer",
            SaveFileName);

        private SaveData _currentSave;

        public SaveManager()
        {
            _currentSave = new SaveData();
        }

        public void NewGame()
        {
            _currentSave = new SaveData();
            _currentSave.CurrentLevelId = "level1_frontend";
        }

        public void Save()
        {
            try
            {
                // Создание директории / Create directory
                string? directory = Path.GetDirectoryName(SavePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Сериализация / Serialization
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };
                
                string json = JsonSerializer.Serialize(_currentSave, options);
                File.WriteAllText(SavePath, json);
                
                System.Diagnostics.Debug.WriteLine($"Game saved to: {SavePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            }
        }

        public bool Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    System.Diagnostics.Debug.WriteLine("No save file found");
                    return false;
                }

                string json = File.ReadAllText(SavePath);
                var loadedSave = JsonSerializer.Deserialize<SaveData>(json);
                
                if (loadedSave != null)
                {
                    _currentSave = loadedSave;
                    System.Diagnostics.Debug.WriteLine($"Game loaded from: {SavePath}");
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
                return false;
            }
        }

        public bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                _currentSave = new SaveData();
            }
        }

        // Методы для обновления данных / Methods for updating data
        
        public void SetCurrentLevel(string levelId)
        {
            _currentSave.CurrentLevelId = levelId;
        }

        public void UpdateLevelData(string levelId, LevelSaveData data)
        {
            if (!_currentSave.LevelData.ContainsKey(levelId))
            {
                _currentSave.LevelData[levelId] = data;
            }
            else
            {
                var existing = _currentSave.LevelData[levelId];
                if (data.BestCompletionPercent > existing.BestCompletionPercent)
                {
                    _currentSave.LevelData[levelId] = data;
                }
            }
        }

        public void UnlockTechnology(string techId)
        {
            if (!_currentSave.UnlockedTechnologies.Contains(techId))
            {
                _currentSave.UnlockedTechnologies.Add(techId);
            }
        }

        public void UpdateStatistics(Action<StatisticsData> updateAction)
        {
            updateAction(_currentSave.Statistics);
        }

        public void AddPlayTime(int seconds)
        {
            _currentSave.TotalPlayTimeSeconds += seconds;
            _currentSave.Statistics.TotalPlayTimeSeconds += seconds;
        }

        // Геттеры / Getters
        
        public SaveData CurrentSave => _currentSave;
        public StatisticsData Statistics => _currentSave.Statistics;
        public SettingsData Settings => _currentSave.Settings;
        public string CurrentLevelId => _currentSave.CurrentLevelId;
        
        public LevelSaveData? GetLevelData(string levelId)
        {
            return _currentSave.LevelData.ContainsKey(levelId) 
                ? _currentSave.LevelData[levelId] 
                : null;
        }

        public bool IsLevelCompleted(string levelId)
        {
            return _currentSave.LevelData.ContainsKey(levelId) && 
                   _currentSave.LevelData[levelId].IsCompleted;
        }

        public bool IsTechnologyUnlocked(string techId)
        {
            return _currentSave.UnlockedTechnologies.Contains(techId);
        }
    }
}
