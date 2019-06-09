using System.Numerics;

namespace Models
{
    class Scene
    {

        //55.763582;
        //37.663053;
        public string Name { get; set; } = "Проект принятия решения";
        public Vector2 Size { get; set; } = new Vector2(0, 0);
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Coordinate { get; set; } = new Vector2(0, 0);
        public int Zoom { get; set; } = 18;

        public Scene() : this(new Vector2(0, 0))
        {
        }

        public Scene(Vector2 size)
        {
            Size = size;
            Position = Vector2.Add(Position, Size / 2);
        }

        public Vector2 Update(Vector2 delta)
        {
            Position = Vector2.Add(Position, delta);
            return Position;
        }

        public Vector2 Resize(Vector2 size)
        {
            Position = Vector2.Subtract(Position, Size / 2);
            Size = size;
            Position = Vector2.Add(Position, size / 2);
            return Position;
        }

        public void AppendProject(Project proj)
        {
            Name = proj.Name;
            Coordinate = new Vector2((float)proj.TileX, (float)proj.TileY);
        }
    }
}
