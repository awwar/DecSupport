using CropProd;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Device;

namespace Controllers
{
    static class SceneHandler
    {
        public static Scene scene;
        public static Form1 form;
        static Vector2 first = new Vector2(0, 0);
        static Vector2 delta = new Vector2(0, 0);
        static Vector2 last = new Vector2(0, 0);
        
        static public void Initialization(Form1 getform)
        {
            scene = new Scene();
            form = getform;
            scene.size = new Vector2(
                form.scene.Width,
                form.scene.Height
            );
        }

        static public void Draw(object sender, PaintEventArgs e)
        {
            scene.size = new Vector2(
                form.scene.Width,
                form.scene.Height
            );
            List<Frame> frames = scene.drawScene(delta);


            if (frames != null)
            {
                frames.Sort((x, y) => x.zorder.CompareTo(y.zorder));

                Pen pen;
                foreach (Frame frame in frames)
                {
                    frame.lefttop = Vector2.Add(frame.lefttop, delta);
                    e.Graphics.DrawImage(frame.image, frame.lefttop.X, frame.lefttop.Y, frame.image.Width, frame.image.Height);
                    pen = new Pen(Color.Red, 1f);
                    e.Graphics.DrawRectangle(pen, frame.lefttop.X, frame.lefttop.Y, frame.image.Width, frame.image.Height);

                    e.Graphics.DrawString(frame.cardX + " " + frame.cardY, new Font("Arial", 15), new SolidBrush(Color.White), frame.lefttop.X, frame.lefttop.Y);
                }
                //Рисуем центр сцены
                pen = new Pen(Color.Orange, 4f);
                e.Graphics.DrawLine(pen, scene.center.X - 10, scene.center.Y, scene.center.X + 10, scene.center.Y);
                e.Graphics.DrawLine(pen, scene.center.X, scene.center.Y - 10, scene.center.X, scene.center.Y + 10);
                //Рисуем центр экрана
                pen = new Pen(Color.Red, 2f);
                e.Graphics.DrawLine(pen, scene.camera.center.X - 10, scene.camera.center.Y, scene.camera.center.X + 10, scene.camera.center.Y);
                e.Graphics.DrawLine(pen, scene.camera.center.X, scene.camera.center.Y - 10, scene.camera.center.X, scene.camera.center.Y + 10);

                e.Graphics.DrawString("Center: " + SceneHandler.scene.camera.center.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 0);
                e.Graphics.DrawString("Position: " + SceneHandler.scene.camera.position.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 20);
                e.Graphics.DrawString("Tile Center: " + SceneHandler.scene.camera.tileCenter.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 40);
            }
            delta = new Vector2(0, 0);
        }
        
        static public void Scene_MouseMoove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                first = new Vector2(e.X, e.Y);
                delta = Vector2.Subtract(last, first);
                last = first;
                form.Redraw();
            }
        }

        static public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        static public void AddFrame(Vector2 point, Image img, double[] latlon)
        {
            Frame frm = new Frame();
            frm.lefttop = point;
            frm.image = img;
            frm.cardX = latlon[0];
            frm.cardY = latlon[1];
            scene.addImage(frm);
        }
    }
}
