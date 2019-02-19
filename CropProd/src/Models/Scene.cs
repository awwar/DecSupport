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
        public Camera camera;

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

            foreach (Frame frame in frames.ToArray())
            {
                if (!CutOut(frame))
                {
                    frame.scenecoord = Vector2.Add(frame.lefttop, center);
                }
            }
            return frames;
        }

        public void AddImage(Frame item)
        {
            frames.Add(item);
        }

        public void RemoveImage(Frame item)
        {
            frames.Remove(item);
        }

        public void ClearImagePool()
        {
            frames.Clear();
        }


        private bool CutOut(Frame frame)
        {
            if (frame.scenecoord.Y < -frame.image.Size.Height || frame.scenecoord.Y > size.Y)
            {
                RemoveImage(frame);
                /*TileHandler.GetTileAt(new Vector2(
                    (frame.lefttop.Y < 0)
                        ? frame.lefttop.Y + (float)(Math.Floor(size.Y/256)+1) * 256
                        : frame.lefttop.Y - (float)(Math.Floor(size.Y / 256)+1) * 256,
                    frame.lefttop.X
                    )
                );*/
                return true;
            }
            else if (frame.scenecoord.X < -frame.image.Size.Width || frame.scenecoord.X > size.X)
            {
                RemoveImage(frame);
                TileHandler.GetTileAt(new Vector2(
                    (frame.lefttop.X < 0)
                        ? frame.lefttop.X + (float)(Math.Floor(size.X / 256)+1) * 256
                        : frame.lefttop.X - (float)(Math.Floor(size.X / 256) + 1) * 256,
                    frame.lefttop.Y
                    )
                );
                return true;
            }
            return false;
        }
    }
}
