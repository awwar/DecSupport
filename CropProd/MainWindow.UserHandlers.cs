using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private bool IsDecisionMode { get; set; } = false;
        private List<PointF> points = new List<PointF>() { };
        Vector2 delta;
        Vector2 Position { get; set; } = new Vector2(0, 0);
        HatchBrush myBrush = new HatchBrush(HatchStyle.Horizontal, Color.Blue, Color.FromArgb(90, 0, 0, 100));
        SolidBrush myBrush2 = new SolidBrush(Color.Red);

        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                delta = decisionSupport.OnMouseMoove(e.X, e.Y);
                Position = Vector2.Add(Position, delta);
                scene.Refresh();
            }

        }

        private void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                decisionSupport.OnMouseDown(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left && IsDecisionMode)
            {
                points.Add(new PointF(e.X - Position.X, e.Y - Position.Y));
                OnNeedRedraw();
            }
        }

        private void AcceptDecision_Click(object sender, EventArgs e)
        {
            IsDecisionMode = true;
        }

        private void BeginDecision_Click(object sender, EventArgs e)
        {
            Layer[] layers = new Layer[layerlist.Count];
            for (int i = 0; i < layerlist.Count; i++)
            {
                layers[i] = layerlist[i].Layer;
                layers[i].setMax = layerlist[i].Max;
                layers[i].setMin = layerlist[i].Min;
            }

            PointF[] pointarray = points.ToArray();
            for (int i = 0; i < pointarray.Length; i++)
            {
                pointarray[i].X += Position.X;
                pointarray[i].Y += Position.Y;
            }

            decisionSupport.OnBeginDecision(layers, pointarray);
        }

        private void CancelDecision_Click(object sender, EventArgs e)
        {
            points.Clear();
            IsDecisionMode = false;
            OnNeedRedraw();
        }

        private void DeleteLayer_Click(Layer layer)
        {
            RedrawLayerItem(
                decisionSupport.OnLayerDelete(layer)
            );
            decisionSupport.OnSaveProject();
        }

        private void OnLayerCreate_Click(object sender, EventArgs e)
        {

            Status.Text = "Создание нового слоя";
            try
            {
                LayerMakerDialogData data = ShowLayerMakerDialog();
                decisionSupport.OnLayerCreate(data);
            }
            catch (Exception exc)
            {
                ShowBouble(exc.Message);
            }

            Status.Text = "Ok";
        }

        private void OnSaveProject_Click(object sender, EventArgs e)
        {
            Status.Text = "Сохраняю...";
            try
            {
                decisionSupport.OnSaveProject();
            }
            catch (IOException)
            {
                string filename = ShowSaveFileDialog();
                decisionSupport.OnSaveProject(filename);
            }
            Status.Text = "Ok";
        }

        private void OnOpenProject_Click(object sender, EventArgs e)
        {
            OnOpenProject_Click();
        }

        private void OnOpenProject_Click()
        { 
            string filename = ShowOpenFileDialog();
            OnOpenProject_Click(filename);
        }

        private void OnOpenProject_Click(string filename)
        {
            try
            {
                Status.Text = "Открываю проект";
                Project proj = decisionSupport.OnOpenProject(filename);
                RedrawLayerItem(proj.Layers);
                Text = proj.Name;
            }
            catch (Exception exc)
            {
                ShowBouble(exc.Message);
            }
            Status.Text = "Ok";
        }


        private void OnNewProject_Click(object sender, EventArgs e)
        {
            Status.Text = "Создание нового проекта";
            try
            {
                CreateProjDialogData createProj = ShowCreateProjDialog();
                Project proj = decisionSupport.OnNewProject(createProj);
                string filename = ShowSaveFileDialog(proj.Name);
                decisionSupport.OnSaveProject(filename);
                Text = proj.Name;
                ShowBouble($"Проект {proj.Name} успешно создан!");
            }
            catch (Exception exc)
            {
                ShowBouble(exc.Message);
            }
            Status.Text = "Ok";
        }

        private void Scene_Resize(object sender, EventArgs e)
        {
            decisionSupport.OnResize();
        }

        private void Scene_Paint(object sender, PaintEventArgs e)
        { 
            Frame[] frames = decisionSupport.OnDraw();
            e.Graphics.Clear(Color.Black);

            if (frames != null)
            {

                foreach (Frame frame in frames)
                {
                    e.Graphics.DrawImage(
                        frame.Image,
                        (float)Math.Floor(frame.Screenposition.X),
                        (float)Math.Floor(frame.Screenposition.Y),
                        frame.Size.X,
                        frame.Size.Y
                    );
                    e.Graphics.DrawString(frame.Screenposition.ToString(), new Font("Areal", 12), myBrush2, frame.Screenposition.X,
                         frame.Screenposition.Y);
                    e.Graphics.DrawString((frame.Screenposition + new Vector2(256,256)).ToString(), new Font("Areal", 12), myBrush2, frame.Screenposition.X + 244,
                        frame.Screenposition.Y + 244);
                }
            }

            if (IsDecisionMode)
            {
                GraphicsPath graphPath = new GraphicsPath();
                if(points.Count > 2)
                {

                    for (int i = 0; i < points.Count; i++)
                    {
                        float x = points[i].X + Position.X;
                        float y = points[i].Y + Position.Y;
                        if(i == points.Count - 1)
                        {
                            graphPath.AddLine(x, y, points[0].X + Position.X, points[0].Y + Position.Y);
                        } else
                        {
                            graphPath.AddLine(x, y, points[i + 1].X + Position.X, points[i + 1].Y + Position.Y);
                        }
                        DrawXmark(pen3, ref e, x, y);
                        e.Graphics.DrawString(String.Format("{0}_{1}", x, y), new Font("Areal", 12), myBrush2, x,y);
                    }
                    e.Graphics.FillPath(myBrush, graphPath);
                } else
                {
                    foreach (PointF item in points)
                    {
                        DrawXmark(pen3, ref e, item.X + Position.X, item.Y + Position.Y);
                    }
                }

            }
            /*
            DrawXmark(pen, ref e, decisionSupport.Scene.Position.X, decisionSupport.Scene.Position.Y);
            DrawXmark(pen2, ref e, decisionSupport.Scene.Size.X / 2, decisionSupport.Scene.Size.Y / 2);*/
        }
        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainWindow_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);
                switch (ext)
                {
                    case ".cpproj":
                        OnOpenProject_Click(file);
                        break;
                    case ".cplay":
                        RedrawLayerItem(
                            decisionSupport.OnLayerDrop(file)
                        );
                        break;
                    default:
                        MessageBox.Show("Неизвестное расширение проекта!");
                        break;
                }
            }
            e.Effect = DragDropEffects.None;
            OnNeedRedraw();
        }

        public void RedrawLayerItem(Layer[] layers)
        {
            foreach (LayerListItem item in layerlist)
            {
                item.Dispose();
            }
            layerlist.Clear();
            for (int i = 0; i < layers.Length; i++)
            {
                LayerListItem item = new LayerListItem();
                item.CreateControl();
                item.Parent = LayerList;
                item.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                item.Width = item.Parent.Width - item.Parent.Padding.Left * 2;
                item.Layer = layers[i];
                item.LayName.Text = layers[i].Name;
                item.LayerDelete += DeleteLayer_Click;
                item.Location = new Point(item.Parent.Padding.Left, item.Parent.Padding.Top + item.Height * i + 10);
                item.Show();
                layerlist.Add(item);
            }
        }
    }
}
