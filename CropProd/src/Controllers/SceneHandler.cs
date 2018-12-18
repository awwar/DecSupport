using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Controllers
{
    class SceneHandler
    {
        Scene scene;

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

        public void Scene_MouseLeave(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Scene_MouseUp(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
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
