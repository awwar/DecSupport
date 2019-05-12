using CropProd;
using Handlers;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace DSCore
{
    public class DecisionSupport<T> where T : IUserForm
    {
        internal TileHandler TileHandler { get; private set; }
        internal LayerLoader DataHandler { get; private set; }
        internal ReportHandler ReportHandler { get; private set; }
        internal Scene Scene { get => scene; }

        internal event Action OnNeedRedraw;

        private readonly T form;
        private Scene scene;
        private Vector2 first   = new Vector2(0, 0);
        private Vector2 delta   = new Vector2(0, 0);
        private Vector2 last    = new Vector2(0, 0);
        private readonly Thread TileThread;

        public DecisionSupport(T myform)
        {
            form = myform;
            scene = new Scene(form.GetDrawableSize());
            form.ChangeTitle(scene.Name);

            TileThread = new Thread(Start)
            {
                IsBackground = true
            };
            TileThread.Start();
        }

        private void Start()
        {
            TileHandler = new TileHandler(ref scene);
            DataHandler = new LayerLoader(ref scene);
            ReportHandler = new ReportHandler();
            TileHandler.Redraw += OnNeedRedraw;
            DataHandler.Redraw += OnNeedRedraw;
        }

        public void OnResize()
        {
            scene.Resize(form.GetDrawableSize());
            TileHandler.Update();
            OnNeedRedraw();
        }


        public void OnBeginDecision(Layer[] layers, PointF[] points)
        {
            // Границы установленные пользователем - нативные, числовые а нам нужно перебить их в цветовой эквивалент
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].setMax = ((layers[i].setMax - layers[i].Min) * 65025) / (layers[i].Max - layers[i].Min);
                layers[i].setMin = ((layers[i].setMin - layers[i].Min) * 65025) / (layers[i].Max - layers[i].Min);
            }

            Frame[] tiles = TileHandler.Handle();
            ReportHandler.PrepareReport(layers, points, tiles);
        }

        public Frame[] OnDraw()
        {
            scene.Update(delta);
            Frame[] frames = new Frame[] { };
            //Если поток не 
            if (TileHandler != null || DataHandler != null)
            {
                frames = TileHandler.Handle();

                frames = frames.Concat(DataHandler.Handle()).ToArray();
            }

            return frames;
        }

        public Vector2 OnMouseMoove(int x, int y)
        {
            first = new Vector2(x, y);
            delta = Vector2.Subtract(last, first);
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
            Project proj = DataHandler.OpenProject(file);
            scene.AppendProject(proj);
            TileHandler.Update();
            OnNeedRedraw();
            return proj;
        }

        public Layer[] OnLayerDrop(string file)
        {
            return DataHandler.LoadLayer(file); ;
        }

        public void OnSaveProject(string file = null)
        {
            bool rez = DataHandler.SaveProject(file);
            if (!rez)
            {
                throw new IOException();
            }
        }

        public Project OnNewProject(CreateProjDialogData createProj)
        {
            Project proj = DataHandler.CreateProject(
                            createProj.Name,
                            createProj.Lat,
                            createProj.Lon);
            TileHandler.Update();
            OnNeedRedraw();
            return proj;
        }

        public void OnLayerCreate(LayerMakerDialogData data)
        {
            DataHandler.CreateLayer(
                data.Tiles,
                data.Lat,
                data.Lon,
                data.Min,
                data.Max,
                data.FileName
            );
        }

        public Layer[] OnLayerDelete(Layer layer)
        {
            return DataHandler.DeleteLayer(layer);
        }
    }
}
