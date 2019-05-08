using System.Drawing;
using System.Numerics;

namespace Models
{
    public class Data : Frame
    {
        public string LayerHash { set; get; }
        public Data(Vector2 coordinate, Image image, string LayerHash)
        {
            this.LayerHash = LayerHash;
            Size = new Vector2(image.Size.Width, image.Size.Height);
            Image = image;
            Position = Vector2.Multiply(coordinate, Size);
        }

        public Data(Vector2 coordinate, Vector2 size, Image image, string LayerHash)
        {
            this.LayerHash = LayerHash;
            Size = size;
            Image = image;
            Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
