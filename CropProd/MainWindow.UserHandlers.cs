﻿using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private void OnLayerCreate_Click(object sender, EventArgs e)
        {
            LayerMakerDialogData data = this.ShowLayerMakerDialog();
            decisionSupport.OnLayerCreate(data);
        }

        private void OnSaveProject_Click(object sender, EventArgs e)
        {
            try
            {
                decisionSupport.OnSaveProject();
            }
            catch (IOException)
            {
                string filename = this.ShowSaveFileDialog();
                decisionSupport.OnSaveProject(filename);
            }
        }

        private void OnOpenProject_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = this.ShowOpenFileDialog();
                decisionSupport.OnOpenProject(filename);
            }
            catch (Exception exc)
            {
                this.ShowBouble(exc.Message);
            }
        }

        private void OnNewProject_Click(object sender, EventArgs e)
        {
            try
            {
                CreateProjDialogData createProj = this.ShowCreateProjDialog();
                decisionSupport.OnNewProject(createProj);
            }
            catch (Exception exc)
            {
                this.ShowBouble(exc.Message);
            }
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
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
                        decisionSupport.OnOpenProject(file);
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
