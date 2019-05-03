using DSCore;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CropProd
{
    public partial class MainWindow : Form
    {
        private readonly DecisionSupport decisionSupport;

        public MainWindow()
        {
            InitializeComponent();
            decisionSupport = new DecisionSupport(this);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();

            //Handle draw calls
            // scene.MouseWheel    += decisionSupprot.OnZoom;
            scene.Paint         += decisionSupport.OnDraw;
            scene.Resize        += decisionSupport.OnResize;
            this.DragDrop       += decisionSupport.OnFileDrop;
            this.DragEnter      += decisionSupport.OnFileEnter;
            scene.MouseDown     += decisionSupport.OnMouseDown;
            scene.MouseMove     += decisionSupport.OnMouseClick;
            onNewProject.Click  += decisionSupport.OnNewProject;
            onOpenProject.Click += decisionSupport.OnOpenProject;
            onSaveProject.Click += decisionSupport.OnSaveProject;

            decisionSupport.OnNeedRedraw += OnNeedRedraw;

        }

        private void OnNeedRedraw()
        {
            scene.Refresh();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            decisionSupport.TileHandler.Update();
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
