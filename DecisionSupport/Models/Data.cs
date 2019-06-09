using System.Drawing;
using System.Numerics;

namespace Models
{
    public class Data : Frame
    {
        public Data(Vector2 coordinate, Image image)
        {
            Size = new Vector2(image.Size.Width, image.Size.Height);
            Image = image;
            Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
