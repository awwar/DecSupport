using Events;
using Interfaces;
using System.Drawing;
using System.Numerics;

namespace Models
{
    class Tile : IFrame
    {
        public Vector2 position { get; set; }
        public Vector2 size { get; set; }
        public Vector2 screenposition { get; set; }
        public Image   image { get; set; }

        public Vector2 coordinate = new Vector2(0, 0); // это положение тайла не умножанное на размер
        public string url = null;
        public string path = null;

        private Scene scene; // Это ссылка на изображение


        public Tile(Vector2 position, Image image, ref Scene scene)
        {
            setTile(position, "", "", ref scene, image);
        }

        public Tile(Vector2 position, string url, string path, ref Scene scene)
        {
            setTile(position, url, path, ref scene, null);
        }

        private void setTile(Vector2 position, string url, string path, ref Scene scene, Image image = null)
        {
            size = new Vector2(256, 256);
            this.scene = scene;
            if (image == null)
            {
                this.image = Image.FromFile("def.png");
            } else
            {
                this.image = image;
            }
            coordinate = position;
            this.url = url;
            this.path = path;
            this.position = Vector2.Multiply(coordinate, size);
            screenposition = Vector2.Add(position, scene.position);
        }

        public void ImageLoaded(object sender, ImageLoadArgs e)
        {
            if (e.path == path)
            {
                this.image = e.image;

            }
        }

        public void draw()
        {
            screenposition = Vector2.Add(position, scene.position);
        }
    }
}
