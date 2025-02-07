using System;
using System.Numerics;
using Raylib_cs;

namespace Pong
{
    class Program
    {
        static void Main()
        {
            //  Pelin ikkunan asetukset
            const int screenWidth = 800;
            const int screenHeight = 600;
            Raylib.InitWindow(screenWidth, screenHeight, "Pong Game");
            Raylib.SetTargetFPS(60);

            //  Pelaajien asetukset
            const int paddleWidth = 20;
            const int paddleHeight = 100;
            const int paddleSpeed = 6;

            //  Pallon asetukset
            const int ballRadius = 10;
            Vector2 ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 ballSpeed = new Vector2(5, 5);

            // Pelaajat (x, y, leveys, korkeus)
            Rectangle player1 = new Rectangle(30, screenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight);
            Rectangle player2 = new Rectangle(screenWidth - 50, screenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight);

            // Pistelasku
            int player1Score = 0;
            int player2Score = 0;

            while (!Raylib.WindowShouldClose())
            {
                // Pelaajien ohjaus (käytetään `Y` eikä `y`)
                if (Raylib.IsKeyDown(KeyboardKey.W) && player1.Y > 0)
                    player1.Y -= paddleSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.S) && player1.Y + player1.Height < screenHeight)
                    player1.Y += paddleSpeed;

                if (Raylib.IsKeyDown(KeyboardKey.Up) && player2.Y > 0)
                    player2.Y -= paddleSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.Down) && player2.Y + player2.Height < screenHeight)
                    player2.Y += paddleSpeed;

                //  Pallon liike
                ballPosition.X += ballSpeed.X;
                ballPosition.Y += ballSpeed.Y;

                //  Törmäykset pelaajiin
                if (Raylib.CheckCollisionCircleRec(ballPosition, ballRadius, player1) ||
                    Raylib.CheckCollisionCircleRec(ballPosition, ballRadius, player2))
                {
                    ballSpeed.X *= -1.1f; // Vaihda suunta ja nopeuta hieman
                }

                //  Törmäykset ylä- ja alareunaan
                if (ballPosition.Y - ballRadius <= 0 || ballPosition.Y + ballRadius >= screenHeight)
                {
                    ballSpeed.Y *= -1;
                }

                //  Pistelasku
                if (ballPosition.X + ballRadius <= 0) // Pelaaja 2 saa pisteen
                {
                    player2Score++;
                    ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
                    ballSpeed = new Vector2(5, 5);
                }
                else if (ballPosition.X - ballRadius >= screenWidth) // Pelaaja 1 saa pisteen
                {
                    player1Score++;
                    ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
                    ballSpeed = new Vector2(-5, -5);
                }

                //  Piirretään peli
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkBlue);

                // Pisteviiva keskelle kenttää
                for (int i = 0; i < screenHeight; i += 20)
                {
                    Raylib.DrawRectangle(screenWidth / 2 - 2, i, 4, 10, Color.LightGray);
                }

                // Pelaajat
                Raylib.DrawRectangleRec(player1, Color.SkyBlue);
                Raylib.DrawRectangleRec(player2, Color.Gold);

                // Pallo
                Raylib.DrawCircleV(ballPosition, ballRadius, Color.White);

                // Pisteet
                Raylib.DrawText(player1Score.ToString(), screenWidth / 4, 20, 40, Color.SkyBlue);
                Raylib.DrawText(player2Score.ToString(), screenWidth * 3 / 4, 20, 40, Color.Gold);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
