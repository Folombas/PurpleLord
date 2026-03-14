// ============================================================================
// Program.cs - Точка входа в игру Purple Lord: Искатель в Цифровой Вселенной
// Entry point for Purple Lord: Digital Universe Seeker
// ============================================================================

using MonoGame.Framework.WindowsDX;

namespace PurpleLord.Core;

/// <summary>
/// Главный класс программы / Main program class
/// </summary>
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // Инициализация игры / Game initialization
        using var game = new Game1();
        game.Run();
    }
}
