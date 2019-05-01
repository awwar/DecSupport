using System.Drawing;
using System.Numerics;

namespace Models
{
    class Data : Frame
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
            Screenposition = Vector2.Add(Position, Scene.Position);
        }
    }
}
