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
            SceneHandler.form = this;
            //Handle draw calls
            scene.Paint += new PaintEventHandler(SceneHandler.Draw);
            scene.MouseDown += new MouseEventHandler(SceneHandler.Scene_MouseDown);
            scene.MouseMove += new MouseEventHandler(SceneHandler.Scene_MouseMoove);

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += GoogleTileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();

            GoogleTileHandler.GetScreenAt(GoogleTileHandler.CurrentLat, GoogleTileHandler.CorrentLon, GoogleTileHandler.CorrentZ);
        }
        private void button1_Click(object sender, EventArgs e)
        {
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
            SceneHandler.AddFrame(new Vector2(0, 0), img);
            SceneHandler.AddFrame(new Vector2(0, img.Width), img);
            scene.Refresh();
        }
        

        public void Redraw()
        {
            scene.Invalidate();
        }
    }
}
