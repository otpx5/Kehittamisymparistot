using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using System.Numerics;
using ClassLibrary;

namespace Ateroids
{
   internal enum AsteroidSize
    {
        Small,
        Medium,
        Large
    }
    internal class Asteroid
    {
       public Transform transform;
        Texture2D texture;
        public AsteroidSize size;
        public float radius;
        public Asteroid(Vector2 startPosition, Vector2 startVelocity, Texture2D asteroidTexture, AsteroidSize size)
        {
            transform = new Transform();
            transform.position = startPosition;
            transform.velocity = startVelocity;
            texture = asteroidTexture;
            this .size = size;
            if (size == AsteroidSize.Large)
            {
                radius = 50;
            }
            else if (size == AsteroidSize.Medium)
            {
                radius = 30;
            }
            else
            {
                radius = 10;
            }
        }
        public void Draw()
        {
            float rotationAngle = (float)Math.Atan2(transform.velocity.Y, transform.velocity.X) + (float)(Math.PI / 2);
            SpriteCompoment.DrawRotated(texture, transform.position, rotationAngle * Raylib.RAD2DEG);
            Raylib.DrawCircleLines((int)transform.position.X, (int)transform.position.Y, radius, Color.Red);
            transform.move();
        }
    }
}
