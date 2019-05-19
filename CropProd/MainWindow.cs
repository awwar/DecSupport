using DSCore;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            decisionSupport = new DecisionSupport();

            UpdateStyles();

            Settings.Settings.TileSize = 256;

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
                            data.Min = layerMaker.MinInput.Text;
                            data.Max = layerMaker.MaxInput.Text;
                            data.FileName = sfd.FileName;
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
