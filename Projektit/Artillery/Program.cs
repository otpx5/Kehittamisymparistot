using System;
using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Artillery
{
    internal class Program
    {
        Rectangle Player1;
        Rectangle Player2;
        Vector2 GunDirection1;
        Vector2 GunDirection2;

        ActiveProjectile? projectile = null;

        bool isPlayer1Turn = true;
        float gravity = 150f;

        float power1 = 400f;
        float power2 = 400f;
        float minPower = 200f;
        float maxPower = 800f;

        List<Rectangle> terrain = new List<Rectangle>();
        int score1 = 0;
        int score2 = 0;

        // Ammustyypit
        readonly List<AmmoType> ammoTypes = new List<AmmoType>();
        int selectedAmmoIndex1 = 0;
        int selectedAmmoIndex2 = 0;

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

            // valitaan “ei ihan reunasta”
            Rectangle terrainLeft = terrain[Math.Min(2, terrain.Count - 1)];
            Rectangle terrainRight = terrain[Math.Max(0, terrain.Count - 3)];

            Player1 = new Rectangle(terrainLeft.X + 5, terrainLeft.Y - 10, 10, 10);
            Player2 = new Rectangle(terrainRight.X + 5, terrainRight.Y - 10, 10, 10);

            GunDirection1 = Vector2.Normalize(new Vector2(1, -1));
            GunDirection2 = Vector2.Normalize(new Vector2(-1, -1));

            LoadAmmoTypes(); 
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

        private void LoadAmmoTypes()
        {
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            string[] files;
            try
            {
                files = Directory.GetFiles(".", "ammo*.json");
            }
            catch
            {
                files = Array.Empty<string>();
            }

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    AmmoType? ammo = JsonSerializer.Deserialize<AmmoType>(json, opts);
                    if (ammo != null)
                    {
                        ammoTypes.Add(ammo);
                    }
                    else
                    {
                        Console.WriteLine($"[WARN] '{file}' → Deserialisointi palautti null.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] Ammuksen lataus epäonnistui tiedostosta '{file}': {ex.Message}");
                }
            }

            if (ammoTypes.Count == 0)
            {
                ammoTypes.Add(new AmmoType { Name = "Perusammus", ExplosionRadius = 30, Price = 50, Damage = 20, Color = "Yellow" });
                ammoTypes.Add(new AmmoType { Name = "Raskasammus", ExplosionRadius = 60, Price = 100, Damage = 50, Color = "Orange" });
            }

            selectedAmmoIndex1 = 0;
            selectedAmmoIndex2 = 0;
        }

        private void Update()
        {
            float rotationSpeed = 1.5f * Raylib.GetFrameTime();

            bool projInactive = projectile == null || !projectile.Active;

            if (projInactive)
            {
                if (isPlayer1Turn)
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) RotateGun(ref GunDirection1, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) RotateGun(ref GunDirection1, rotationSpeed);

                    if (Raylib.IsKeyDown(KeyboardKey.Up)) power1 = Math.Min(power1 + 200f * Raylib.GetFrameTime(), maxPower);
                    if (Raylib.IsKeyDown(KeyboardKey.Down)) power1 = Math.Max(power1 - 200f * Raylib.GetFrameTime(), minPower);

                    for (int i = 0; i < Math.Min(9, ammoTypes.Count); i++)
                    {
                        if (Raylib.IsKeyPressed((KeyboardKey)((int)KeyboardKey.One + i)))
                        {
                            selectedAmmoIndex1 = i;
                        }
                    }
                    // varmistus
                    selectedAmmoIndex1 = ClampIndex(selectedAmmoIndex1, ammoTypes.Count);

                    if (Raylib.IsKeyPressed(KeyboardKey.Space) && ammoTypes.Count > 0)
                    {
                        Fire(Player1, GunDirection1, power1, ammoTypes[selectedAmmoIndex1]);
                    }
                }
                else
                {
                    if (Raylib.IsKeyDown(KeyboardKey.A)) RotateGun(ref GunDirection2, -rotationSpeed);
                    if (Raylib.IsKeyDown(KeyboardKey.D)) RotateGun(ref GunDirection2, rotationSpeed);

                    if (Raylib.IsKeyDown(KeyboardKey.W)) power2 = Math.Min(power2 + 200f * Raylib.GetFrameTime(), maxPower);
                    if (Raylib.IsKeyDown(KeyboardKey.S)) power2 = Math.Max(power2 - 200f * Raylib.GetFrameTime(), minPower);

                    for (int i = 0; i < Math.Min(9, ammoTypes.Count); i++)
                    {
                        if (Raylib.IsKeyPressed((KeyboardKey)((int)KeyboardKey.One + i)))
                        {
                            selectedAmmoIndex2 = i;
                        }
                    }
                    selectedAmmoIndex2 = ClampIndex(selectedAmmoIndex2, ammoTypes.Count);

                    if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.KpEnter))
                    {
                        if (ammoTypes.Count > 0)
                            Fire(Player2, GunDirection2, power2, ammoTypes[selectedAmmoIndex2]);
                    }
                }
            }

            if (projectile != null && projectile.Active)
            {
                float dt = Raylib.GetFrameTime();

                Vector2 v = projectile.Velocity;
                v.Y += gravity * dt;
                projectile.Velocity = v;

                Vector2 p = projectile.Position;
                p += projectile.Velocity * dt;
                projectile.Position = p;

                foreach (var block in terrain)
                {
                    if (Raylib.CheckCollisionPointRec(projectile.Position, block))
                    {
                        projectile.Active = false;
                        NextTurn();
                        return;
                    }
                }

                if (Raylib.CheckCollisionPointRec(projectile.Position, Player1))
                {
                    score2 += projectile.Type.Damage;
                    projectile.Active = false;
                    NextTurn();
                    return;
                }
                else if (Raylib.CheckCollisionPointRec(projectile.Position, Player2))
                {
                    score1 += projectile.Type.Damage;
                    projectile.Active = false;
                    NextTurn();
                    return;
                }

                if (projectile.Position.X < 0 || projectile.Position.X > Raylib.GetScreenWidth()
                    || projectile.Position.Y > Raylib.GetScreenHeight())
                {
                    projectile.Active = false;
                    NextTurn();
                }
            }
        }

        private static int ClampIndex(int idx, int count)
        {
            if (count <= 0) return 0;
            if (idx < 0) return 0;
            if (idx >= count) return count - 1;
            return idx;
        }

        private void RotateGun(ref Vector2 direction, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float x = direction.X * cos - direction.Y * sin;
            float y = direction.X * sin + direction.Y * cos;
            direction = Vector2.Normalize(new Vector2(x, y));
        }

        private void Fire(Rectangle player, Vector2 direction, float power, AmmoType type)
        {
            projectile = new ActiveProjectile
            {
                Type = type,
                Position = new Vector2(player.X + player.Width / 2, player.Y),
                Velocity = direction * power,
                Active = true
            };
        }

        private void NextTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            projectile = null; 
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            foreach (var block in terrain)
                Raylib.DrawRectangleRec(block, Color.DarkGreen);

            Raylib.DrawRectangleRec(Player1, Color.Red);
            Raylib.DrawRectangleRec(Player2, Color.Blue);

            Vector2 gunStart1 = new Vector2(Player1.X + Player1.Width / 2, Player1.Y);
            Vector2 gunStart2 = new Vector2(Player2.X + Player2.Width / 2, Player2.Y);
            Raylib.DrawLineV(gunStart1, gunStart1 + GunDirection1 * 25, Color.White);
            Raylib.DrawLineV(gunStart2, gunStart2 + GunDirection2 * 25, Color.White);

            if (projectile != null && projectile.Active)
            {
                Color projColor = GetColorFromName(projectile.Type.Color);
                Raylib.DrawCircleV(projectile.Position, 4, projColor);
            }

            Raylib.DrawText($"Red score: {score1}", 20, 20, 20, Color.Red);
            Raylib.DrawText($"Blue score: {score2}", Raylib.GetScreenWidth() - 170, 20, 20, Color.SkyBlue);

            Raylib.DrawText($"Red power: {(int)power1}", 20, 70, 15, Color.White);
            Raylib.DrawText($"Blue power: {(int)power2}", Raylib.GetScreenWidth() - 170, 70, 15, Color.White);

            int y = 200;
            if (ammoTypes.Count == 0)
            {
                Raylib.DrawText("No ammo types loaded.", 20, y, 15, Color.Yellow);
            }
            else
            {
                for (int i = 0; i < ammoTypes.Count; i++)
                {
                    var a = ammoTypes[i];
                    string text = $"{i + 1}. {a.Name} (DMG {a.Damage}, RAD {a.ExplosionRadius}, Price {a.Price})";
                    bool selected = (isPlayer1Turn && i == selectedAmmoIndex1) || (!isPlayer1Turn && i == selectedAmmoIndex2);
                    Raylib.DrawText(text, 20, y, 15, selected ? Color.Yellow : Color.White);
                    y += 20;
                }
            }

            string turnText = isPlayer1Turn ? "Red's turn" : "Blue's turn";
            Color turnColor = isPlayer1Turn ? Color.Red : Color.SkyBlue;
            Raylib.DrawText(turnText, Raylib.GetScreenWidth() / 2 - 50, 140, 20, turnColor);

            Raylib.EndDrawing();
        }

        private Color GetColorFromName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "yellow": return Color.Yellow;
                case "orange": return Color.Orange;
                case "red": return Color.Red;
                case "blue": return Color.Blue;
                case "green": return Color.Green;
                default: return Color.White;
            }
        }
    }

    public class AmmoType
    {
        public string Name { get; set; } = string.Empty; 
        public int ExplosionRadius { get; set; }
        public int Price { get; set; }
        public int Damage { get; set; }
        public string Color { get; set; } = "White";  
    }

    public class ActiveProjectile
    {
        public AmmoType Type { get; set; } = default!;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool Active { get; set; } = true;
    }
}
