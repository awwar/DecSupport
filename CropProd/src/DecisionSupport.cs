using CropProd;
using Handlers;
using Models;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace DSCore
{
    class DecisionSupport
    {
        internal TileHandler TileHandler { get; private set; }
        internal DataHandler DataHandler { get; private set; }
        internal event Action OnNeedRedraw;

        private Scene scene;
        private static MainWindow form;
        private Vector2 first = new Vector2(0, 0);
        private Vector2 delta = new Vector2(0, 0);
        private Vector2 last = new Vector2(0, 0);
        private readonly Thread TileThread;
        private readonly Pen pen = new Pen(Color.Red, 1f);

        public DecisionSupport(MainWindow myform)
        {
            form = myform;
            scene = new Scene(new Vector2(myform.Size.Height));


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

        public void OnResize(object sender, EventArgs e)
        {
            scene.Resize(new Vector2(
                form.scene.Width,
                form.scene.Height
            ));
            OnNeedRedraw();
        }

        public void OnDraw(object sender, PaintEventArgs e)
        {
            scene.Update(delta);
            e.Graphics.Clear(Color.Black);


            //Если поток не 
            if (TileHandler == null || DataHandler == null)
            {
                return;
            }

            Frame[] frames = TileHandler.Handle();

            frames = frames.Concat(DataHandler.Handle()).ToArray();


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
                            frame.Size.Y + 1
                        );
                    }
                    catch
                    {
                        Console.WriteLine("image error");
                    }
                    //e.Graphics.DrawRectangle(pen, frame.Screenposition.X, frame.Screenposition.Y, 256, 256);
                }
            }

            e.Graphics.DrawLine(new Pen(Color.Green, 3f), scene.Position.X - 10, scene.Position.Y, scene.Position.X + 10, scene.Position.Y);
            e.Graphics.DrawLine(new Pen(Color.Green, 3f), scene.Position.X, scene.Position.Y - 10, scene.Position.X, scene.Position.Y + 10);
            e.Graphics.DrawLine(pen, scene.Size.X / 2 - 10, scene.Size.Y / 2, scene.Size.X / 2 + 10, scene.Size.Y / 2);
            e.Graphics.DrawLine(pen, scene.Size.X / 2, scene.Size.Y / 2 - 10, scene.Size.X / 2, scene.Size.Y / 2 + 10);
            delta = new Vector2(0, 0);
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
            foreach (string file in files) DataHandler.OpenProject(file);
        }

        public void OnOpenProject(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "d:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filename = ofd.FileName;
                    DataHandler.OpenProject(filename);
                }
            }
        }

        public void OnSaveProject(object sender, EventArgs e)
        {
            DataHandler.SaveProject();
        }

        public void OnNewProject(object sender, EventArgs e)
        {
            CreateProjDialog createProj = new CreateProjDialog();

            if (createProj.ShowDialog() == DialogResult.OK)
            {
                if (createProj.LatInput.TextLength > 0
                    && createProj.LonInput.TextLength > 0
                    && createProj.ProjName.TextLength > 0)
                {
                    DataHandler.CreateProject(createProj.ProjName.Text,
                                           createProj.LatInput.Text,
                                           createProj.LonInput.Text);
                }
            }
            createProj.Dispose();
        }
    }
}
