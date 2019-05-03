using System.Drawing;
using System.Numerics;

namespace Models
{
    internal class Tile: Frame
    {
        public Vector2 coordinate = new Vector2(0, 0); // это положение тайла не умножанное на размер
        public string Url { get; set; }
        public string Path { get; set; }


        public Tile(Vector2 position, Image image, ref Scene scene)
        {
            SetTile(position, "", "", ref scene, image);
        }

        public Tile(Vector2 position, string url, string path, ref Scene scene)
        {
            SetTile(position, url, path, ref scene, null);
        }

        private void SetTile(Vector2 coordinate, string url, string path, ref Scene scene, Image image = null)
        {
            Size = new Vector2(256, 256);
            this.Scene = scene;
            this.Image = image;
            if (this.Image == null)
            {
                this.Image = CropProd.Properties.Resources.def;
            }
            this.coordinate = coordinate;
            this.Url = url;
            this.Path = path;
            this.Position = Vector2.Multiply(coordinate, Size);
            Screenposition = Vector2.Add(coordinate, scene.Position);
        }

        public override void Draw()
        {
            Screenposition = Vector2.Add(Position, Scene.Position);
        }
    }
}
