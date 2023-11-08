using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Gravity
{
    internal class Player
    {
        public Transform transform { get; private set; }
        public Collision collision { get; private set; }


        //Texture playerImage;


        public Player(Vector2 startPos, int speed, int size)
        {
            transform = new Transform(startPos, new Vector2(0, 0), speed);
            collision = new Collision(new Vector2(size, size));
            //this.playerImage = playerImage;
        }

        public void Draw()
        {

            //float scaleX = collision.size.X / playerImage.width;
            //float scaleY = collision.size.Y / playerImage.height;
            //float scale = Math.Min(scaleX, scaleY);
            //
            //Raylib.DrawTextureEx(playerImage, transform.position, 0f, scale, Raylib.WHITE);

            Raylib.DrawRectangleV(transform.position, collision.size, Raylib.SKYBLUE);
        }

        public bool Update()
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            {
                transform.position.X -= transform.speed * Raylib.GetFrameTime();
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            {
                transform.position.X += transform.speed * Raylib.GetFrameTime();
            }

            return false;
        }

    }
}
