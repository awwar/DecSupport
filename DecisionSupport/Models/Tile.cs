using DSCore;
using System.Drawing;
using System.Numerics;


namespace Models
{
    public class Tile : Frame
    {
        public string Url { get; set; }
        public string Path { get; set; }

        public Tile(Vector2 coordinate, Image image) : this(coordinate, "", "", image) { }

        public Tile(Vector2 coordinate, string url, string path) : this(coordinate, url, path, null) { }

        public Tile(Vector2 coordinate, string url, string path, Image image = null)
        {
            Size = new Vector2(Settings.TileSize, Settings.TileSize);
            Image = image;
            if (Image == null)
            {
                Image = Settings.DefaultImage;
            }
            Coordinate = coordinate;
            Url = url;
            Path = path;
            Position = Vector2.Multiply(coordinate, Size);
        }
    }
}
