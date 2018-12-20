﻿using Models;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CropProd;

namespace Controllers
{
    class SceneHandler
    {
        Scene scene;
        public Form1 form;
        Vector2 first = new Vector2(0, 0);
        Vector2 delta = new Vector2(0,0);
        Vector2 last = new Vector2(0, 0);

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
                    frame.lefttop = Vector2.Add(frame.lefttop, delta);
                    e.Graphics.DrawImage(frame.image, frame.lefttop.X, frame.lefttop.Y);
                }
            }
        }

        public void Scene_MouseMoove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                first = new Vector2(e.X, e.Y);
                delta = Vector2.Subtract(last, first);
                last = first;
                form.Redraw();
            }
            
        }

        public void Scene_MouseUp(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
            delta = Vector2.Subtract(last,first);
            form.Redraw();
        }

        public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        public void addFrame(Vector2 point, Image img)
        {
            Frame frm = new Frame();
            frm.lefttop = point;
            frm.image = img;
            scene.addImage(frm);
        }
    }
}
