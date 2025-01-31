using System;
using System.Drawing;
using System.Numerics;
using Raylib_cs;

namespace Pong
{
    class Program
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 600;

            // Initialize Raylib
            Raylib.InitWindow(screenWidth, screenHeight, "Pong Game");

            // Define player variables
            const int paddleWidth = 20;
            const int paddleHeight = 100;
            const int paddleSpeed = 5;

            // Player 1 variables
            Raylib_cs.Rectangle player1 = new Rectangle(50, screenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight);
            int player1Score = 0;
            Vector2 player1ScorePosition = new Vector2(screenWidth / 4, 20);

            // Player 2 variables
            Rectangle player2 = new Rectangle(screenWidth - 50 - paddleWidth, screenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight);
            int player2Score = 0;
            Vector2 player2ScorePosition = new Vector2(screenWidth * 3 / 4, 20);

            // Ball variables
            Vector2 ballPosition = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 ballSpeed = new Vector2(5, 5); // initial speed
            const int ballRadius = 10;

            while (!Raylib.WindowShouldClose())
            {
                // Update
                // Move players

                // Move ball
                ballPosition.x += ballSpeed.x;
                ballPosition.y += ballSpeed.y;

                // Check collision with players
                if (CheckCollisionCircleRec(ballPosition, ballRadius, player1) || CheckCollisionCircleRec(ballPosition, ballRadius, player2))
                {
                    ballSpeed.x = -ballSpeed.x; // reverse X direction
                }

                // Check collision with top and bottom walls
                if ((ballPosition.y - ballRadius <= 0) || (ballPosition.y + ballRadius >= screenHeight))
                {
                    ballSpeed.y = -ballSpeed.y; // reverse Y direction
                }

                // Check if ball passed the paddle (scoring)
                if (ballPosition.x + ballRadius <= 0)
                {
                    // Player 2 scores
                    player2Score++;
                    ballPosition = new Vector2(screenWidth / 2, screenHeight / 2); // reset ball position
                    ballSpeed = new Vector2(5, 5); // reset ball speed
                }
                else if (ballPosition.x - ballRadius >= screenWidth)
                {
                    // Player 1 scores
                    player1Score++;
                    ballPosition = new Vector2(screenWidth / 2, screenHeight / 2); // reset ball position
                    ballSpeed = new Vector2(-5, -5); // reset ball speed
                }

                // Input handling
                // TODO: Handle player input to move paddles

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                // Draw players
                Raylib.DrawRectangleRec(player1, Color.WHITE);
                Raylib.DrawRectangleRec(player2, Color.WHITE);

                // Draw ball
                Raylib.DrawCircleV(ballPosition, ballRadius, Color.WHITE);

                // Draw scores
                Raylib.DrawText(player1Score.ToString(), (int)player1ScorePosition.x, (int)player1ScorePosition.y, 20, Color.WHITE);
                Raylib.DrawText(player2Score.ToString(), (int)player2ScorePosition.x, (int)player2ScorePosition.y, 20, Color.WHITE);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
