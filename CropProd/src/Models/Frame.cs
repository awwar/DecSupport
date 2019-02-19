using System.Drawing;
using System.Numerics;

namespace Models
{
    class Frame
    {
        public Image image = null; //изображение 
        public Vector2 lefttop = new Vector2(0, 0); //левый-верхний угол
        public Vector2 scenecoord = new Vector2(0, 0);//координаты на сцене
        public int zorder = 0;
    }
}
