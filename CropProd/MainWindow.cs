using DSCore;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form
    {
        private DecisionSupport decisionSupport;
        private readonly Pen pen3 = new Pen(Color.Orange, 2f);

        private List<LayerListItem> layerlist = new List<LayerListItem>();

        bool isdraw = false;

        public MainWindow(string[] args)
        {
            InitializeComponent();

            UpdateStyles();

            Settings.TempPath = Path.GetTempPath();
            Settings.TileSize = 256;
            Settings.DefaultTileImage = CropProd.Properties.Resources.def;
            Settings.DistributorName = "Google";
            Settings.DistributorSrc = "http://mt2.google.com/vt/lyrs=s&x={0}&y={1}&z={2}";
            /*
            * https://a.tile.openstreetmap.org/{2}/{0}/{1}.png
            * http://mt2.google.com/vt/lyrs=s&x={0}&y={1}&z={2}
            * http://c.tile.stamen.com/watercolor/{2}/{0}/{1}.jpg
            * https://khms1.googleapis.com/kh?v=821&x={0}&y={1}&z={2}
            * https://tile1.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.5&r=g&ts=online_sd
            * https://sat01.maps.yandex.net/tiles?l=sat&v=3.449.0&x={0}&y={1}&z={2}&lang=ru_RU
            */

            decisionSupport = new DecisionSupport();

            scene.Paint += Scene_Paint;
            scene.Resize += Scene_Resize;
            scene.MouseDown += Scene_MouseDown;
            scene.Invalidated += Scene_Invalidated;
            DragDrop += MainWindow_DragDrop;
            DragEnter += MainWindow_DragEnter;
            scene.MouseMove += Scene_MouseMove;
            onNewProject.Click += OnNewProject_Click;
            onOpenProject.Click += OnOpenProject_Click;
            onSaveProject.Click += OnSaveProject_Click;
            onLayerCreate.Click += OnLayerCreate_Click;
            BeginDecision.Click += BeginDecision_Click;
            RegionDecision.Click += AcceptDecision_Click;
            CancelDecision.Click += CancelDecision_Click;

            decisionSupport.OnNeedRedraw += OnNeedRedraw;

            ChangeTitle("Проект принятия решения");

            decisionSupport.OnResize(new Vector2(scene.Size.Width, scene.Size.Height));
            if (args.Length > 0)
            {
                MessageBox.Show("Открыть: " + args[0] + "?");
                OnOpenProject_Click(args[0]);
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
                Min = null,
                Max = null,
                FilePath = null
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
                            data.Min = layerMaker.MinInput.Text;
                            data.Max = layerMaker.MaxInput.Text;
                            data.ValueType = layerMaker.ValueTypeInput.Text;
                            data.FilePath = sfd.FileName;
                        }
                    }
                }
            }
            else
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

        private void DrawFrame(Frame frame, ref PaintEventArgs e)
        {
            e.Graphics.DrawImage(
                frame.Image,
                (float)Math.Floor(frame.ScreenPosition.X),
                (float)Math.Floor(frame.ScreenPosition.Y),
                frame.Size.X,
                frame.Size.Y
            );
        }

        private void OnNeedRedraw()
        {
            if (!isdraw)
            {
                isdraw = true;
                scene.Invalidate(true);
            }
        }


        private void Scene_Invalidated(object sender, InvalidateEventArgs e)
        {
            isdraw = false;
        }


    }

}
