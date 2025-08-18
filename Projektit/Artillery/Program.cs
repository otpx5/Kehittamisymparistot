using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

namespace Artillery
{
    internal class Program
    {
        Rectangle Player1;
        Rectangle Player2;
        Vector2 GunDirection1;
        Vector2 GunDirection2;
        Vector2? ProjectilePosition = null;
        Vector2 ProjectileVelocity;
        bool isPlayer1Turn = true;
        bool projectileActive = false;
        float gravity = 150f;

        float power1 = 400f; 
        float power2 = 400f; 
        float minPower = 200f;
        float maxPower = 800f;

        List<Rectangle> terrain = new List<Rectangle>();
        int score1 = 0;
        int score2 = 0;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            Raylib.InitWindow(800, 600, "Artillery");
            Raylib.SetTargetFPS(60);
            InitGame();

            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Raylib.CloseWindow();
        }

        private void InitGame()
        {
            GenerateTerrain();

            Rectangle terrainLeft = terrain[2];
            Rectangle terrainRight = terrain[terrain.Count - 3];

            Player1 = new Rectangle(terrainLeft.X + 5, terrainLeft.Y - 10, 10, 10);
            Player2 = new Rectangle(terrainRight.X + 5, terrainRight.Y - 10, 10, 10);

            GunDirection1 = new Vector2(1, -1);
            GunDirection2 = new Vector2(-1, -1);
        }

        private void GenerateTerrain()
        {
            terrain.Clear();
            int blockWidth = 20;
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            Random rand = new Random();

            for (int x = 0; x < screenWidth; x += blockWidth)
            {
                int height = rand.Next(50, 150);
                Rectangle rect = new Rectangle(x, screenHeight - height, blockWidth, height);
                terrain.Add(rect);
            }
        }

        private void Update()
        {
            float rotationSpeed = 1.5f * Raylib.GetFrameTime();

            if (!projectileActive)
            {
                if (isPlayer1Turn)
                {
                    // Aseen kulma
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) RotateGun(ref GunDirection1, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) RotateGun(ref GunDirection1, rotationSpeed);

                    // Voiman säätö
                    if (Raylib.IsKeyDown(KeyboardKey.Up)) power1 = Math.Min(power1 + 200f * Raylib.GetFrameTime(), maxPower);
                    if (Raylib.IsKeyDown(KeyboardKey.Down)) power1 = Math.Max(power1 - 200f * Raylib.GetFrameTime(), minPower);

                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        Fire(Player1, GunDirection1, power1);
                    }
                }
                else
                {
                    if (Raylib.IsKeyDown(KeyboardKey.A)) RotateGun(ref GunDirection2, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.D)) RotateGun(ref GunDirection2, rotationSpeed);

                    // Voiman säätö
                    if (Raylib.IsKeyDown(KeyboardKey.W)) power2 = Math.Min(power2 + 200f * Raylib.GetFrameTime(), maxPower);
                    if (Raylib.IsKeyDown(KeyboardKey.S)) power2 = Math.Max(power2 - 200f * Raylib.GetFrameTime(), minPower);

                    if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.KpEnter))
                    {
                        Fire(Player2, GunDirection2, power2);
                    }
                }
            }

            if (projectileActive && ProjectilePosition.HasValue)
            {
                float dt = Raylib.GetFrameTime();
                ProjectileVelocity.Y += gravity * dt;
                ProjectilePosition += ProjectileVelocity * dt;

                foreach (var block in terrain)
                {
                    if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, block))
                    {
                        projectileActive = false;
                        NextTurn();
                        return;
                    }
                }

                if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, Player1))
                {
                    score2++;
                    projectileActive = false;
                    NextTurn();
                    return;
                }
                else if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, Player2))
                {
                    score1++;
                    projectileActive = false;
                    NextTurn();
                    return;
                }

                if (ProjectilePosition.Value.X < 0 || ProjectilePosition.Value.X > Raylib.GetScreenWidth()
                    || ProjectilePosition.Value.Y > Raylib.GetScreenHeight())
                {
                    projectileActive = false;
                    NextTurn();
                }
            }
        }

        private void RotateGun(ref Vector2 direction, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float x = direction.X * cos - direction.Y * sin;
            float y = direction.X * sin + direction.Y * cos;
            direction = Vector2.Normalize(new Vector2(x, y));
        }

        private void Fire(Rectangle player, Vector2 direction, float power)
        {
            Vector2 gunPos = new Vector2(player.X + player.Width / 2, player.Y);
            ProjectilePosition = gunPos;
            ProjectileVelocity = direction * power;
            projectileActive = true;
        }

        private void NextTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            ProjectilePosition = null;
            ProjectileVelocity = Vector2.Zero;
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            foreach (var block in terrain)
            {
                Raylib.DrawRectangleRec(block, Color.DarkGreen);
            }

            Raylib.DrawRectangleRec(Player1, Color.Red);
            Raylib.DrawRectangleRec(Player2, Color.Blue);

            Vector2 gunStart1 = new Vector2(Player1.X + Player1.Width / 2, Player1.Y);
            Vector2 gunStart2 = new Vector2(Player2.X + Player2.Width / 2, Player2.Y);
            Raylib.DrawLineV(gunStart1, gunStart1 + GunDirection1 * 25, Color.White);
            Raylib.DrawLineV(gunStart2, gunStart2 + GunDirection2 * 25, Color.White);

            if (ProjectilePosition.HasValue)
            {
                Raylib.DrawCircleV(ProjectilePosition.Value, 4, Color.Yellow);
            }

            Raylib.DrawText($"Red score: {score1}", 20, 20, 20, Color.Red);
            Raylib.DrawText($"Blue score: {score2}", Raylib.GetScreenWidth() - 170, 20, 20, Color.SkyBlue);

            // UUSI: näytetään voima HUDissa
            Raylib.DrawText($"Red power: {(int)power1}", 20, 70, 15, Color.White);
            Raylib.DrawText($"Blue power: {(int)power2}", Raylib.GetScreenWidth() - 170, 70, 15, Color.White);

            Raylib.DrawText("Red: ←/→ kulma, ↑/↓ voima, SPACE = ammu", 20, 100, 15, Color.LightGray);
            Raylib.DrawText("Blue: A/D kulma, W/S voima, ENTER = ammu", Raylib.GetScreenWidth() - 330, 100, 15, Color.LightGray);

            string turnText = isPlayer1Turn ? "Red's turn" : "Blue's turn";
            Color turnColor = isPlayer1Turn ? Color.Red : Color.SkyBlue;
            Raylib.DrawText(turnText, Raylib.GetScreenWidth() / 2 - 50, 140, 20, turnColor);

            Raylib.EndDrawing();
        }
    }
}
