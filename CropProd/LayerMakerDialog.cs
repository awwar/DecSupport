﻿using DSCore;
using Models;
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
        int left;
        int right;        
        int width;
        int top;      
        int height;
        int bottom;     
        int clickx;
        int clicky;
        Pen pen = new Pen(Color.Red, 1f);
        Pen pen1 = new Pen(Color.Orange, 10f);

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
            right = (int)Math.Ceiling((double)(img.Width / Settings.Settings.TileSize));
            bottom = (int)Math.Ceiling((double)(img.Height / Settings.Settings.TileSize));
            left =  e.X % 256;
            top  =  e.Y % 256; ;

            pictureBox1.Invalidate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, ImageFormat.Png);
            Image img = Image.FromStream(ms);
            int tile = Settings.Settings.TileSize;
            int clicktileX = (int)Math.Ceiling((double)(clickx / tile));
            int clicktileY = (int)Math.Ceiling((double)(clicky / tile));
            for (int x = 0; x <= bottom; x++)
            {
                for (int y = 0; y <= right; y++)
                {
                    Rectangle tileBounds = new Rectangle(y * tile - left, x * tile - top, tile, tile);
                    Bitmap target = new Bitmap(tile, tile);
                    string filename = String.Format("{0}_{1}", y - clicktileX, x - clicktileY);
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
            ms.Dispose();
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
            int tile = Settings.Settings.TileSize;
            e.Graphics.FillRectangle(myBrush, new Rectangle(clickx - 5, clicky - 5, 10, 10));
            if (img != null)
            {
                pictureBox1.Image = img;
                for (int i = 0; i <= bottom; i++)
                {
                    for (int j = 0; j <= right; j++)
                    {
                        e.Graphics.DrawRectangle(
                            pen,
                            j * tile - left,
                            i * tile - top,
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
                myStream.Close();
            }
            width = img.Width;
            height = img.Height;/*
            width = (int)Math.Ceiling((double)(img.Width / Settings.Settings.TileSize));
            height = (int)Math.Ceiling((double)(img.Height / Settings.Settings.TileSize));*/
            pictureBox1.Invalidate();
        }
    }
}
