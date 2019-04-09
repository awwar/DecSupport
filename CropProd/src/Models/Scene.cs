using System.Collections.Generic;
using System.Numerics;

namespace Models
{
    class Scene
    {
        public Vector2 size = new Vector2(0, 0);
        public Vector2 position = new Vector2(0, 0);
        public Vector2 coordinate = new Vector2(0, 0);
        public int zoom = 18;
        public double Lat = 55.763582;
        public double Lon = 37.663053;

        private List<Tile> frames = new List<Tile>();

        public Scene(Vector2 size)
        {
            this.size = size / 2;
            position = Vector2.Add(position, this.size);
        }

        public void update(Vector2 delta)
        {
            position = Vector2.Add(position, delta);
        }

        public List<Tile> drawScene()
        {
            return frames;
        }

        public void addFrame(Tile item)
        {
            frames.Add(item);
        }

        public void removeFrame(Tile item)
        {
            frames.Remove(item);
        }

        public void clearFramePool()
        {
            foreach (Tile item in frames)
            {
                item.image.Dispose();
            }
            frames.Clear();
        }

        public void setTileCenter(Vector2 center)
        {
            coordinate = center;
        }
    }
}
