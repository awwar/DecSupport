using Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    abstract class Frame : IFrame
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Screenposition { get; set; }
        public Image Image { get; set; }

        protected Scene Scene;

        abstract public void Draw();
    }
}
