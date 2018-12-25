using System.Drawing;
using System.Numerics;

namespace Models
{
    class Frame
    {
        public Image image = null;
        public Vector2 origin = new Vector2(0, 0);
        public Vector2 lefttop = new Vector2(0, 0);
        public double cardX;
        public double cardY;
        public int zorder = 0;
    }
}
