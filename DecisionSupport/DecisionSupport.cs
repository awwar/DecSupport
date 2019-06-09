using Handlers;
using Models;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;

namespace DSCore
{
    public class DecisionSupport
    {
        internal TileHandler TileHandler;
        internal LayerLoader LayerHandler;
        internal ReportHandler ReportHandler;

        public event Action OnNeedRedraw;

        private Scene scene;
        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
        private readonly Thread HandlersThread;

        public DecisionSupport()
        {
            scene = new Scene();

            HandlersThread = new Thread(Start)
            {
                IsBackground = true
            };
            HandlersThread.Start();
        }

        private void Start()
        {
            TileHandler = new TileHandler(ref scene);
            LayerHandler = new LayerLoader(ref scene);
            ReportHandler = new ReportHandler();
            TileHandler.Redraw += OnNeedRedraw;
            LayerHandler.Redraw += OnNeedRedraw;
        }

        public Vector2 OnResize(Vector2 size)
        {
            Vector2 pos = scene.Resize(size);
            if (TileHandler != null)
            {
                TileHandler.Update();
            }
            OnNeedRedraw();
            return pos;
        }


        public void OnBeginDecision(Layer[] layers, PointF[] points)
        {
            Frame[] tiles = TileHandler.Handle();
            ReportHandler.PrepareReport(layers, points, tiles);
        }

        public Report[] OnGetDecision()
        {
            return ReportHandler.Handle();
        }

        public Tile[] OnDrawTile()
        {
            Tile[] frames = new Tile[] { };

            if (TileHandler != null)
            {
                try
                {
                    frames = TileHandler.Handle();
                }
                catch (Exception)
                {
                }
            }

            return frames;
        }

        public Layer[] OnDrawLayer()
        {
            Layer[] layers = new Layer[] { };

            if (LayerHandler != null)
            {
                try
                {
                    layers = LayerHandler.Handle();
                }
                catch (Exception)
                {
                }
            }

            return layers;
        }

        public Vector2 OnMouseMoove(int x, int y)
        {
            first = new Vector2(x, y);
            delta = Vector2.Subtract(last, first);
            scene.Update(delta);
            last = first;
            return delta;
        }

        public void OnMouseDown(int x, int y)
        {
            last = new Vector2(x, y);
        }

        public void OnZoom(int delta)
        {
            int zoom = scene.Zoom + ((delta > 0) ? 1 : -1);
            scene.Zoom = (zoom <= 0) ? 1 : (zoom > 18) ? 18 : zoom;
            TileHandler.Update();
            OnNeedRedraw();
        }

        public Project OnOpenProject(string file)
        {
            Project proj = LayerHandler.OpenProject(file);
            scene.AppendProject(proj);
            TileHandler.Update();
            OnNeedRedraw();
            return proj;
        }

        public Layer[] OnOpenLayer(string file)
        {
            Layer[] layers = LayerHandler.OpenLayer(file);
            OnNeedRedraw();
            return layers;
        }

        public void OnSaveProject(string file = null)
        {
            bool rez = LayerHandler.SaveProject(file);
            if (!rez)
            {
                throw new IOException();
            }
        }

        public Project OnNewProject(CreateProjDialogData createProj)
        {
            Project proj = LayerHandler.CreateProject(createProj);
            TileHandler.Update();
            OnNeedRedraw();
            return proj;
        }

        public void OnLayerCreate(LayerMakerDialogData data)
        {
            LayerHandler.CreateLayer(data);
        }

        public Layer[] OnLayerDelete(Layer layer)
        {
            Layer[] layers = LayerHandler.DeleteLayer(layer);
            OnNeedRedraw();
            return layers;
        }
    }
}
