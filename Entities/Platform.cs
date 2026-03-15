// ============================================================================
// Platform.cs - Класс платформы / Platform class
// ============================================================================

using Microsoft.Xna.Framework;

namespace PurpleLord.Entities
{
    /// <summary>
    /// Типы платформ / Platform types
    /// </summary>
    public enum PlatformType
    {
        Solid,        // Твёрдая платформа / Solid platform
        OneWay,       // Односторонняя / One-way platform
        Moving,       // Движущаяся / Moving platform
        Disappearing, // Исчезающая / Disappearing platform
        Hazard        // Опасная (шипы, лава) / Hazard (spikes, lava)
    }

    /// <summary>
    /// Класс платформы.
    /// Platform class.
    /// </summary>
    public class Platform : GameObject
    {
        public PlatformType Type { get; set; }
        public bool IsSolid => Type == PlatformType.Solid;
        public bool IsOneWay => Type == PlatformType.OneWay;
        public bool IsHazard => Type == PlatformType.Hazard;

        public Platform(Vector2 position, int width, int height, PlatformType type = PlatformType.Solid)
            : base(position)
        {
            Width = width;
            Height = height;
            Type = type;
            Tag = "Platform";
        }
    }
}
