using System.Numerics;
using Raylib_cs;
namespace Ateroids
{
    class Program
    {
        public static Player player;
        public static List<Asteroid> asteroids;
        public static List<Ammo> ammo;
        public static Texture2D bigasteroidTexture;
        public static Texture2D mediumasteroidTexture;
        public static Texture2D smallasteroidTexture;
        public static Texture2D imageTexture;
        public static int level = 1;
        static void Main()
        {

            Raylib.InitWindow(800, 800, "Asteroids");
            Raylib.SetTargetFPS(60);
            imageTexture = Raylib.LoadTexture("Images/playerShip1_blue.png");
            Texture2D ammoTexture = Raylib.LoadTexture("Images/laserRed16.png");
            bigasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_big3.png");
            mediumasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_med1.png");
            smallasteroidTexture = Raylib.LoadTexture("Images/meteorGrey_small2.png");
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
                    ammo.Add(new Ammo(player.transform.position, player.transform.direction * 500, ammoTexture));
                }
                for (int i = 0; i < ammo.Count; i++)
                {
                    Ammo currentAmmo = ammo[i];
                    // Check lifetime
                    if ((float)Raylib.GetTime() - currentAmmo.creationTime > currentAmmo.lifeTime)
                    {
                        ammo.RemoveAt(i);
                        i--;
                        continue;
                    }
                    currentAmmo.Draw();
                }
                CheckCollisions();
                if(asteroids.Count == 0)
                {
                    level++;
                    ResetGame();
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
        public static void CheckCollisions()
        {
           
            // Käy läpi pelaajan ammukset lopusta alkuun.
            for (int a = asteroids.Count - 1; a >= 0; a--)
            {

                Asteroid checkAsteroid = asteroids[a];
                if (Raylib.CheckCollisionCircles(player.transform.position, 20, checkAsteroid.transform.position, 50))
                {
                    level = 1;
                    ResetGame();
                    break;
                }
                for (int b = ammo.Count - 1; b >= 0; b--)
                {
                    Ammo checkBullet = ammo[b];
              

                    // Tarkista törmääkö ammus asteroidiin
                    if (Raylib.CheckCollisionCircles(checkAsteroid.transform.position, 50, checkBullet.transform.position, 10))
                    {
                        // poista ammus
                        ammo.RemoveAt(b);

                        // Luo kaksi uutta asteroidia?

                        // poista asteroidi
                        asteroids.RemoveAt(a);
                        // ToDo Split Asteroids
                        if (checkAsteroid.size == AsteroidSize.Large)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 newVelocity = new Vector2(Raylib.GetRandomValue(-50, 50), Raylib.GetRandomValue(-50, 50));
                                asteroids.Add(new Asteroid(checkAsteroid.transform.position, newVelocity, mediumasteroidTexture, AsteroidSize.Medium));
                            }
                        }
                        if (checkAsteroid.size == AsteroidSize.Medium)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 newVelocity = new Vector2(Raylib.GetRandomValue(-100, 100), Raylib.GetRandomValue(-100, 100));
                                asteroids.Add(new Asteroid(checkAsteroid.transform.position, newVelocity, smallasteroidTexture, AsteroidSize.Small));
                            }
                        }
                        else
                        if (checkAsteroid.size == AsteroidSize.Small)
                        {
                            // Pienin asteroidi, ei luoda uusia
                            break;
                        }


                        // Skippaa loput asteroidit
                        break;
                    }
                    
                }
            }
        }
        public static void ResetGame()
        {
            asteroids.Clear();
            ammo.Clear();
            player = new Player(new Vector2(400, 300), imageTexture);
            for (int i = 0; i < 3*level; i++)
            {
                asteroids.Add(new Asteroid(new Vector2(Raylib.GetRandomValue(0, 800), Raylib.GetRandomValue(0, 800)), new Vector2(Raylib.GetRandomValue(-20, 20), Raylib.GetRandomValue(-20, 20)), bigasteroidTexture, AsteroidSize.Large));
            }
        }
    }
}
    