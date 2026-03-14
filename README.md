<<<<<<< HEAD
# PurpleLord
=======
# Purple Lord: Path of Choices | Фиолетовый Лорд: Путь Выбора

**2D-платформер на MonoGame о выборе пути в IT-технологиях**

[![GitHub](https://img.shields.io/github/license/Folombas/PurpleLord)](LICENSE)
[![MonoGame](https://img.shields.io/badge/MonoGame-3.8-purple)](https://www.monogame.net)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com)

---

## 📖 Описание | Description

Игра рассказывает о **Фиолетовом Лорде** — персонаже, который путешествует по мирам IT-технологий, собирая знания и делая выбор в пользу специализации.

**Основная философия:** *Нельзя охватить необъятное. Нужно сделать выбор и двигаться дальше.*

### 🎮 Ключевые механики | Key Mechanics

- **Двойной прыжок** — метафора "попытки охватить необъятное"
- **Режим фокуса** — замедление времени для точного платформинга
- **Сбор знаний** — предметы технологий с описанием
- **Эффект расфокусировки** — визуальное искажение при бездействии
- **FOMO двери** — заблокированные пути для повторного прохождения

---

## 🏗️ Архитектура проекта | Project Architecture

```
PurpleLordPlatformer/
├── Content/                    # Текстуры, звуки, шрифты, эффекты
│   ├── Sprites/               # Спрайты персонажей и объектов
│   ├── Fonts/                 # Шрифты
│   ├── Sounds/                # Звуковые эффекты
│   └── Music/                 # Фоновая музыка
├── Core/                      # Ядро игры
│   ├── Game1.cs              # Главный класс игры
│   ├── GameState.cs          # Глобальное состояние
│   ├── Camera2D.cs           # 2D камера
│   ├── GraphicsUtils.cs      # Графические утилиты
│   └── SceneTransition.cs    # Переходы между сценами
├── Entities/                  # Игровые объекты
│   ├── GameObject.cs         # Базовый класс объекта
│   ├── Player/               # Игрок (Фиолетовый Лорд)
│   ├── Enemies/              # Враги и боссы
│   └── Objects/              # Объекты уровня (платформы, двери, NPC)
├── Levels/                    # Уровни
│   ├── Level.cs              # Базовый класс уровня
│   ├── Level1_FrontendForest/
│   ├── Level2_BackendBadlands/
│   ├── Level3_NeuralNebula/
│   └── Bonus_JuniorOffice/
├── Managers/                  # Менеджеры
│   ├── SceneManager.cs       # Управление сценами
│   ├── ContentManager.cs     # Загрузка контента
│   ├── InputManager.cs       # Ввод (клавиатура, мышь, геймпад)
│   ├── AudioManager.cs       # Звук и музыка
│   ├── EffectManager.cs      # Визуальные эффекты
│   ├── SaveManager.cs        # Сохранения
│   └── UIManager.cs          # Интерфейс
├── Scenes/                    # Сцены
│   └── Scene.cs              # Базовый класс сцены
├── Systems/                   # Игровые системы
│   ├── Physics/              # Физика и коллизии
│   ├── Animation/            # Анимация
│   ├── HealthSystem.cs       # Здоровье
│   ├── PostProcessingSystem.cs # Пост-обработка
│   ├── AchievementSystem.cs  # Достижения
│   ├── DialogueSystem.cs     # Диалоги
│   ├── QuestSystem.cs        # Квесты
│   └── EnemyWaveManager.cs   # Волны врагов
├── Effects/                   # Эффекты
│   └── Particles/            # Система частиц
└── UI/                        # Интерфейс
    ├── Menus/                # Меню (главное, пауза, настройки)
    └── HUD/                  # HUD элементы
```

---

## 🗺️ Уровни | Levels

### Уровень 1: Лес Фронтенда (Frontend Forest)
| Характеристика | Описание |
|----------------|----------|
| **Визуал** | Яркие цвета, платформы в виде div-блоков |
| **Враги** | Баги с глазами-иконками браузеров (Chrome, Firefox) |
| **Механика** | CSS-селектор триггеры |
| **Технологии** | HTML, CSS, JavaScript, React, Vue, Angular, TypeScript, SASS |

### Уровень 2: Пустоши Бэкенда (Backend Badlands)
| Характеристика | Описание |
|----------------|----------|
| **Визуал** | Тёмные тона, серверные стойки, логи на фоне |
| **Враги** | Серверы, кружки кофе |
| **Механика** | Лабиринт из API-труб, 3 ключа-запроса |
| **Технологии** | Go, Python, Node.js, Java, PostgreSQL, Redis, MongoDB, Docker, Kubernetes, gRPC |

### Уровень 3: Туман Нейросетей (Neural Network Nebula)
| Характеристика | Описание |
|----------------|----------|
| **Визуал** | Абстрактные формы, исчезающие платформы, бинарный снег |
| **Враги** | Нейроны-призраки |
| **Механика** | Платформы-веса, активация триггерами |
| **Технологии** | TensorFlow, PyTorch, Keras, Scikit-learn, OpenCV, NLP, GPT |

### Бонусный уровень: Офис Джун (Junior Office)
| Характеристика | Описание |
|----------------|----------|
| **Визуал** | Офис с кубиклами, IDE-иконки |
| **Враги** | Кружки кофе, дедлайны |
| **Механика** | Головоломка с цитатами мемов |
| **Технологии** | Git, Debugging, Unit Tests, Code Review, Agile/Scrum |

---

## 🎮 Управление | Controls

| Действие | Клавиатура | Геймпад |
|----------|------------|---------|
| Движение влево/вправо | A / D или ← / → | Левый стик |
| Прыжок | Пробел | A (Cross) |
| Двойной прыжок | Пробел (в воздухе) | A (в воздухе) |
| Режим фокуса | F | LB (L1) |
| Пауза | Escape | Start |
| Навигация в меню | W / A / S / D | Стрелки/Стик |
| Выбор в меню | Enter или Пробел | A (Cross) |

---

## 🚀 Запуск | How to Run

### Требования | Requirements
- **OS:** Windows 10/11 (x64)
- **.NET:** 8.0 SDK или выше
- **Graphics:** DirectX 11.1 compatible GPU
- **IDE:** Visual Studio 2022 / Rider / VS Code (опционально)

### Установка | Installation

```bash
# Клонирование репозитория
git clone https://github.com/Folombas/PurpleLord.git
cd PurpleLordPlatformer

# Восстановление зависимостей
dotnet restore

# Сборка проекта
dotnet build

# Запуск игры
dotnet run
```

### Сборка релиза | Build Release

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

---

## 🏆 Достижения | Achievements

| Достижение | Описание | Очки |
|------------|----------|------|
| **First Blood** | Убейте первого врага | 10 |
| **Knowledge Seeker** | Соберите 10 предметов знаний | 20 |
| **Focus Master** | Используйте режим фокуса 50 раз | 15 |
| **Collector** | Соберите 80% знаний на уровне | 25 |
| **Explorer** | Посетите все 4 уровня | 30 |
| **Boss Slayer** | Победите босса | 50 |
| **Double Jump Master** | Сделайте 100 двойных прыжков | 20 |

---

## 💾 Сохранения | Save System

Сохранения хранятся в:
```
%APPDATA%/PurpleLordPlatformer/savegame.json
```

**Сохраняемые данные:**
- Прогресс по уровням
- Собранные знания
- Статистика (смерти, время, прыжки)
- Открытые достижения
- Настройки игры

---

## 🛠️ Расширение | Extending the Game

### Добавление нового уровня

1. Создайте папку в `Levels/`
2. Унаследуйтесь от `Level`:
```csharp
public class Level4_NewLevel : Level
{
    public override string SceneId => "level4_new";
    public override string SceneName => "New Level";
    
    protected override void CreateLevel()
    {
        // Создание платформ, врагов, предметов
    }
}
```

### Добавление новой технологии

```csharp
// В KnowledgeItem.cs добавьте новый тип
public enum KnowledgeType
{
    // ...
    NewTech  // Новый тип
}

// Создайте предмет на уровне
AddKnowledge(new KnowledgeItem(position, KnowledgeType.NewTech,
    "Technology Name", "Description text"));
```

### Добавление нового врага

```csharp
public class NewEnemy : Enemy
{
    public NewEnemy(Vector2 position) 
        : base(position, EnemyType.NewEnemy, EnemyBehavior.Patrol)
    {
        Width = 40;
        Height = 40;
    }
    
    protected override void DrawPlaceholder(SpriteBatch spriteBatch)
    {
        // Отрисовка врага
    }
}
```

---

## 📝 Лицензия | License

[MIT License](LICENSE)

---

## 👥 Авторы | Authors

**Senior C# Game Developer**
- Игра создана на **MonoGame 3.8**
- Язык: **C# .NET 8.0**
- Архитектура: **Clean Architecture + ECS элементы**

---

## 🎵 Музыка и Звуки | Music & Sounds

| Уровень | Стиль музыки |
|---------|--------------|
| Главное меню | Minimalist ambient |
| Лес Фронтенда | Chillout electronic |
| Пустоши Бэкенда | Industrial techno |
| Туман Нейросетей | Neural soundscape |
| Офис Джун | Lo-fi hip hop |

---

## 🔧 Технологии разработки | Development Technologies

| Компонент | Технология |
|-----------|------------|
| Движок | MonoGame 3.8.2 |
| Язык | C# 12 (.NET 8) |
| IDE | Visual Studio 2022 |
| Контроль версий | Git + GitHub |
| Архитектура | Clean Architecture |

---

## 📊 Статистика проекта | Project Statistics

| Метрика | Значение |
|---------|----------|
| Файлов кода | ~40 |
| Строк кода | ~8000+ |
| Классов | ~50 |
| Уровней | 4 |
| Достижений | 7 |
| Типов врагов | 6 |
| Технологий | 30+ |

---

**🎮 Приятной игры! | Enjoy the game!**
>>>>>>> 298d2aa9e26c55efb4c968f66277850e4c695877
