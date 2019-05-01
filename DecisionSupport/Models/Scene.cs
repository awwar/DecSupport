using System.Numerics;

namespace Models
{
    class Scene
    {
        public Vector2 Size { get; set; } = new Vector2(0, 0);
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Coordinate { get; set; } = new Vector2(0, 0);
        public int Zoom { get; set; } = 18;
        public double Lat { get; set; } = 55.763582;
        public double Lon { get; set; } = 37.663053;

        public Scene(Vector2 size)
        {
            this.Size = size / 2;
            Position = Vector2.Add(Position, this.Size);
        }

        public void Update(Vector2 delta)
        {
            Position = Vector2.Add(Position, delta);
        }

        public void SetTileCenter(Vector2 center)
        {
            Coordinate = center;
        }

        public void Resize(Vector2 size)
        {
            Position = Vector2.Subtract(Position, this.Size);
            this.Size = size;
            Position = Vector2.Add(Position, size / 2);
        }
    }
}
