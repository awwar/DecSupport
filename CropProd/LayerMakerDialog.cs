using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace CropProd
{
    public struct LayerMakerDialogData
    {
        public Dictionary<string, Bitmap> Tiles;
        public string Lat;
        public string Lon;
        public string FileName;
    }

    public partial class LayerMakerDialog : Form
    {
        Image img;
        int rignt;
        int down;
        int clickTileX;
        int clickTileY;
        Pen pen = new Pen(Color.Red, 1f);
        Pen pen1 = new Pen(Color.Orange, 4f);

        public Dictionary<string, Bitmap> tiles = new Dictionary<string, Bitmap>();

        public LayerMakerDialog()
        {
            InitializeComponent();
            this.DragDrop += LayerMaker_DragDrop;
            this.DragEnter += LayerMaker_DragEnter; ;
            this.pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseClick += PictureBox1_Click;
            this.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
            var img = Image.FromStream(ms);
            int tileWidth = Settings.Settings.TileSize;
            int tileHeight = Settings.Settings.TileSize;
            
            for (int x = 0; x <= down; x++)
            {
                for (int y = 0; y <= rignt;  y++)
                {
                    Rectangle tileBounds = new Rectangle(y * tileWidth, x * tileHeight, tileWidth, tileHeight);
                    Bitmap target = new Bitmap(tileWidth, tileHeight);
                    string filename = String.Format("{0}_{1}", y - clickTileX, x - clickTileY);
                    using (Graphics graphics = Graphics.FromImage(target))
                    {
                        graphics.DrawImage(
                            img,
                            new Rectangle(0, 0, tileWidth, tileHeight),
                            tileBounds,
                            GraphicsUnit.Pixel);
                    }
                    tiles.Add(filename, target);
                }
            }
            ms.Close();
        }

        private void PictureBox1_Click(object sender, MouseEventArgs e)
        {
            clickTileX = (int)Math.Floor((double)(e.X / Settings.Settings.TileSize));
            clickTileY = (int)Math.Floor((double)(e.Y / Settings.Settings.TileSize));
            pictureBox1.Invalidate();
        }

        private void LayerMaker_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (img != null)
            {
                float tile = Settings.Settings.TileSize;
                pictureBox1.Image = img;
                for (int i = 0; i <= down; i++)
                {
                    for (int j = 0; j <= rignt; j++)
                    {
                        e.Graphics.DrawRectangle(
                            pen,
                            j * tile,
                            i * tile,
                            tile,
                            tile
                        );
                    }
                }
                e.Graphics.DrawRectangle(
                    pen1,
                    clickTileX * tile,
                    clickTileY * tile,
                    tile,
                    tile
                );
            }
        }

        private void LayerMaker_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            img = Image.FromFile(files[0]);
            rignt = (int)Math.Ceiling((double)(img.Width / Settings.Settings.TileSize));
            down = (int)Math.Ceiling((double)(img.Height / Settings.Settings.TileSize));
            pictureBox1.Invalidate();
        }
    }
}
