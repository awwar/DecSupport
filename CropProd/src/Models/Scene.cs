using System.Collections.Generic;
using System.Numerics;

namespace Models
{
    class Scene
    {
        List<Frame> frames = new List<Frame>();
        Vector2 leftTop = new Vector2(0, 0);

        public List<Frame> drawScene()
        {
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
