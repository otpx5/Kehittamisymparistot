using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Ateroids
{
    internal class Transform
    {
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 direction = new Vector2(0, -1);
        public void move()
        {
            position += velocity * Raylib.GetFrameTime();
            if (position.X > Raylib.GetScreenWidth()) 
            {
                position.X = 0; 
            }
            if (position.X < 0) 
            {
                position.X = Raylib.GetRenderWidth(); 
            }
            if (position.Y > Raylib.GetScreenHeight()) 
            {
                position.Y = 0; 
            }
            if (position.Y < 0) 
            {
                position.Y = Raylib.GetScreenHeight(); 
            }
        }
    }
}
