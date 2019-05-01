using Controllers;
using Events;
using Models;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace DecisionSupport
{
    public class DecisionSupportCore
    {
        internal TileHandler TileHandler { get; private set; }
        internal DataHandler DataHandler { get; private set; }
        public event Action OnNeedRedraw;

        private Scene scene;
        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
        private readonly Thread TileThread;

        public DecisionSupportCore(float Width, float Height)
        {
            scene = new Scene(new Vector2(Width, Height));


            TileThread = new Thread(() =>
            {
                TileHandler = new TileHandler(ref scene, OnNeedRedraw);
                DataHandler = new DataHandler(ref scene, OnNeedRedraw);
            })
            {
                IsBackground = false
            };
            TileThread.Start();
        }

        public void OnResize(object sender, FormResizeArgs e)
        {
            scene.Resize(new Vector2(
                e.Width,
                e.Height
            ));
            OnNeedRedraw();
        }

        public Frame[] OnDraw()
        {
            scene.Update(delta);
            delta = new Vector2(0, 0);
            Frame[] frames = { };
            if (TileHandler == null)
            {
                return frames;
            }

            frames = TileHandler.Handle();
            return frames.Concat(DataHandler.Handle()).ToArray();
        }

        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                first = new Vector2(e.X, e.Y);
                delta = Vector2.Subtract(last, first);
                last = first;
                OnNeedRedraw();
            }
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            last = new Vector2(e.X, e.Y);
        }

        public void OnZoom(object sender, MouseEventArgs e)
        {
            int zoom = scene.Zoom + ((e.Delta > 0) ? 1 : -1);
            scene.Zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            TileHandler.Update();
            OnNeedRedraw();
        }
    }
}
