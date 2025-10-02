using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Ateroids
{
    internal class Player
    {
        public Transform transform;
        Texture2D imageTexture;
        float enginePower = 100.0f;
        float max_speed = 200.0f;
        float turningSpeed = 3.0f;
        float rotationAngle = 0.0f;

        public object White { get; private set; }

        public Player(Vector2 startPosition, Texture2D texture)
        {
            transform = new Transform();
           transform.position = startPosition;
           transform.velocity = new Vector2(0, 0);
            imageTexture = texture;
        }
        public void Draw()
        {
            SpriteCompoment.DrawRotated(imageTexture, transform.position, rotationAngle * Raylib.RAD2DEG);
        }
        public void Update()
        {
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                enginePower = 200.0f;
            }
            else
            {
                enginePower = 0.0f;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                float rotation = turningSpeed * Raylib.GetFrameTime();
                transform.direction = Vector2.Transform(transform.direction, Matrix3x2.CreateRotation(rotation));
                rotationAngle += rotation;
            }
            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                float rotation = -turningSpeed * Raylib.GetFrameTime();
                transform.direction = Vector2.Transform(transform.direction, Matrix3x2.CreateRotation(rotation));
                rotationAngle += rotation;
            }
            transform.velocity += transform.direction * enginePower * Raylib.GetFrameTime();
            // Check that does not go too fast!
            if ((transform.velocity.Length()) > max_speed)
            {
                transform.velocity = Vector2.Normalize(transform.velocity) * max_speed;
            }
            transform.move();

        }
        public bool shootUpdate()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                return true;
            }
            return false;
        }

    }
   
}
