using System.Drawing;
using System.Numerics;

namespace Models
{
    internal class Tile : Frame
    {
        public string Url { get; set; }
        public string Path { get; set; }

        public Tile(Vector2 coordinate, Image image) : this(coordinate, "", "", image) { }

        public Tile(Vector2 coordinate, string url, string path) : this(coordinate, url, path, null) { }

        public Tile(Vector2 coordinate, string url, string path, Image image = null)
        {
            Size = new Vector2(Settings.Settings.TileSize, Settings.Settings.TileSize);
            Image = image;
            if (Image == null)
            {
                Image = CropProd.Properties.Resources.def;
            }
            Coordinate = coordinate;
            Url = url;
            Path = path;
            Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
