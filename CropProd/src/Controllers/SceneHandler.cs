using CropProd;
using Interfaces;
using LatLonToTile;
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
        public TileHandler TileHandler { get; private set; }
        public DataHandler DataHandler { get; private set; }

        private Scene scene;
        private static Form1 form;
        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
        private readonly Thread TileThread;
        private readonly Pen pen = new Pen(Color.Red, 1f);

        public SceneHandler(Form1 getform)
        {
            form = getform;
            scene = new Scene(new Vector2(getform.Size.Height));


            TileThread = new Thread(StartTileHandler)
            {
                IsBackground = false
            };
            TileThread.Start();
        }

        private void StartTileHandler()
        {
            TileHandler = new TileHandler(ref scene);
            DataHandler = new DataHandler(ref scene);

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += TileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();
        }

        public void Scene_Resize(object sender, EventArgs e)
        {
            scene.Resize(new Vector2(
                form.scene.Width,
                form.scene.Height
            ));
            Refresh();
        }

        public void Scene_Draw(object sender, PaintEventArgs e)
        {
            scene.Update(delta);
            e.Graphics.Clear(Color.Black);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;


            //Если поток не 
            if (TileHandler == null)
            {
                return;
            }

            Frame[] frames = TileHandler.Handle();

            if (frames != null)
            {

                foreach (Tile frame in frames)
                {
                    try
                    {
                        e.Graphics.DrawImage(
                            frame.Image,
                            frame.Screenposition.X,
                            frame.Screenposition.Y,
                            frame.Size.X,
                            frame.Size.Y
                        );

                    }
                    catch
                    {
                        Console.WriteLine("image error");
                    }
                    e.Graphics.DrawRectangle(pen, frame.Screenposition.X, frame.Screenposition.Y, 256, 256);
                }
            }

            frames = DataHandler.Handle();

            if (frames != null)
            {

                foreach (Frame frame in frames)
                {
                    try
                    {
                        e.Graphics.DrawImage(
                            frame.Image,
                            frame.Screenposition.X,
                            frame.Screenposition.Y,
                            frame.Size.X,
                            frame.Size.Y
                        );

                    }
                    catch
                    {
                        Console.WriteLine("image error");
                    }
                }
            }

            e.Graphics.DrawLine(new Pen(Color.Green, 3f), scene.position.X - 10, scene.position.Y, scene.position.X + 10, scene.position.Y);
            e.Graphics.DrawLine(new Pen(Color.Green, 3f), scene.position.X, scene.position.Y - 10, scene.position.X, scene.position.Y + 10);
            e.Graphics.DrawLine(pen, scene.size.X / 2 - 10, scene.size.Y / 2, scene.size.X / 2 + 10, scene.size.Y / 2);
            e.Graphics.DrawLine(pen, scene.size.X / 2, scene.size.Y / 2 - 10, scene.size.X / 2, scene.size.Y / 2 + 10);
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
            TileHandler.Update();
            Refresh();
        }

        public static void Refresh()
        {
            form.Redraw();
        }
    }
}
