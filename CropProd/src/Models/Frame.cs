using System;
using System.Drawing;
using System.Numerics;

namespace Models
{
    class Frame
    {
        public Image image = null; //изображение 
        public Vector2 lefttop = new Vector2(0, 0); //левый-верхний угол
        public Vector2 scenecoord = new Vector2(0, 0);//координаты на сцене
        public Vector2 coord = new Vector2(0, 0);//координаты на сцене

        public Frame(Image image, Vector2 coord)
        {
            this.image = image;
            this.coord = coord;
            this.lefttop.X = coord.X * 256;
            this.lefttop.Y = coord.Y * 256;
            Console.WriteLine(lefttop.ToString());
        }

    }
}
