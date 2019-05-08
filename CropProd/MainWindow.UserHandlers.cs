using Interfaces;
using Models;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == Keys.Control.ToString())
            {

            }
        }

        private void DeleteLayer_Click(Layer layer)
        {
            decisionSupport.OnLayerDelete(layer);
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

        private void Scene_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                decisionSupport.OnMouseMoove(e.X, e.Y);
            }
        }

        private void Scene_MouseDown(object sender, MouseEventArgs e)
        {
            decisionSupport.OnMouseDown(e.X, e.Y);
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
                        decisionSupport.OnLayerDrop(file);
                        break;
                    default:
                        MessageBox.Show("Неизвестное расширение проекта!");
                        break;
                }
            }
        }
    }
}
