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
        static int zoom = 18;

        static public void Initialization(Form1 getform)
        {
            form = getform;
            scene = new Scene(new Vector2(
                form.scene.Width,
                form.scene.Height
            ));
        }

        static public void Scene_Resize(object sender, EventArgs e)
        {
            scene.size = new Vector2(
                            form.scene.Width,
                            form.scene.Height
                        );
            scene.center = scene.size / 2;
            Refresh();
        }

        static public void Scene_Draw(object sender, PaintEventArgs e)
        {
            Tile[] tiles = scene.DrawScene(delta).ToArray();

            if (tiles != null)
            {
                Pen pen = new Pen(Color.Red, 1f);
                UpdateTiles(tiles);
                foreach (Tile frame in tiles)
                {
                    e.Graphics.DrawImage(frame.image, frame.scenecoord.X, frame.scenecoord.Y, 256, 256);
                    e.Graphics.DrawRectangle(pen, frame.scenecoord.X, frame.scenecoord.Y, 256, 256);
                    e.Graphics.DrawString(frame.scenecoord.ToString(), new Font("Arial", 15), new SolidBrush(Color.White), frame.lefttop.X + scene.center.X, frame.lefttop.Y + scene.center.Y);
                }
                //Рисуем центр сцены
                pen = new Pen(Color.Orange, 4f);
                e.Graphics.DrawLine(pen, scene.center.X - 10, scene.center.Y, scene.center.X + 10, scene.center.Y);
                e.Graphics.DrawLine(pen, scene.center.X, scene.center.Y - 10, scene.center.X, scene.center.Y + 10);
                //Рисуем центр экрана
                pen = new Pen(Color.Red, 2f);
                e.Graphics.DrawLine(pen, scene.camera.center.X - 10, scene.camera.center.Y, scene.camera.center.X + 10, scene.camera.center.Y);
                e.Graphics.DrawLine(pen, scene.camera.center.X, scene.camera.center.Y - 10, scene.camera.center.X, scene.camera.center.Y + 10);

                e.Graphics.DrawString("Item Count: " + tiles.Length, new Font("Arial", 16), new SolidBrush(Color.Black), 0, 0);
                e.Graphics.DrawString("Scene: " + scene.size.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black), 0, 20);
                e.Graphics.DrawString("Zoom: " + TileHandler.CurrentZ, new Font("Arial", 16), new SolidBrush(Color.Black), 0, 40);

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
                Refresh();
            }
        }

        static public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        static public void Zoom(int zoomed)
        {
            zoom += zoomed;
            zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            TileHandler.CurrentZ = zoom;
            TileHandler.Clear();
            TileHandler.GetScreenAt();
        }

        static public void AddTile(Vector2 point, Image image)
        {
            Tile tile = new Tile(
                    point,
                    image
                );
            AddTile(tile);
        }

        static public void AddTile(Tile tile)
        {
            scene.AddImage(tile);
            Refresh();
        }

        static public void Refresh()
        {
            form.Redraw();
        }

        static private void UpdateTiles(Tile[] frames)
        {
            foreach (Tile frame in frames)
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
                }
                else if (frame.scenecoord.X < -256 || frame.scenecoord.X > scene.size.X)
                {
                    scene.RemoveImage(frame);
                    TileHandler.GetTileAt(new Vector2(
                        (frame.scenecoord.X - 256 < 0)
                            ? frame.coord.X + (float)(Math.Floor(scene.size.X / 256) + 2)
                            : frame.coord.X - (float)(Math.Floor(scene.size.X / 256) + 2),
                        frame.coord.Y
                        )
                    );
                }
                else
                {
                    frame.scenecoord = Vector2.Add(frame.lefttop, scene.center);
                }
            }

        }
    }
}
