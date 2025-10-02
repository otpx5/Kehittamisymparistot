using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using System.Numerics;

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
        Transform transform;
        Texture2D texture;
        AsteroidSize size;
        public Asteroid(Vector2 startPosition, Vector2 startVelocity, Texture2D asteroidTexture, AsteroidSize size)
        {
            transform = new Transform();
            transform.position = startPosition;
            transform.velocity = startVelocity;
            texture = asteroidTexture;
            this .size = size;
        }
        public void Draw()
        {
            float rotationAngle = (float)Math.Atan2(transform.velocity.Y, transform.velocity.X) + (float)(Math.PI / 2);
            SpriteCompoment.DrawRotated(texture, transform.position, rotationAngle * Raylib.RAD2DEG);
            transform.move();
        }
    }
}
