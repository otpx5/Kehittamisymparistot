using System.Numerics;
using Raylib_cs;
namespace Ateroids
{
    class Program
    {
        public static Player player;
        public static List<Asteroid> asteroids;
        public static List<Ammo> ammo;
        static void Main()
        {

            Raylib.InitWindow(800, 800, "Asteroids");
            Raylib.SetTargetFPS(60);
            Texture2D imageTexture = Raylib.LoadTexture("Images/playerShip1_blue.png");
            Texture2D ammoTexture = Raylib.LoadTexture("Images/laserRed16.png");
            Texture2D bigasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_big3.png");
            Texture2D mediumasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_med1.png");
            Texture2D smallasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_small2.png");
            ammo = new List<Ammo>();
            asteroids = new List<Asteroid>();
            player = new Player(new Vector2(400, 300), imageTexture);
            for (int i = 0; i < 5; i++)
            {
                asteroids.Add(new Asteroid(new Vector2(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 800)), new Vector2(Raylib.GetRandomValue(-20, 20), Raylib.GetRandomValue(-20, 20)), bigasteroidTexture, AsteroidSize.Large));
            }
            while (!Raylib.WindowShouldClose())
            {
                player.Update();
                if (player.shootUpdate())
                {
                    ammo.Add(new Ammo(player.transform.position, player.transform.direction * 100, ammoTexture));
                }
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);
                player.Draw();
                for (int i = 0; i < ammo.Count; i++)
                {
                    ammo[i].Draw();
                }
                for (int i = 0; i < asteroids.Count; i++)
                {
                    asteroids[i].Draw();
                }
                Raylib.EndDrawing();
            }
            Raylib.UnloadTexture(imageTexture);
            Raylib.CloseWindow();

        }
        public void CheckCollisions()
        {
            // Käy läpi pelaajan ammukset lopusta alkuun.
            for (int b = ammo.Count - 1; b >= 0; b--)
            {
                // Käy myös asteroidit läpi lopusta alkuun
                for (int a = asteroids.Count - 1; a >= 0; a--)
                {
                    Ammo checkBullet = ammo[b];
                    Asteroid checkAsteroid = asteroids[a];

                    // Tarkista törmääkö ammus asteroidiin
                    if (false)
                    {
                        // poista ammus
                        ammo.RemoveAt(b);

                        // Luo kaksi uutta asteroidia?

                        // poista asteroidi
                        asteroids.RemoveAt(a);

                        // Skippaa loput asteroidit
                        break;
                    }
                }
            }
        }
    }
}

    