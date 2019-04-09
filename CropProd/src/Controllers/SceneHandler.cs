using CropProd;
using Models;
using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using static Controllers.ImageLoader;

namespace Controllers
{
    static class SceneHandler
    {
        public static Scene scene;
        public static Form1 form;
        public static ImageLoader Loader;
        private static Vector2 first = new Vector2(0, 0);
        private static Vector2 delta = new Vector2(0, 0);
        private static Vector2 last = new Vector2(0, 0);
        private static int zoom = 18;
        private static Thread TilerThread;
        private static readonly Pen pen = new Pen(Color.Red, 1f);

        public static void Initialization(Form1 getform)
        {
            form = getform;
            scene = new Scene(new Vector2(
                form.scene.Width,
                form.scene.Height
            ));

            Loader = new ImageLoader();
            AddFrame(new Vector2(1, 1), null);
            AddFrame(new Vector2(0, 0), null);
            TilerThread = new Thread(TileHandler.Initialization)
            {
                IsBackground = true
            };
            TilerThread.Start();
        }

        public static void Scene_Resize(object sender, EventArgs e)
        {
            scene.size = new Vector2(
                form.scene.Width,
                form.scene.Height
            );
            Refresh();
        }

        public static void Scene_Draw(object sender, PaintEventArgs e)
        {
            scene.update(delta);
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawLine(pen, scene.position.X - 10, scene.position.Y, scene.position.X + 10, scene.position.Y);
            e.Graphics.DrawLine(pen, scene.position.X, scene.position.Y - 10, scene.position.X, scene.position.Y + 10);
            Tile[] frames = scene.drawScene().ToArray();

            if (frames != null)
            {
                foreach (Tile frame in frames)
                {

                    e.Graphics.DrawImage(frame.image, frame.draw().X, frame.draw().Y, 256, 256);
                    e.Graphics.DrawRectangle(pen, frame.draw().X, frame.draw().Y, 256, 256);
                    e.Graphics.DrawString(
                        "X: " + frame.screenposition.X + "_Y: " + frame.screenposition.Y,
                        new Font("Arial", 15),
                        new SolidBrush(Color.White),
                        frame.position.X + scene.position.X,
                        frame.position.Y + scene.position.Y
                    );
                    UpdateFrame(frame);
                }

            }
            delta = new Vector2(0, 0);
        }

        public static void Scene_MouseMoove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                first = new Vector2(e.X, e.Y);
                delta = Vector2.Subtract(last, first);
                last = first;
                Refresh();
            }
        }

        public static void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        public static void Zoom(int zoomed)
        {
            zoom += zoomed;
            zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            TileHandler.CurrentZ = zoom;
            Loader.ClearPool();
            scene.clearFramePool();
            GC.Collect();
            TileHandler.GetScreenAt();
        }

        public static void AddFrame(Vector2 point, Image image)
        {
            Tile tile = new Tile(
                point,
                image,
                ref scene
            );
            AddFrame(tile);
        }

        public static void AddFrame(Tile frame)
        {
            scene.addFrame(frame);
            Refresh();
        }

        public static void LoadFrame(Tile frame)
        {
            AddFrame(frame);
            Loader.AddFrame(frame);
            Loader.onImageLoad += new ImageLoadHandler(frame.ImageLoaded);
        }

        public static void Refresh()
        {
            form.Redraw();
        }

        private static void UpdateFrame(Tile frame)
        {
            if (frame.screenposition.Y < -256 * 2 || frame.screenposition.Y > scene.size.Y + 256)
            {
                scene.removeFrame(frame);
                TileHandler.GetTileAt(new Vector2(
                    frame.coordinate.X,
                    (frame.screenposition.Y < -256 * 2)
                        ? frame.coordinate.Y + (int) scene.size.Y / (256) + 3
                        : frame.coordinate.Y - (int) scene.size.Y / (256) - 3
                    )
                );
            }
            else if (frame.screenposition.X < -256 * 2 || frame.screenposition.X > scene.size.X + 256)
            {
                scene.removeFrame(frame);
                TileHandler.GetTileAt(
                    new Vector2(
                        (frame.screenposition.X < -256 * 2)
                            ? frame.coordinate.X + (int) scene.size.X / (256) + 3
                            : frame.coordinate.X - (int) scene.size.X / (256) - 3,
                    frame.coordinate.Y
                ));
            }
        }
    }
}
