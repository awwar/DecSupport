using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private bool Iscontrollpress { get; set; } = false;
        private bool IsDecisionMode { get; set; } = false;
        private List<PointF> points = new List<PointF>() { };
        Vector2 delta;
        Vector2 Position { get; set; } = new Vector2(0, 0);
        SolidBrush myBrush = new SolidBrush(System.Drawing.Color.Red);

        private void MainWindow_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                Iscontrollpress = true;
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.ControlKey | e.KeyData == Keys.Control)
            {
                Iscontrollpress = false;
            }
        }

        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Iscontrollpress)
            {
                delta = decisionSupport.OnMouseMoove(e.X, e.Y);
                Position = Vector2.Add(Position, delta);
            }

        }

        private void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Iscontrollpress)
            {
                decisionSupport.OnMouseDown(e.X, e.Y);
            }
            if (IsDecisionMode && !Iscontrollpress)
            {
                points.Add(new PointF(e.X - Position.X, e.Y - Position.Y));
            }
        }

        private void AcceptDecision_Click(object sender, EventArgs e)
        {
            IsDecisionMode = true;
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
            try
            {
                Status.Text = "Открываю проект";
                string filename = ShowOpenFileDialog();
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
                }
            }

            if (IsDecisionMode)
            {
                foreach (PointF item in points)
                {
                    DrawXmark(pen2, ref e, item.X + Position.X, item.Y + Position.Y);
                }
            }
            e.Graphics.DrawString(Position.ToString(), new Font("Times new Roman", 12), myBrush, 0, 0);

            DrawXmark(pen, ref e, decisionSupport.Scene.Position.X, decisionSupport.Scene.Position.Y);
            DrawXmark(pen2, ref e, decisionSupport.Scene.Size.X / 2, decisionSupport.Scene.Size.Y / 2);
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
                        Project proj = decisionSupport.OnOpenProject(file);
                        Text = proj.Name;
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
