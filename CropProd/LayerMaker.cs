using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CropProd
{
    public partial class LayerMaker : Form
    {
        Image img;
        int rignt;
        int down;
        int clickTileX;
        int clickTileY;
        Pen pen = new Pen(Color.Red, 1f);
        Pen pen1 = new Pen(Color.Orange, 4f);

        public Dictionary<string, Bitmap> tiles = new Dictionary<string, Bitmap>();

        public LayerMaker()
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
            int tileWidth = 256;
            int tileHeight = 256;
            
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
            ms.Dispose();
        }

        private void PictureBox1_Click(object sender, MouseEventArgs e)
        {
            clickTileX = (int)Math.Floor((double)(e.X / 256));
            clickTileY = (int)Math.Floor((double)(e.Y / 256));
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

                pictureBox1.Image = img;
                for (int i = 0; i <= down; i++)
                {
                    for (int j = 0; j <= rignt; j++)
                    {
                        e.Graphics.DrawRectangle(
                            pen,
                            j * 256,
                            i * 256,
                            256,
                            256
                        );
                    }
                }
                e.Graphics.DrawRectangle(
                    pen1,
                    clickTileX * 256,
                    clickTileY * 256,
                    256,
                    256
                );
            }
        }

        private void LayerMaker_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            img = Image.FromFile(files[0]);
            rignt = (int)Math.Ceiling((double)(img.Width / 256));
            down = (int)Math.Ceiling((double)(img.Height / 256));
            pictureBox1.Invalidate();
        }
    }
}
