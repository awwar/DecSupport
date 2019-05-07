using CropProd;
using Handlers;
using Interfaces;
using Models;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace DSCore
{
    class DecisionSupport<T> where T : IUserForm
    {
        internal TileHandler TileHandler { get; private set; }
        internal DataHandler DataHandler { get; private set; }
        internal Scene Scene { get => scene; }

        internal event Action OnNeedRedraw;

        private Scene scene;
        private static T form;
        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
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
            this.TileHandler = new TileHandler(ref scene);
            this.DataHandler = new DataHandler(ref scene);
            TileHandler.Redraw += OnNeedRedraw;
            DataHandler.Redraw += OnNeedRedraw;
        }

        public void OnResize()
        {
            scene.Resize(form.GetDrawableSize());
            TileHandler.Update();
            OnNeedRedraw();
        }

        public Frame[] OnDraw()
        {
            scene.Update(delta);
            delta = new Vector2(0, 0);
            Frame[] frames = new Frame[] { };

            //Если поток не 
            if (TileHandler == null || DataHandler == null)
            {
                return frames;
            }

            frames = TileHandler.Handle();

            frames = frames.Concat(DataHandler.Handle()).ToArray();

            return frames;
        }

        public void OnMouseMoove(int x, int y)
        {
            first = new Vector2(x, y);
            delta = Vector2.Subtract(last, first);
            last = first;
            OnNeedRedraw();
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
            form.RedrawLayerItem(proj.Layers);
            TileHandler.Update();
            return proj;
        }

        public void OnLayerDrop(string file)
        {
            form.RedrawLayerItem(
               DataHandler.AddLayer(file)
            );
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
            return proj;
        }

        public void OnLayerCreate(LayerMakerDialogData data)
        {
            DataHandler.CreateLayer(
                data.Tiles,
                data.Lat,
                data.Lon,
                data.FileName
            );
        }

        public void OnLayerDelete(Layer layer)
        {
            form.RedrawLayerItem(
                DataHandler.DeleteLayer(layer)
            );
        }
    }
}
