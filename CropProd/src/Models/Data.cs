using System.Drawing;
using System.Numerics;

namespace Models
{
    class Data : Frame
    {
        public Data(Vector2 coordinate, Vector2 size, Image image)
        {
            this.Size = size;
            this.Image = image;
            this.Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
