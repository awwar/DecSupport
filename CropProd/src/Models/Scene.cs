using Controllers;
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
        public Vector2 coord = new Vector2(0, 0);
        public Camera camera;
        public int itemCount = 0;

        public Scene()
        {
            camera = new Camera();
            camera.Moove(center, size);
            center = Vector2.Add(center, camera.center);
        }

        public List<Frame> DrawScene(Vector2 delta)
        {
            center = Vector2.Add(center, delta);
            camera.Moove(center, size);
            
            return frames;
        }

        public void AddImage(Frame item)
        {
            itemCount++;
            frames.Add(item);
        }

        public void RemoveImage(Frame item)
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
