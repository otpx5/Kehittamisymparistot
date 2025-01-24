using Raylib_cs;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        // Ikkunan mitat
        const int screenWidth = 800;
        const int screenHeight = 450;

        // Aloita ikkuna
        Raylib.InitWindow(screenWidth, screenHeight, "DVD Screensaver");

        // Tekstin sijainti, suunta ja nopeus
        Vector2 position = new Vector2(screenWidth / 2, screenHeight / 2); // Keskellä aluksi
        Vector2 direction = new Vector2(1, 1); // Diagonaalinen liike
        float speed = 100.0f;

        // Tekstin asetukset
        string text = "DVD";
        int fontSize = 40;
        float spacing = 2.0f;

        // Fontti ja tekstin koko
        Font font = Raylib.GetFontDefault();
        Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, spacing);

        // Tekstin väri
        Color textColor = Color.Yellow;

        while (!Raylib.WindowShouldClose()) // Päälooppi
        {
            // Päivitä
            float frameTime = Raylib.GetFrameTime();

            // Liikuta tekstiä
            position += direction * speed * frameTime;

            // Törmäystarkistus ikkunan reunoihin (vaaka)
            if (position.X <= 0 || position.X + textSize.X >= screenWidth)
            {
                direction.X *= -1; // Vaihda suunta vaaka-akselilla
                position.X = Math.Clamp(position.X, 0, screenWidth - textSize.X); // Estä ulos hyppiminen
                textColor = GetRandomColor(); // Vaihda väriä
                speed += 10.0f; // Kasvata nopeutta
            }

            // Törmäystarkistus ikkunan reunoihin (pysty)
            if (position.Y <= 0 || position.Y + textSize.Y >= screenHeight)
            {
                direction.Y *= -1; // Vaihda suunta pysty-akselilla
                position.Y = Math.Clamp(position.Y, 0, screenHeight - textSize.Y); // Estä ulos hyppiminen
                textColor = GetRandomColor(); // Vaihda väriä
                speed += 10.0f; // Kasvata nopeutta
            }

            // Piirrä
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawTextEx(font, text, position, fontSize, spacing, textColor); // Piirrä teksti
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    // Apufunktio satunnaisen värin luomiseen
    static Color GetRandomColor()
    {
        return new Color(
            Raylib.GetRandomValue(0, 255), // Punainen
            Raylib.GetRandomValue(0, 255), // Vihreä
            Raylib.GetRandomValue(0, 255), // Sininen
            255 // Läpinäkyvyys
        );
    }
}
