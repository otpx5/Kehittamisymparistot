using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using ClassLibrary;


namespace Ateroids
{
    internal class Ammo
    {
        public Transform transform;
        Texture2D texture;
        public float radius = 20;
        public float lifeTime = 2.0f;// Lifetime in seconds
        public float creationTime;
        public Ammo(Vector2 startPosition, Vector2 startVelocity, Texture2D ammoTexture)
        {
            transform = new Transform();
            transform.position = startPosition;
            transform.velocity = startVelocity;
            texture = ammoTexture;
            creationTime = (float)Raylib.GetTime();
        }
        public void Draw()
        {
            float rotationAngle = (float)Math.Atan2(transform.velocity.Y, transform.velocity.X) + (float)(Math.PI / 2);
            SpriteCompoment.DrawRotated(texture, transform.position, rotationAngle * Raylib.RAD2DEG);
            Raylib.DrawCircleLines((int)transform.position.X, (int)transform.position.Y, radius, Color.Blue);
            transform.move();
        }


    }
}
