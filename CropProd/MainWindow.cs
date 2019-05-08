using DSCore;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form, IUserForm
    {
        private DecisionSupport<MainWindow> decisionSupport;
        private readonly Pen pen = new Pen(Color.Red, 1f);
        private readonly Pen pen2 = new Pen(Color.Green, 4f);

        private List<LayerListItem> layerlist = new List<LayerListItem>();

        public MainWindow(string[] args)
        {
            InitializeComponent();
            decisionSupport = new DecisionSupport<MainWindow>(this);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();

            Settings.Settings.TileSize = 256;

            scene.Paint += Scene_Paint;
            scene.Resize += Scene_Resize;
            DragDrop += MainWindow_DragDrop;
            DragEnter += MainWindow_DragEnter;
            scene.MouseDown += Scene_MouseDown;
            scene.MouseMove += Scene_MouseMove;
            onNewProject.Click += OnNewProject_Click;
            onOpenProject.Click += OnOpenProject_Click;
            onSaveProject.Click += OnSaveProject_Click;
            onLayerCreate.Click += OnLayerCreate_Click;
            KeyPress += MainWindow_KeyPress;

            decisionSupport.OnNeedRedraw += OnNeedRedraw;


            if (args.Length > 0)
            {
                MessageBox.Show("Открыть: " + args[0] + "?");
                decisionSupport.OnOpenProject(args[0]);
            }
        }

        public Vector2 GetDrawableSize()
        {
            return new Vector2(scene.Width, scene.Height);
        }

        public void ChangeTitle(string title)
        {
            Text = title;
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

        public string ShowSaveFileDialog(string name = "")
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                sfd.Filter = "Crop Pod Projects (*.cpproj)|*.cpproj";
                sfd.FileName = name;
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
            } else
            {
                throw new Exception("Layer not created");
            }
            layerMaker.Dispose();
            return data;
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

        /*
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
        }*/
    }

}
