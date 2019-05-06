using System.Drawing;
using System.Numerics;

namespace Models
{
    class Data : Frame
    {
        public Data(Vector2 coordinate, Image image)
        {
            this.Size = new Vector2(image.Size.Width, image.Size.Height);
            this.Image = image;
            this.Position = Vector2.Multiply(coordinate, Size);
        }

        public Data(Vector2 coordinate, Vector2 size, Image image)
        {
            this.Size = size;
            this.Image = image;
            this.Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
