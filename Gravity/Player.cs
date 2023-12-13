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

        public bool inAir = true;

        int gravity = 500;



        public Player(Vector2 startPos, int speed, int size)
        {
            transform = new Transform(startPos, new Vector2(0, 0), speed);
            collision = new Collision(new Vector2(size, size));
        }

        public void Draw()
        {

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
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && !inAir)
            {
                inAir = true;
                gravity *= -1;
            }

            if (inAir)
            {
                transform.position.Y += gravity * Raylib.GetFrameTime();
            }


            return false;
        }

        public Rectangle GetRec()
        {
            return new Rectangle(transform.position.X, transform.position.Y, collision.size.X, collision.size.Y);
        }

    }
}
