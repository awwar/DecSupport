using Interfaces;
using System.Drawing;
using System.Numerics;

namespace Models
{
    public abstract class Frame : IFrame
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Screenposition { get; set; }
        public Image Image { get; set; }

        internal Scene Scene;

        abstract public void Draw();
    }
}
