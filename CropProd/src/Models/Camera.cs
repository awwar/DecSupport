using System;
using System.Numerics;

namespace Models
{
    class Camera
    {
        public Vector2 position = new Vector2(0, 0);
        public Vector2 center = new Vector2(0, 0);
        public Vector2 tileCenter = new Vector2(0, 0);
        public Vector2 coordcenter = new Vector2(0, 0);

        public void Moove(Vector2 center, Vector2 size)
        {

            this.center = size / 2;
            position = -(center - (size / 2));

            tileCenter = new Vector2(
                (float)(Math.Floor(this.position.X / 256)),
                (float)(Math.Floor(this.position.Y / 256))
            );
        }
    }
}
