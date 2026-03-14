// ============================================================================
// Program.cs - Точка входа / Entry point
// ============================================================================

using System;
using Microsoft.Xna.Framework;

namespace PurpleLordPlatformer
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
            {
                Core.GraphicsUtils.GameInstance.GraphicsDevice = game.GraphicsDevice;
                game.Run();
            }
        }
    }
}
