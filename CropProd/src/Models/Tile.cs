using System.Drawing;
using System.Numerics;

namespace Models
{
    internal class Tile: Frame
    {
        public string Url { get; set; }
        public string Path { get; set; }

        public Tile(Vector2 coordinate, Image image) : this(coordinate , "", "" , image) { }

        public Tile(Vector2 coordinate, string url, string path) : this(coordinate, url, path, null) { }

        public Tile(Vector2 coordinate, string url, string path , Image image = null)
        {
            this.Size = new Vector2(Settings.Settings.TileSize, Settings.Settings.TileSize);
            this.Image = image;
            if (this.Image == null)
            {
                this.Image = CropProd.Properties.Resources.def;
            }
            this.Coordinate = coordinate;
            this.Url = url;
            this.Path = path;
            this.Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
