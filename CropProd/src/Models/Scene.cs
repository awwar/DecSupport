using System;
using System.Collections.Generic;
using System.Numerics;

namespace Models
{
    class Scene
    {
        List<Frame> frames = new List<Frame>();
        public Vector2 center = new Vector2(0, 0);
        public Vector2 size = new Vector2(0, 0);
        public Camera camera;

        public Scene()
        {
            camera = new Camera();
            camera.Moove(center, size);
        }

        public List<Frame> drawScene(Vector2 delta)
        {
            center = Vector2.Add(center, delta);
            camera.Moove(center, size);
            return frames;
        }

        public void addImage(Frame item)
        {
            frames.Add(item);
        }

        public void removeImage(Frame item)
        {
            frames.Remove(item);
        }

        public void clearImagePool()
        {
            frames.Clear();
        }
    }
}
