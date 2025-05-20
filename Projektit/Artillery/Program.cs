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

            // Valitaan pelaajille sopiva kohta maastosta
            Rectangle terrainLeft = terrain[2]; // ei aivan reunasta
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
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) RotateGun(ref GunDirection1, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) RotateGun(ref GunDirection1, rotationSpeed);
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        Fire(Player1, GunDirection1);
                    }
                }
                else
                {
                    if (Raylib.IsKeyDown(KeyboardKey.A)) RotateGun(ref GunDirection2, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.D)) RotateGun(ref GunDirection2, rotationSpeed);
                    if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.KpEnter))
                    {
                        Fire(Player2, GunDirection2);
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
                    if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, Player1))
                    {
                        score2++;
                        projectileActive = false;
                        NextTurn();
                        return; // <-- Add this
                    }
                    else if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, Player2))
                    {
                        score1++;
                        projectileActive = false;
                        NextTurn();
                        return; // <-- Add this
                    }

                    // Jos menee ruudun ulkopuolelle
                    if (ProjectilePosition.Value.X < 0 || ProjectilePosition.Value.X > Raylib.GetScreenWidth()
                        || ProjectilePosition.Value.Y > Raylib.GetScreenHeight())
                    {
                        projectileActive = false;
                        NextTurn();
                        return; // <-- Add this
                    }
                    NextTurn();
                }
                else if (Raylib.CheckCollisionPointRec(ProjectilePosition.Value, Player2))
                {
                    score1++;
                    projectileActive = false;
                    NextTurn();
                    return; // <-- Add this
                }

                // Jos menee ruudun ulkopuolelle
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

        private void Fire(Rectangle player, Vector2 direction)
        {
            Vector2 gunPos = new Vector2(player.X + player.Width / 2, player.Y);
            ProjectilePosition = gunPos;
            ProjectileVelocity = direction * 400;
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

            // Maasto
            foreach (var block in terrain)
            {
                Raylib.DrawRectangleRec(block, Color.DarkGreen);
            }

            // Pelaajat
            Raylib.DrawRectangleRec(Player1, Color.Red);
            Raylib.DrawRectangleRec(Player2, Color.Blue);

            // Tykit
            Vector2 gunStart1 = new Vector2(Player1.X + Player1.Width / 2, Player1.Y);
            Vector2 gunStart2 = new Vector2(Player2.X + Player2.Width / 2, Player2.Y);
            Raylib.DrawLineV(gunStart1, gunStart1 + GunDirection1 * 25, Color.White);
            Raylib.DrawLineV(gunStart2, gunStart2 + GunDirection2 * 25, Color.White);

            // Ammus
            if (ProjectilePosition.HasValue)
            {
                Raylib.DrawCircleV(ProjectilePosition.Value, 4, Color.Yellow);
            }

            // Pisteet ja ohjeet
            Raylib.DrawText($"Red score: {score1}", 20, 20, 20, Color.Red);
            Raylib.DrawText($"Blue score: {score2}", Raylib.GetScreenWidth() - 170, 20, 20, Color.SkyBlue);
            Raylib.DrawText("Red: ←/→ + SPACE", 20, 50, 15, Color.LightGray);
            Raylib.DrawText("Blue: A/D + ENTER", Raylib.GetScreenWidth() - 230, 50, 15, Color.LightGray);

            // Vuoronäyttö
            string turnText = isPlayer1Turn ? "Red's turn" : "Blue's turn";
            Color turnColor = isPlayer1Turn ? Color.Red : Color.SkyBlue;
            Raylib.DrawText(turnText, Raylib.GetScreenWidth() / 2 - 50, 80, 20, turnColor);

            Raylib.EndDrawing();
        }
    }
}
