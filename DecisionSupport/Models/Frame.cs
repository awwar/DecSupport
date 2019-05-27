﻿using System.Drawing;
using System.Numerics;

namespace Models
{
    public abstract class Frame
    {
        public Vector2 Coordinate { get; set; } = new Vector2(0, 0); // это положение тайла не умножанное на размер
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Screenposition { get; set; }
        public Image Image { get; set; }

        public virtual void Draw(Vector2 scenePose)
        {
            Screenposition = Vector2.Add(Position, scenePose);
        }
    }
}