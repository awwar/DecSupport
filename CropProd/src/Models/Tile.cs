using Events;
using Interfaces;
using System.Drawing;
using System.Numerics;

namespace Models
{
    internal class Tile: Frame
    {
        public Vector2 coordinate = new Vector2(0, 0); // это положение тайла не умножанное на размер
        public string url = null;
        public string path = null;


        public Tile(Vector2 position, Image image, ref Scene scene)
        {
            SetTile(position, "", "", ref scene, image);
        }

        public Tile(Vector2 position, string url, string path, ref Scene scene)
        {
            SetTile(position, url, path, ref scene, null);
        }

        private void SetTile(Vector2 position, string url, string path, ref Scene scene, Image image = null)
        {
            Size = new Vector2(256, 256);
            this.Scene = scene;
            this.Image = image;
            if (this.Image == null)
            {
                this.Image = CropProd.Properties.Resources.def;
            }
            coordinate = position;
            this.url = url;
            this.path = path;
            this.Position = Vector2.Multiply(coordinate, Size);
            Screenposition = Vector2.Add(position, scene.position);
        }

        public void ImageLoaded(object sender, ImageLoadArgs e)
        {
            if (e.Path == path)
            {
                Image = e.Image;
            }
        }

        public override void Draw()
        {
            Screenposition = Vector2.Add(Position, Scene.position);
        }
    }
}
