using DSCore;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CropProd
{
    public partial class Form1 : Form
    {
        private readonly DecisionSupport decisionSupprot;

        public Form1()
        {
            InitializeComponent();
            decisionSupprot = new DecisionSupport(this);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();

            //Handle draw calls
            // scene.MouseWheel    += decisionSupprot.OnZoom;
            scene.Paint         += decisionSupprot.OnDraw;
            scene.Resize        += decisionSupprot.OnResize;
            this.DragDrop      += decisionSupprot.OnFileDrop;
            this.DragEnter     += decisionSupprot.OnFileEnter;
            scene.MouseDown     += decisionSupprot.OnMouseDown;
            scene.MouseMove     += decisionSupprot.OnMouseClick;
            
            decisionSupprot.OnNeedRedraw += OnNeedRedraw;

        }

        private void OnNeedRedraw()
        {
            scene.Invalidate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "d:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //SceneHandler.scene.ClearImagePool();
                    string filename = ofd.FileName;
                    label1.Text = "Открыт файл " + filename;
                    Readimg(filename);
                }
            }

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            decisionSupprot.TileHandler.Update();
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
    }

}
