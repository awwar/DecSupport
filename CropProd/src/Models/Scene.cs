using System.Collections.Generic;
using System.Numerics;

namespace Models
{
    class Scene
    {
        List<Tile> frames = new List<Tile>();
        public Vector2 center = new Vector2(0, 0);
        public Vector2 size = new Vector2(0, 0);
        public Vector2 coord = new Vector2(0, 0);
        public Camera camera;
        public int itemCount = 0;

        public Scene(Vector2 size)
        {
            this.size = size;
            camera = new Camera();
            camera.Moove(center, size);
        }

        public List<Tile> DrawScene(Vector2 delta)
        {
            center = Vector2.Add(center, delta);
            camera.Moove(center, size);

            return frames;
        }

        public void AddImage(Tile item)
        {
            itemCount++;
            frames.Add(item);
        }

        public void RemoveImage(Tile item)
        {
            itemCount--;
            frames.Remove(item);
        }

        public void ClearImagePool()
        {
            itemCount = 0;
            frames.Clear();
        }
    }
}
