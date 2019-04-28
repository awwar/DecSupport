using LatLonToTile;
using System.Numerics;

namespace Models
{
    internal class Scene
    {
        public Vector2 size = new Vector2(0, 0);
        public Vector2 position = new Vector2(0, 0);
        public Vector2 coordinate = new Vector2(0, 0);
        public Vector2 delta = new Vector2(0, 0);
        public int zoom = 18;
        public double Lat = 55.763582;
        public double Lon = 37.663053;

        public Scene(Vector2 size)
        {
            this.size = size / 2;
            position = Vector2.Add(position, this.size);
        }

        public void Update(Vector2 delta)
        {
            position = Vector2.Add(position, delta);
            this.delta = Vector2.Add(this.delta, delta);
        }

        public void SetTileCenter(Vector2 center)
        {
            coordinate = center;
        }

        public void Resize(Vector2 size)
        {
            position = Vector2.Subtract(position, this.size);
            this.size = size;
            position = Vector2.Add(position, size / 2);
        }
    }
}
