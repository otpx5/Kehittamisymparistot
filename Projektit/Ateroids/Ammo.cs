using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Ateroids
{
    internal class Ammo
    {
        Transform transform;
        Texture2D texture;
        public Ammo(Vector2 startPosition, Vector2 startVelocity, Texture2D ammoTexture)
        {
            transform = new Transform();
            transform.position = startPosition;
            transform.velocity = startVelocity;
            texture = ammoTexture;
        }
        public void Draw()
        {
            float rotationAngle = (float)Math.Atan2(transform.velocity.Y, transform.velocity.X) + (float)(Math.PI / 2);
            SpriteCompoment.DrawRotated(texture, transform.position, rotationAngle * Raylib.RAD2DEG);
            transform.move();
        }


    }
}
