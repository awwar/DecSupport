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
            float maxX = points[0].X, minX = points[0].X, maxY = points[0].Y, minY = points[0].Y;
            List<Data> newlist = new List<Data>();
            foreach (PointF item in points)
            {
                if(item.X > maxX)
                {
                    maxX = item.X;
                }
                else
                if (item.X < minX)
                {
                    minX = item.X;
                }
                if (item.Y > maxY)
                {
                    maxY = item.Y;
                }
                else
                if (item.Y < minY)
                {
                    minY = item.Y;
                }
            }
            Bitmap bitmap;
            Graphics g;
            foreach (Layer layer in layers)
            {
                bitmap = new Bitmap(Convert.ToInt32(maxX - minX), Convert.ToInt32(maxY - minY));
                g = Graphics.FromImage(bitmap);
                // Add drawing commands here
                g.Clear(Color.Transparent);
                foreach (Data data in layer.Datas)
                {
                    if(
                       ((data.Screenposition.X < maxX ||  data.Screenposition.X + 255 < maxX) && (data.Screenposition.X > minX || data.Screenposition.X + 255 > minX)) ||
                       ((data.Screenposition.Y < maxY || data.Screenposition.Y + 255 < maxY) && (data.Screenposition.Y > minY || data.Screenposition.Y + 255 > minY))
                    )
                    {
                        g.DrawImage(data.Image, (float)Math.Floor(data.Screenposition.X - minX), (float)Math.Floor(data.Screenposition.Y - minY), 256, 256);
                    }
                }
                bitmap.Save(String.Format(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\{0}.png", layer.Hash), ImageFormat.Png);
            }
            bitmap = new Bitmap(Convert.ToInt32(maxX - minX), Convert.ToInt32(maxY - minY));
            g = Graphics.FromImage(bitmap);
            // Add drawing commands here
            g.Clear(Color.Transparent);

            GraphicsPath graphPath = new GraphicsPath();

            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X;
                float y = points[i].Y;
                if (i == points.Length - 1)
                {
                    graphPath.AddLine(x, y, points[0].X, points[0].Y );
                }
                else
                {
                    graphPath.AddLine(x, y, points[i + 1].X, points[i + 1].Y);
                }
            }
            g.FillPath(new SolidBrush(Color.Red), graphPath);

            bitmap.Save(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\cut.png", ImageFormat.Png);
            /*ReportLoader report = new ReportLoader();
            report.MakePDF(img);*/
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
                data.Max,
                data.Min,
                data.FileName
            );
        }

        public Layer[] OnLayerDelete(Layer layer)
        {
            return DataHandler.DeleteLayer(layer);
        }
    }
}
