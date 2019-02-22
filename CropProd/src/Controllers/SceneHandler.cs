using CropProd;
using Models;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

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
            Tile[] tiles = scene.DrawScene(delta).ToArray();


            if (tiles != null)
            {
                Pen pen = new Pen(Color.Red, 1f);
                UpdateTiles(tiles);
                foreach (Tile frame in tiles)
                {
                    e.Graphics.DrawImage(frame.image, frame.scenecoord.X, frame.scenecoord.Y, 256, 256);
                }
                //e.Graphics.DrawString("Scene center: " + scene.center.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 80);
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

        static public void AddTile(Tile frame)
        {
            scene.AddImage(frame);
        }

        static public void AddTile(Vector2 point, Image image)
        {
            Tile tile = new Tile(
                    point,
                    image
                );
            scene.AddImage(tile);
        }

        static public void Refresh()
        {
            form.Redraw();
        }

        static private void UpdateTiles(Tile[] frame)
        {
            TileHandler.Loader.block = true;
            foreach (Tile frames in frame)
            {
                if (frames.scenecoord.Y < -256 || frames.scenecoord.Y > scene.size.Y)
                {
                    scene.RemoveImage(frames);
                    TileHandler.GetTileAt(new Vector2(
                        frames.coord.X,
                        (frames.scenecoord.Y - 256 < 0)
                            ? frames.coord.Y + (float)(Math.Floor(scene.size.Y / 256) + 2)
                            : frames.coord.Y - (float)(Math.Floor(scene.size.Y / 256) + 2)
                        )
                    );
                }
                else if (frames.scenecoord.X < -256 || frames.scenecoord.X > scene.size.X)
                {
                    scene.RemoveImage(frames);
                    TileHandler.GetTileAt(new Vector2(
                        (frames.scenecoord.X - 256 < 0)
                            ? frames.coord.X + (float)(Math.Floor(scene.size.X / 256) + 2)
                            : frames.coord.X - (float)(Math.Floor(scene.size.X / 256) + 2),
                        frames.coord.Y
                        )
                    );
                }
                else
                {
                    frames.scenecoord = Vector2.Add(frames.lefttop, scene.center);
                }
            }
            TileHandler.Loader.block = false;
        }
    }
}
