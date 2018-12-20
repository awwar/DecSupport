using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

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
