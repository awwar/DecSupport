using DSCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace CropProd
{
    public partial class LayerMakerDialog : Form
    {
        Image img;
        int left;
        int right;
        int top;
        int bottom;
        int clickx;
        int clicky;
        Pen pen = new Pen(Color.Red, 1f);

        public Dictionary<string, Bitmap> tiles = new Dictionary<string, Bitmap>();

        public LayerMakerDialog()
        {
            InitializeComponent();
            DragDrop += LayerMaker_DragDrop;
            DragEnter += LayerMaker_DragEnter;
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_Move; ;
            button1.Click += Button1_Click;
        }

        private void PictureBox1_Move(object sender, MouseEventArgs e)
        {
            clickx = e.X;
            clicky = e.Y;
            right = (int)Math.Ceiling((double)(img.Width / Settings.TileSize));
            bottom = (int)Math.Ceiling((double)(img.Height / Settings.TileSize));
            left = e.X % 256;
            top = e.Y % 256;

            pictureBox1.Invalidate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, ImageFormat.Png);
            Image img = Image.FromStream(ms);
            int tile = Settings.TileSize;
            int clicktileX = (int)Math.Ceiling((double)(clickx / tile));
            int clicktileY = (int)Math.Ceiling((double)(clicky / tile));
            for (int i = 0; i <= bottom; i++)
            {
                for (int j = 0; j <= right + 1; j++)
                {
                    Rectangle tileBounds = new Rectangle((j - 1) * tile + left, (i - 1) * tile + top, tile, tile);
                    Bitmap target = new Bitmap(tile, tile);
                    string filename = String.Format("{0}_{1}", j - clicktileX - 1, i - clicktileY - 1);
                    using (Graphics graphics = Graphics.FromImage(target))
                    {
                        graphics.Clear(Color.Transparent);
                        graphics.DrawImage(
                            img,
                            new Rectangle(0, 0, tile, tile),
                            tileBounds,
                            GraphicsUnit.Pixel);
                    }
                    tiles.Add(filename, target);
                }
            }
            ms.Close();
        }

        private void LayerMaker_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            int tile = Settings.TileSize;
            e.Graphics.FillRectangle(myBrush, new Rectangle(clickx - 5, clicky - 5, 10, 10));
            if (img != null)
            {
                pictureBox1.Image = img;
                for (int i = 0; i <= bottom; i++)
                {
                    for (int j = 0; j <= right + 1; j++)
                    {
                        e.Graphics.DrawRectangle(
                            pen,
                            (j - 1) * tile + left,
                            (i - 1) * tile + top,
                            tile,
                            tile
                        );
                    }
                }
            }
        }

        private void LayerMaker_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            using (FileStream myStream = new FileStream(files[0], FileMode.Open, FileAccess.Read))
            {
                img = Image.FromStream(myStream);
            }
            pictureBox1.Invalidate();
        }
    }
}
