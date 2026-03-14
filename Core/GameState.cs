// ============================================================================
// GameState.cs - Состояние игры / Game state
// Хранит глобальные данные о прогрессе игрока
// Stores global player progress data
// ============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PurpleLordPlatformer.Core
{
    /// <summary>
    /// Перечисление состояний игры.
    /// Game state enumeration.
    /// </summary>
    public enum GameFlowState
    {
        Menu,           // Главное меню / Main menu
        Playing,        // Игра / Playing
        Paused,         // Пауза / Paused
        LevelComplete,  // Уровень завершён / Level complete
        GameOver        // Конец игры / Game over
    }

    /// <summary>
    /// Статистика прохождения уровня.
    /// Level completion statistics.
    /// </summary>
    public class LevelStats
    {
        public string LevelName { get; set; }
        public int KnowledgeCollected { get; set; }      // Собранные знания / Collected knowledge
        public int KnowledgeTotal { get; set; }          // Всего знаний / Total knowledge
        public float CompletionPercent => KnowledgeTotal > 0 
            ? (float)KnowledgeCollected / KnowledgeTotal * 100f 
            : 0f;
        public TimeSpan TimeSpent { get; set; }          // Время на уровне / Time spent
        public int Deaths { get; set; }                  // Количество смертей / Death count
        public bool IsCompleted { get; set; }            // Завершён ли уровень / Is level completed
    }

    /// <summary>
    /// Глобальное состояние игры.
    /// Global game state.
    /// </summary>
    public class GameState
    {
        // Текущее состояние потока игры / Current game flow state
        public GameFlowState FlowState { get; set; } = GameFlowState.Menu;
        
        // Текущий уровень / Current level
        public string CurrentLevelId { get; set; } = "menu";
        
        // Статистика по уровням / Level statistics
        public Dictionary<string, LevelStats> LevelStats { get; set; } 
            = new Dictionary<string, LevelStats>();
        
        // Открытые технологии (для повторного прохождения) / Unlocked technologies (for replay)
        public HashSet<string> UnlockedTechnologies { get; set; } 
            = new HashSet<string>();
        
        // Общий процент изученного / Overall study percentage
        public float OverallCompletionPercent
        {
            get
            {
                if (LevelStats.Count == 0) return 0f;
                
                float total = 0f;
                foreach (var stats in LevelStats.Values)
                {
                    total += stats.CompletionPercent;
                }
                return total / LevelStats.Count;
            }
        }
        
        // Уровень "расфокусировки" (накапливается при бездействии) / Focus loss level (accumulates during inactivity)
        public float FocusDrainLevel { get; set; } = 0f;
        
        // Множитель времени (для режима фокуса) / Time multiplier (for focus mode)
        public float TimeScale { get; set; } = 1f;
        
        // Текущая шкала осознанности / Current awareness meter
        public float AwarenessMeter { get; set; } = 100f;
        
        // Максимальная шкала осознанности / Maximum awareness meter
        public float MaxAwarenessMeter { get; set; } = 100f;

        /// <summary>
        /// Обновление состояния игры.
        /// Update game state.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Увеличение расфокусировки со временем / Increase focus drain over time
            if (FlowState == GameFlowState.Playing && TimeScale >= 1f)
            {
                FocusDrainLevel += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
                FocusDrainLevel = MathHelper.Clamp(FocusDrainLevel, 0f, 100f);
            }
        }

        /// <summary>
        /// Сброс расфокусировки.
        /// Reset focus drain.
        /// </summary>
        public void ResetFocusDrain()
        {
            FocusDrainLevel = 0f;
        }

        /// <summary>
        /// Получить или создать статистику уровня.
        /// Get or create level statistics.
        /// </summary>
        public LevelStats GetOrCreateLevelStats(string levelId)
        {
            if (!LevelStats.ContainsKey(levelId))
            {
                LevelStats[levelId] = new LevelStats { LevelName = levelId };
            }
            return LevelStats[levelId];
        }

        /// <summary>
        /// Сохранение прогресса.
        /// Save progress.
        /// </summary>
        public void SaveProgress()
        {
            // TODO: Реализовать сохранение в файл / TODO: Implement file saving
            System.Diagnostics.Debug.WriteLine("Progress saved!");
        }

        /// <summary>
        /// Загрузка прогресса.
        /// Load progress.
        /// </summary>
        public void LoadProgress()
        {
            // TODO: Реализовать загрузку из файла / TODO: Implement file loading
            System.Diagnostics.Debug.WriteLine("Progress loaded!");
        }
    }
}
