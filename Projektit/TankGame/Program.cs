using Raylib_cs;
using System.Numerics;


class Program
{
    static void Main()
    {
        Raylib.InitWindow(800, 600, "Tanks Game");
        Raylib.SetTargetFPS(60);

        Tank player1 = new Tank(new Vector2(100, 250), Color.Blue, KeyboardKey.Space);
        Tank player2 = new Tank(new Vector2(600, 250), Color.Red, KeyboardKey.Enter);

        Rectangle wall1 = new Rectangle(300, 100, 50, 400);
        Rectangle wall2 = new Rectangle(500, 100, 50, 400);

        while (!Raylib.WindowShouldClose())
        {
            player1.HandleInput(KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D, wall1, wall2);
            player2.HandleInput(KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right, wall1, wall2);

            player1.UpdateBullets(wall1, wall2, player2);
            player2.UpdateBullets(wall1, wall2, player1);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Green);

            Raylib.DrawRectangleRec(wall1, Color.Maroon);
            Raylib.DrawRectangleRec(wall2, Color.Maroon);

            player1.Draw();
            player2.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

class Tank
{
    public Vector2 Position;
    public Color TankColor;
    private Vector2 direction;
    private const int Size = 40;
    private KeyboardKey shootKey;
    private List<Bullet> bullets = new List<Bullet>();
    private float shootCooldown = 0;
    private const float MaxCooldown = 0.5f; // 0.5 sekunnin viive

    public Tank(Vector2 position, Color color, KeyboardKey shootKey)
    {
        Position = position;
        TankColor = color;
        this.shootKey = shootKey;
        direction = new Vector2(0, -1);
    }

    public void HandleInput(KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right, Rectangle wall1, Rectangle wall2)
    {
        Vector2 newPosition = Position;

        if (Raylib.IsKeyDown(up)) { newPosition.Y -= 5; direction = new Vector2(0, -1); }
        if (Raylib.IsKeyDown(down)) { newPosition.Y += 5; direction = new Vector2(0, 1); }
        if (Raylib.IsKeyDown(left)) { newPosition.X -= 5; direction = new Vector2(-1, 0); }
        if (Raylib.IsKeyDown(right)) { newPosition.X += 5; direction = new Vector2(1, 0); }

        Rectangle newBounds = new Rectangle(newPosition.X, newPosition.Y, Size, Size);

        if (!Raylib.CheckCollisionRecs(newBounds, wall1) &&
            !Raylib.CheckCollisionRecs(newBounds, wall2) &&
            newBounds.X >= 0 && newBounds.X + Size <= 800 &&
            newBounds.Y >= 0 && newBounds.Y + Size <= 600)
        {
            Position = newPosition;
        }

        // Päivitä cooldownia
        if (shootCooldown > 0)
        {
            shootCooldown -= Raylib.GetFrameTime();
        }

        if (Raylib.IsKeyPressed(shootKey) && shootCooldown <= 0)
        {
            bullets.Add(new Bullet(Position + new Vector2(Size / 2, Size / 2), direction));
            shootCooldown = MaxCooldown;
        }
    }

    public void UpdateBullets(Rectangle wall1, Rectangle wall2, Tank enemy)
    {
        bullets.ForEach(b => b.Update());
        bullets.RemoveAll(b => b.IsOffScreen() || b.HasCollided(wall1) || b.HasCollided(wall2) || b.HasHitTank(enemy));
    }

    public void Draw()
    {
        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, Size, Size, TankColor);
        Vector2 barrelStart = new Vector2(Position.X + Size / 2 - 5, Position.Y + Size / 2 - 5);
        Vector2 barrelEnd = barrelStart + direction * 20;
        Raylib.DrawRectangle((int)barrelEnd.X, (int)barrelEnd.Y, 10, 20, Color.DarkGray);
        bullets.ForEach(b => b.Draw());
    }
}

class Bullet
{
    public Vector2 Position;
    private Vector2 direction;
    private const int Speed = 10;
    private const int Size = 5;

    public Bullet(Vector2 position, Vector2 direction)
    {
        Position = position;
        this.direction = direction;
    }

    public void Update()
    {
        Position += direction * Speed;
    }

    public void Draw()
    {
        Raylib.DrawCircle((int)Position.X, (int)Position.Y, Size, Color.Black);
    }

    public bool IsOffScreen()
    {
        return Position.X < 0 || Position.X > 800 || Position.Y < 0 || Position.Y > 600;
    }

    public bool HasCollided(Rectangle wall)
    {
        return Raylib.CheckCollisionCircleRec(Position, Size, wall);
    }

    public bool HasHitTank(Tank tank)
    {
        return Raylib.CheckCollisionCircleRec(Position, Size, new Rectangle(tank.Position.X, tank.Position.Y, 40, 40));
    }
}
