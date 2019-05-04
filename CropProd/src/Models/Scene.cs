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

        public Scene(Vector2 size)
        {
            this.Size = size;
            this.Position = Vector2.Add(this.Position, this.Size / 2);
        }

        public void Update(Vector2 delta)
        {
            this.Position = Vector2.Add(this.Position, delta);
        }

        public void SetTileCenter(Vector2 center)
        {
            this.Coordinate = center;
        }

        public void Resize(Vector2 size)
        {
            this.Position = Vector2.Subtract(this.Position, this.Size / 2);
            this.Size = size;
            this.Position = Vector2.Add(this.Position, size / 2);
        }

        public void AppendProject(Project proj)
        {
            this.Name = proj.Name;
            this.Coordinate = new Vector2((float)proj.Lat, (float)proj.Lon);
        }
    }
}
