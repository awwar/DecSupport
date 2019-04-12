using CropProd;
using Interfaces;
using Models;
using System;
using System.Device.Location;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace Controllers
{
    internal class SceneHandler
    {
        public TileHandler tileHandler;

        Scene scene;
        static Form1 form;

        Vector2 first = new Vector2(0, 0);
        Vector2 delta = new Vector2(0, 0);
        Vector2 last = new Vector2(0, 0);

        Thread TileThread;
        Pen pen = new Pen(Color.Red, 1f);

        public SceneHandler(Form1 getform)
        {
            form = getform;
            scene = new Scene(new Vector2(getform.Size.Width, getform.Size.Height));


            TileThread = new Thread(startTileHandler)
            {
                IsBackground = false
            };
            TileThread.Start();
        }

        void startTileHandler()
        {
            tileHandler = new TileHandler(ref scene);
            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += tileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();
        }

        public void Scene_Resize(object sender, EventArgs e)
        {
            scene.resize(new Vector2(
                form.scene.Width,
                form.scene.Height
            ));
            Refresh();
        }

        public void Scene_Draw(object sender, PaintEventArgs e)
        {
            scene.update(delta);
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawLine(pen, scene.position.X - 10, scene.position.Y, scene.position.X + 10, scene.position.Y);
            e.Graphics.DrawLine(pen, scene.position.X, scene.position.Y - 10, scene.position.X, scene.position.Y + 10);

            //Если поток не 
            if (tileHandler == null)
            {
                return;
            }

            IFrame[] frames = tileHandler.handle();

            if (frames != null)
            {

                foreach (Tile frame in frames)
                {
                    try
                    {
                        e.Graphics.DrawImage(
                            frame.image,
                            frame.screenposition.X,
                            frame.screenposition.Y,
                            frame.size.X,
                            frame.size.Y
                        );

                    }
                    catch
                    {
                        Console.WriteLine("image error");
                    }

                    e.Graphics.DrawRectangle(pen,
                        frame.screenposition.X,
                        frame.screenposition.Y,
                        256,
                        256
                    );
                    e.Graphics.DrawString(
                        frame.coordinate.ToString(),
                        new Font("Arial", 15),
                        new SolidBrush(Color.White),
                        frame.position.X + scene.position.X,
                        frame.position.Y + scene.position.Y
                    );
                }
            }

            delta = new Vector2(0, 0);
        }

        public void Scene_MouseMoove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                first = new Vector2(e.X, e.Y);
                delta = Vector2.Subtract(last, first);
                last = first;
                Refresh();
            }
        }

        public void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        public void Zoom(int zoomed)
        {
            int zoom = scene.zoom + zoomed;
            scene.zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            tileHandler.update();
        }

        public static void Refresh()
        {
            form.Redraw();
        }
    }
}
