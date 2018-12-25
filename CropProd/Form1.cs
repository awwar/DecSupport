using Controllers;
using System;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using System.Windows.Forms;

namespace CropProd
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            SceneHandler.Initialization(this);
            //Handle draw calls
            scene.Paint += new PaintEventHandler(SceneHandler.Draw);
            scene.MouseDown += new MouseEventHandler(SceneHandler.Scene_MouseDown);
            scene.MouseMove += new MouseEventHandler(SceneHandler.Scene_MouseMoove);

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += TileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SceneHandler.scene.clearImagePool();
            string filename = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "d:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filename = ofd.FileName;
                    label1.Text = "Открыт файл " + filename;
                }
            }
            Image img = Image.FromFile(filename);
            SceneHandler.AddFrame(new Vector2(SceneHandler.scene.center.X, SceneHandler.scene.center.Y), img,new double[2] {0,0});
            scene.Refresh();
        }


        public void Redraw()
        {
            scene.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SceneHandler.scene.clearImagePool();
            TileHandler.GetScreenAt(TileHandler.CurrentLat, TileHandler.CurrentLon, TileHandler.CurrentZ);
            scene.Refresh();
        }
    }
}
