﻿using System.Drawing;
using System.Numerics;

namespace Models
{
    class Data : Frame
    {
        public Data(Vector2 coordinate, Vector2 size, Image image, ref Scene scene)
        {
            this.Scene = scene;
            this.Size = size;
            this.Image = image;
            this.Position = Vector2.Multiply(coordinate, Size);
        }

        public override void Draw()
        {
            Screenposition = Vector2.Add(Position, Scene.Position);
        }
    }
}
