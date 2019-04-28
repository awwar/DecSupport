using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Data : Frame,IFrame
    {
        public Data(Vector2 position, Vector2 size, Image image, ref Scene scene)
        {
            this.Scene = scene;
            this.Position = position;
            this.Size = size;
            this.Image = image;
        }

        public override void Draw()
        {
            Screenposition = Vector2.Add(Position, Scene.position);
        }
    }
}
