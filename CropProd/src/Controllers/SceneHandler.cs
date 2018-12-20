using Models;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Controllers
{
    class SceneHandler
    {
        Scene scene;
        Size first = new Size(0, 0);

        public SceneHandler()
        {
            scene = new Scene();
        }

        public void Draw(object sender, PaintEventArgs e)
        {
            List<Frame> frames = scene.drawScene();
            if(frames != null)
            {
                frames.Sort((x, y) => x.zorder.CompareTo(y.zorder));

                foreach (Frame frame in frames)
                {
                    e.Graphics.DrawImage(frame.image, frame.lefttop);
                }
            }
        }

        public Point Scene_MouseLeave(object sender, EventArgs e)
        {
            Point last = new Point(0,0);
            return last - first;
        }

        public Point Scene_MouseUp(object sender, MouseEventArgs e)
        {
            Point last = new Point(e.X, e.Y);
            return last - first;
        }

        public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            first = new Size(e.X, e.Y);
        }

        public void addFrame(Point point, Image img)
        {
            Frame frm = new Frame();
            frm.lefttop = point;
            frm.image = img;
            scene.addImage(frm);
        }

    }
}
