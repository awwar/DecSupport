﻿using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    class ReportHandler
    {
        private ReportLoader report = new ReportLoader();

        public void PrepareReport(Layer[] layers, PointF[] points, Frame[] tiles)
        {

            Dictionary<Layer, Bitmap> compileLayer = new Dictionary<Layer, Bitmap>();
            // Вычисляем границы области 
            float maxX = points[0].X, minX = points[0].X, maxY = points[0].Y, minY = points[0].Y;
            foreach (PointF item in points)
            {
                if (item.X > maxX)
                {
                    maxX = item.X;
                }
                else
                if (item.X < minX)
                {
                    minX = item.X;
                }
                if (item.Y > maxY)
                {
                    maxY = item.Y;
                }
                else
                if (item.Y < minY)
                {
                    minY = item.Y;
                }
            }
            foreach (Layer layer in layers)
            {
                compileLayer[layer] = CompileFrames(layer.Datas, maxX, minX, maxY, minY);
            }
            Bitmap cutoutimage = CompileCutOut(points, maxX, minX, maxY, minY);
            Bitmap tileimage = CompileFrames(tiles, maxX, minX, maxY, minY);
            Readimg(compileLayer, cutoutimage);
        }

        private bool isTileInRect(float x, float y, float maxX, float minX, float maxY, float minY)
        {
            if (
                ((x < maxX || x + 256 < maxX) && (x > minX || x + 256 > minX)) ||
                ((y < maxY || y + 256 < maxY) && (y > minY || y + 256 > minY))
            )
            {
                return true;
            }
            return false;
        }

        private Bitmap CompileFrames(Frame[] framse, float maxX, float minX, float maxY, float minY)
        {
            Bitmap bitmap = new Bitmap(Convert.ToInt32(maxX - minX), Convert.ToInt32(maxY - minY));
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            foreach (Frame data in framse)
            {
                if (isTileInRect(data.Screenposition.X, data.Screenposition.Y, maxX, minX, maxY, minY))
                {
                    g.DrawImage(data.Image, (float)Math.Floor(data.Screenposition.X - minX), (float)Math.Floor(data.Screenposition.Y - minY), 256, 256);
                }
            }
            return bitmap;
        }

        private Bitmap CompileCutOut(PointF[] points, float maxX, float minX, float maxY, float minY)
        {
            Bitmap bitmap = new Bitmap(Convert.ToInt32(maxX - minX), Convert.ToInt32(maxY - minY));
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);

            GraphicsPath graphPath = new GraphicsPath();

            // так же отрисовываем границу и параллельно подправляем точки
            // чтоб их залить правильно
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X -= minX;
                points[i].Y -= minY;
                float x = points[i].X;
                float y = points[i].Y;
                if (i == points.Length - 1)
                {
                    graphPath.AddLine(x, y, points[0].X, points[0].Y);
                }
                else
                {
                    graphPath.AddLine(x, y, points[i + 1].X - minX, points[i + 1].Y - minY);
                }
            }
            g.FillPath(new SolidBrush(Color.Red), graphPath);

            return bitmap;
        }

        private void Readimg(Dictionary<Layer, Bitmap> compileLayer, Bitmap cutoutimage)
        {
            Bitmap newimg = new Bitmap(cutoutimage.Width, cutoutimage.Height);
            try
            {
                int x, y;
                Color cutcolor;
                Color laycolor;
                // Loop through the images pixels to reset color.
                for (x = 0; x < newimg.Width; x++)
                {
                    for (y = 0; y < newimg.Height; y++)
                    {
                        cutcolor = cutoutimage.GetPixel(x, y);
                        if(cutcolor.A != 0)
                        {
                            foreach (KeyValuePair<Layer, Bitmap> item in compileLayer)
                            {
                                Layer lay = item.Key;
                                Bitmap img = item.Value;
                                laycolor = img.GetPixel(x, y);
                                float gate = lay.setMax - lay.setMin;
                                float colorpower = (laycolor.G * laycolor.B);
                                if (colorpower >= lay.setMin && colorpower <= lay.setMax)
                                {
                                    newimg.SetPixel(x, y, Color.FromArgb(0, 0, 255));
                                }
                                else
                                {
                                    newimg.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                                }
                            }
                        }
                        else
                        {
                            newimg.SetPixel(x, y, Color.FromArgb(0,0,0,0));
                        }
                    }
                }


            }
            catch (ArgumentException)
            {
              
            }
            newimg.Save(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\rezult.png");
        }

    }
}
