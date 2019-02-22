using Controllers;
using System;
using System.Device.Location;
using System.Drawing;
using System.Numerics;
using System.Threading;
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
            scene.Paint += new PaintEventHandler(SceneHandler.Scene_Draw);
            scene.MouseDown += new MouseEventHandler(SceneHandler.Scene_MouseDown);
            scene.MouseMove += new MouseEventHandler(SceneHandler.Scene_MouseMoove);
            scene.Resize += new EventHandler(SceneHandler.Scene_Resize);

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += TileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();

            Thread TilerThread = new Thread(TileHandler.Initialization)
            {
                IsBackground = true
            };
            TilerThread.Start();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            SceneHandler.scene.ClearImagePool();
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

            Thread th2 = new Thread(() => Readimg(filename))
            {
                IsBackground = false
            };
            th2.Start();
        }


        public void Redraw()
        {
            scene.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SceneHandler.scene.ClearImagePool();
            TileHandler.GetScreenAt(TileHandler.CurrentZ);
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
                
                SceneHandler.AddTile(new Vector2(SceneHandler.scene.center.X, SceneHandler.scene.center.Y), image1);

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
