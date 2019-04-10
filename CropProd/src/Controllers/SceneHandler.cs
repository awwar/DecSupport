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
    class SceneHandler
    {
        public Scene scene;
        static public Form1 form;
        public TileHandler tileHandler;

        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
        private int zoom = 18;
        private readonly Thread TilerThread;
        private readonly Pen pen = new Pen(Color.Red, 1f);

        public SceneHandler(Form1 getform)
        {
            form = getform;
            scene = new Scene(new Vector2(getform.Size.Width, getform.Size.Height));
            tileHandler = new TileHandler(ref scene);
        }

        public void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            scene.Lat = e.Position.Location.Latitude;
            scene.Lon = e.Position.Location.Longitude;
            recalculateSceneTilePosition();
        }

        private void recalculateSceneTilePosition()
        {
            double[] rez = tileHandler.LatLonToMeters(scene.Lat, scene.Lon, scene.zoom);
            scene.setTileCenter(
                new Vector2((float) rez[0], (float) rez[1])
            );
        }

        public void Scene_Resize(object sender, EventArgs e)
        {
            scene.size = new Vector2(
                form.scene.Width,
                form.scene.Height
            );
            Refresh();
        }

        public void Scene_Draw(object sender, PaintEventArgs e)
        {
            scene.update(delta);
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawLine(pen, scene.position.X - 10, scene.position.Y, scene.position.X + 10, scene.position.Y);
            e.Graphics.DrawLine(pen, scene.position.X, scene.position.Y - 10, scene.position.X, scene.position.Y + 10);
            IFrame[] frames = tileHandler.draw();
            /*
             * Вызвать ВСЕ ТАЙЛЫ НА ОТРИСОВКУ!
             * **/
            if (frames != null)
            {
                /*
                 * Можно сипользовать for next (и даже лучше)
                 * **/
                foreach (Tile frame in frames)
                {

                    e.Graphics.DrawImage(
                        frame.image,
                        frame.screenposition.X,
                        frame.screenposition.Y,
                        frame.size.X,
                        frame.size.Y
                    );

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
                    //UpdateFrame(frame);
                }
            }
            /*
             * ВЫЗВАТЬ TILEUPDATE!
             * **/
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
            zoom += zoomed;
            scene.zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            GC.Collect();
            tileHandler.Zoom();
        }

        public static  void Refresh()
        {
            form.Redraw();
        }

 
    }
}
