using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Ateroids
{
    internal class SpriteCompoment
    {
        public static void DrawRotated(Texture2D texture, Vector2 centerPosition, float angle)
        {
            // Piirrä koko kuva:
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            // centerPosition on kuvan keskikohdan paikka ruudulla
            Rectangle dest = new Rectangle(centerPosition, texture.Width, texture.Height);

            Raylib.DrawTexturePro(texture,
              source, dest,
              new Vector2(texture.Width / 2, texture.Height / 2), // Keskikohdan ympäri
              angle,
              Color.White);
        }
    }
}
