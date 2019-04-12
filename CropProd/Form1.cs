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

        SceneHandler sceneHandler;

        public Form1()
        {
            InitializeComponent();
            sceneHandler = new SceneHandler(this);
            //Handle draw calls
            scene.MouseMove += new MouseEventHandler(sceneHandler.Scene_MouseMoove);
            scene.MouseDown += new MouseEventHandler(sceneHandler.Scene_MouseDown);
            scene.Paint += new PaintEventHandler(sceneHandler.Scene_Draw);
            scene.Resize += new EventHandler(sceneHandler.Scene_Resize);
            scene.MouseWheel += Scene_MouseWheel;

            GeoCoordinateWatcher _geoWatcher = new GeoCoordinateWatcher();

            _geoWatcher.PositionChanged += sceneHandler.tileHandler.GeoWatcherOnStatusChanged;

            _geoWatcher.Start();
        }

        private void Scene_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta > 0)
            {
                //UP
                sceneHandler.Zoom(1);
            } else
            {
                //down
                sceneHandler.Zoom(-1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SceneHandler.scene.ClearImagePool();
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

        }


        public void Redraw()
        {
            scene.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sceneHandler.tileHandler.GetScreenAt();
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

                //SceneHandler.AddTile(new Vector2(SceneHandler.scene.center.X, SceneHandler.scene.center.Y), image1);

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
