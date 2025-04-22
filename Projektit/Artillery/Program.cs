
using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Artillery
{
    internal class Program
    {
        Rectangle Ground;
        Rectangle Player;
        Vector2 GunDirection;
        static void Main(string[] args)
        {
            Program game = new Program();
            game.init();

        }
        private void init()
        {
            Raylib.InitWindow(600, 600, "Artillery");
            Ground = new Rectangle(0, Raylib.GetScreenHeight() - 50, Raylib.GetScreenWidth(), 50);
            Player = new Rectangle(50, Ground.Y-10,10, 10);
            GunDirection = new Vector2(0, -1 );
            while (Raylib.WindowShouldClose() == false)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.SkyBlue);
                Raylib.DrawRectangleRec(Ground,Color.Green);
                Raylib.DrawRectangleRec(Player,Color.Red);
                Vector2 GunStart = new Vector2(Player.X + Player.Width / 2, Player.Y);
                Raylib.DrawLineV(GunStart,GunStart + GunDirection * 10, Color.Black);
                Raylib.EndDrawing();
            }
        }
    }
}
