using CropProd;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Device;
using System.Threading;

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
            List<Frame> frames = scene.DrawScene(delta);


            if (frames != null)
            {

                Pen pen = new Pen(Color.Red, 1f);
                foreach (Frame frame in frames.ToArray())
                {
                    if (!CutOut(frame))
                    {
                        frame.scenecoord = Vector2.Add(frame.lefttop, scene.center);
                    } else
                    {
                        continue;
                    }
                    e.Graphics.DrawImage(frame.image, frame.scenecoord.X, frame.scenecoord.Y,frame.image.Width, frame.image.Height);
                    //e.Graphics.DrawRectangle(pen, frame.lefttop.X + scene.center.X, frame.lefttop.Y + scene.center.Y, frame.image.Width, frame.image.Height);

                    //e.Graphics.DrawString(frame.scenecoord.ToString(), new Font("Arial", 15), new SolidBrush(Color.White), frame.lefttop.X + scene.center.X, frame.lefttop.Y + scene.center.Y);
                }
                e.Graphics.DrawString("Scene center: " + SceneHandler.scene.center.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 80);
                //Рисуем центр сцены
                /*pen = new Pen(Color.Orange, 4f);
                e.Graphics.DrawLine(pen, scene.center.X - 10, scene.center.Y, scene.center.X + 10, scene.center.Y);
                e.Graphics.DrawLine(pen, scene.center.X, scene.center.Y - 10, scene.center.X, scene.center.Y + 10);
                //Рисуем центр экрана
                pen = new Pen(Color.Red, 2f);
                e.Graphics.DrawLine(pen, scene.camera.center.X - 10, scene.camera.center.Y, scene.camera.center.X + 10, scene.camera.center.Y);
                e.Graphics.DrawLine(pen, scene.camera.center.X, scene.camera.center.Y - 10, scene.camera.center.X, scene.camera.center.Y + 10);

                e.Graphics.DrawString("Item Count: " + SceneHandler.scene.itemCount.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 0);
                e.Graphics.DrawString("Position: " + SceneHandler.scene.camera.position.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 20);
                e.Graphics.DrawString("Tile Center: " + SceneHandler.scene.camera.tileCenter.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 40);
                e.Graphics.DrawString("Size: " + SceneHandler.scene.size.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 60);*/
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

        static public void AddFrame(Frame frame)
        {
            scene.AddImage(frame);
        }

        static public void AddFrame(Vector2 point, Image image)
        {
            Frame frm = new Frame(
                    point,
                    image
                );
            scene.AddImage(frm);
        }

        static public void Refresh()
        {
            form.Redraw();
        }

        static private bool CutOut(Frame frame)
        {
            if (frame.scenecoord.Y < -256 || frame.scenecoord.Y > scene.size.Y)
            {
                scene.RemoveImage(frame);
                TileHandler.GetTileAt(new Vector2(
                    frame.coord.X,
                    (frame.scenecoord.Y - 256 < 0)
                        ? frame.coord.Y + (float)(Math.Floor(scene.size.Y / 256) + 2)
                        : frame.coord.Y - (float)(Math.Floor(scene.size.Y / 256) + 2)
                    )
                );
                return true;
            }
            else if (frame.scenecoord.X < -256 || frame.scenecoord.X  > scene.size.X)
            {
                scene.RemoveImage(frame);
                TileHandler.GetTileAt(new Vector2(
                    (frame.scenecoord.X - 256 < 0)
                        ? frame.coord.X + (float)(Math.Floor(scene.size.X / 256) + 2)
                        : frame.coord.X - (float)(Math.Floor(scene.size.X / 256) + 2),
                    frame.coord.Y
                    )
                );
                return true;
            }
            return false;
        }
    }
}
