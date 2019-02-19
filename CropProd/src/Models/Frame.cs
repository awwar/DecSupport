using System.Drawing;
using System.Numerics;

namespace Models
{
    class Frame
    {
        public Image image = null; //изображение 
        public Vector2 origin = new Vector2(0, 0); //центр
        public Vector2 lefttop = new Vector2(0, 0); //левый-верхний угол
        public double cardX;
        public double cardY;
        public int zorder = 0;
    }
}
