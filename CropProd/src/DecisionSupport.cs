using CropProd;
using Handlers;
using Interfaces;
using Models;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

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

        public void OnResize(object sender, EventArgs e)
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

        public void OnFileEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        public void OnFileDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) OnFileDrop(file);
        }

        public void OnFileDrop(string file)
        {
            string ext = Path.GetExtension(file);
            switch (ext)
            {
                case ".cpproj":
                    Project proj = DataHandler.OpenProject(file);
                    form.ChangeTitle(proj.Name);
                    for (int i = 0; i < proj.Layers.Length; i++)
                    {
                        form.DrawLayerItem(i);
                    }
                    TileHandler.Update();
                    break;
                case ".cplay":
                    DataHandler.AddLayer(file);
                    break;
                default:
                    MessageBox.Show("Неизвестное расширение проекта!");
                    break;
            }
        }

        public void OnOpenProject(object sender, EventArgs e)
        {
            string filename = form.ShowOpenFileDialog();
            OnFileDrop(filename);
           /* try
            {
            }
            catch (Exception exc)
            {
                form.ShowBouble(exc.Message);
            }*/

        }

        public void OnSaveProject(object sender, EventArgs e)
        {
            try
            {
                bool rez = DataHandler.SaveProject();
                if (!rez)
                {
                    string filename = form.ShowSaveFileDialog();
                    DataHandler.SaveProject(filename);
                }
            }
            catch (Exception exc)
            {
                form.ShowBouble(exc.Message);
            }
        }

        public void OnNewProject(object sender, EventArgs e)
        {
            try
            {
                CreateProjDialogData createProj = form.ShowCreateProjDialog();
                DataHandler.CreateProject(
                               createProj.Name,
                               createProj.Lat,
                               createProj.Lon);
                DataHandler.SaveProject();
                TileHandler.Update();
            }
            catch (Exception exc)
            {
                form.ShowBouble(exc.Message);
            }
        }

        public void OnLayerCreate(object sender, EventArgs e)
        {
            LayerMakerDialogData data = form.ShowLayerMakerDialog();

            DataHandler.CreateLayer(
                data.Tiles,
                data.Lat,
                data.Lon,
                data.FileName
            );
        }
    }
}
