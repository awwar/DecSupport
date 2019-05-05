﻿using DSCore;
using Interfaces;
using Models;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private readonly DecisionSupport<MainWindow> decisionSupport;
        private readonly Pen pen = new Pen(Color.Red, 1f);
        private readonly Pen pen2 = new Pen(Color.Green, 4f);

        public MainWindow(string[] args)
        {
            InitializeComponent();
            decisionSupport = new DecisionSupport<MainWindow>(this);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();

            Settings.Settings.TileSize = 256;

            scene.Paint += Scene_Paint; ;
            scene.Resize += decisionSupport.OnResize;
            this.DragDrop += decisionSupport.OnFileDrop;
            this.DragEnter += decisionSupport.OnFileEnter;
            scene.MouseDown += decisionSupport.OnMouseDown;
            scene.MouseMove += decisionSupport.OnMouseClick;
            onNewProject.Click += decisionSupport.OnNewProject;
            onOpenProject.Click += decisionSupport.OnOpenProject;
            onSaveProject.Click += decisionSupport.OnSaveProject;
            onLayerCreate.Click += decisionSupport.OnLayerCreate;

            decisionSupport.OnNeedRedraw += OnNeedRedraw;


            if (args.Length > 0)
            {
                MessageBox.Show("Открыть: " + args[0] + "?");
                decisionSupport.OnFileDrop(args[0]);
            }
        }


        private void Readimg(string filename)
        {
            try
            {
                Bitmap image1;
                // Retrieve the image.
                image1 = new Bitmap(@filename, true);

                int x, y;
                Color pixelColor;
                Color newColor;
                // Loop through the images pixels to reset color.
                for (x = 0; x < image1.Width; x++)
                {
                    for (y = 0; y < image1.Height; y++)
                    {
                        pixelColor = image1.GetPixel(x, y);
                        if (pixelColor.R > 100 && pixelColor.R < 150)
                        {
                            newColor = Color.FromArgb(pixelColor.R, 0, 0);
                        }
                        else
                        {
                            int g = pixelColor.G;
                            int b = pixelColor.B;
                            newColor = Color.FromArgb(0, g, b);
                        }
                        image1.SetPixel(x, y, newColor);
                    }
                }


            }
            catch (ArgumentException)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }
        }

        public Vector2 GetDrawableSize()
        {
            return new Vector2(scene.Width, scene.Height);
        }

        public void ChangeTitle(string title)
        {
            this.Text = title;
        }

        public void DrawLayerItem(int n)
        {
            LayerListItem item = new LayerListItem();
            item.CreateControl();
            item.Parent = LayerList;
            item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            item.Width = item.Parent.Width - item.Parent.Padding.Left * 2;
            item.Location = new Point(item.Parent.Padding.Left, item.Parent.Padding.Top + item.Height * n + 10);
            item.Show();
        }

        public string ShowOpenFileDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                ofd.Filter = "Crop Pod Projects (*.cpproj)|*.cpproj";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
                else
                {
                    throw new Exception("File not choosen");
                }
            }
        }

        public string ShowSaveFileDialog()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                sfd.Filter = "Crop Pod Projects (*.cpproj)|*.cpproj";
                sfd.FilterIndex = 2;
                sfd.FileName = "";
                sfd.RestoreDirectory = false;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
                else
                {
                    throw new Exception("File not choosen");
                }
            }
        }

        public CreateProjDialogData ShowCreateProjDialog()
        {
            CreateProjDialogData data = new CreateProjDialogData();

            CreateProjDialog createProj = new CreateProjDialog();

            if (createProj.ShowDialog() == DialogResult.OK)
            {
                if (createProj.LatInput.TextLength > 0
                    && createProj.LonInput.TextLength > 0
                    && createProj.ProjName.TextLength > 0)
                {
                    data.Lat = createProj.LatInput.Text;
                    data.Lon = createProj.LonInput.Text;
                    data.Name = createProj.ProjName.Text;
                    createProj.Dispose();
                    return data;
                }
            }
            throw new Exception("Project not created");           
        }

        public void ShowBouble(string msg)
        {
            notifyIcon1.Icon = SystemIcons.Exclamation;
            notifyIcon1.BalloonTipTitle = "Warning";
            notifyIcon1.BalloonTipText = msg;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(90000);
        }

        public LayerMakerDialogData ShowLayerMakerDialog()
        {
            LayerMakerDialog layerMaker = new LayerMakerDialog();

            LayerMakerDialogData data = new LayerMakerDialogData()
            {
                Tiles = null,
                Lat = null,
                Lon = null,
                FileName = null
            };

            if (layerMaker.ShowDialog() == DialogResult.OK)
            {
                if (layerMaker.LatInput.TextLength > 0
                    && layerMaker.LonInput.TextLength > 0
                    && layerMaker.NameInput.TextLength > 0)
                {

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                        sfd.Filter = "Crop Pod Layer (*.cplay)|*.cplay";
                        sfd.FilterIndex = 2;
                        sfd.FileName = layerMaker.NameInput.Text;
                        sfd.RestoreDirectory = false;

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            data.Tiles = layerMaker.tiles;
                            data.Lat = layerMaker.LatInput.Text;
                            data.Lon = layerMaker.LonInput.Text;
                            data.FileName = sfd.FileName;              
                        }
                    }
                }
            }
            layerMaker.Dispose();
            return data;
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

        private void DrawXmark(Pen pen, ref PaintEventArgs e, float x, float y)
        {
            e.Graphics.DrawLine(pen, x - 10, y, x + 10, y);
            e.Graphics.DrawLine(pen, x, y - 10, x, y + 10);
        }

        private void OnNeedRedraw()
        {
            scene.Refresh();
        }


    }

}
