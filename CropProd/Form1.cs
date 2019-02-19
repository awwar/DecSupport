using Controllers;
using System;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
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

            Thread th2 = new Thread(() => readimg(filename));
            th2.Start();
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
        private void readimg(string filename)
        {
            try
            {
                Bitmap image1;
                // Retrieve the image.
                image1 = new Bitmap(@filename, true);

                int x, y;

                // Loop through the images pixels to reset color.
                for (x = 0; x < image1.Width; x++)
                {
                    for (y = 0; y < image1.Height; y++)
                    {
                        Color pixelColor = image1.GetPixel(x, y);
                        Color newColor;
                        if (pixelColor.R > 100 && pixelColor.R < 150)
                        {
                            newColor = Color.FromArgb(pixelColor.R, 0, 0);
                        }
                        else
                        {
                            Random rnd = new Random();
                            int g = pixelColor.G;
                            int b = pixelColor.B;
                            newColor = Color.FromArgb(0, g, b);
                        }
                        image1.SetPixel(x, y, newColor);
                    }
                }

                // Set the PictureBox to display the image.
                SceneHandler.AddFrame(new Vector2(SceneHandler.scene.center.X, SceneHandler.scene.center.Y), image1, new double[2] { 0, 0 });

            }
            catch (ArgumentException)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }
            SceneHandler.Refresh();
        }
    }
    
}
