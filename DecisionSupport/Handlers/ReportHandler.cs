﻿using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Handlers
{
    class ReportHandler : IHandler<Report>
    {
        private ReportLoader Loader = new ReportLoader();
        List<Report> Reports = new List<Report>();
        private Pen pen = new Pen(Color.Blue, 2f);
        private Brush brush = new SolidBrush(Color.Blue);

        public Action Redraw { get; set; }

        public void PrepareReport(Layer[] layers, PointF[] points, Frame[] tiles)
        {
            // Границы установленные пользователем - нативные, числовые а нам нужно перебить их в цветовой эквивалент
            for (int i = 0; i < layers.Length; i++)
            {
                float layerpower = (layers[i].ColorMax - layers[i].ColorMin) / (layers[i].Max - layers[i].Min);
                layers[i].setMax = layers[i].Min + (layers[i].setMax - layers[i].Min) * layerpower;
                layers[i].setMin = layers[i].Min + (layers[i].setMin - layers[i].Min) * layerpower;
            }

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
            Bitmap cutouframe = CompileCutOut(points, maxX, minX, maxY, minY, false);
            Bitmap tileimage = CompileFrames(tiles, maxX, minX, maxY, minY);
            Bitmap newimg = Readimg(compileLayer, cutoutimage);
            try
            {
                Loader.SaveBitmaps(tileimage, newimg, cutouframe);
            }
            catch (Exception)
            {
                throw new Exception("PDF create error!");
            }
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

        private Bitmap CompileCutOut(PointF[] points, float maxX, float minX, float maxY, float minY, bool isfill = true)
        {
            Bitmap bitmap = new Bitmap(Convert.ToInt32(maxX - minX), Convert.ToInt32(maxY - minY));
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);

            GraphicsPath graphPath = new GraphicsPath();

            // так же отрисовываем границу и параллельно подправляем точки
            // чтоб их залить правильно
            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X - minX;
                float y = points[i].Y - minY;
                if (i == points.Length - 1)
                {
                    graphPath.AddLine(x, y, points[0].X - minX, points[0].Y - minY);
                }
                else
                {
                    graphPath.AddLine(x, y, points[i + 1].X - minX, points[i + 1].Y - minY);
                }
            }
            if (isfill)
            {
                g.FillPath(brush, graphPath);
            }
            g.DrawPath(pen, graphPath);
            graphPath.Dispose();
            return bitmap;
        }

        private Bitmap Readimg(Dictionary<Layer, Bitmap> compileLayer, Bitmap cutoutimage)
        {
            cutoutimage.Save(@"C:\Users\awwar\AppData\Local\Temp\CropPod\reports\cuto.png");
            Bitmap newimg = new Bitmap(cutoutimage.Width, cutoutimage.Height);
            try
            {
                int x, y;
                Color cutcolor;
                Color laycolor;
                string colorhex;
                int laycount = compileLayer.Count();
                // Loop through the images pixels to reset color.
                for (x = 0; x < newimg.Width; x++)
                {
                    for (y = 0; y < newimg.Height; y++)
                    {
                        cutcolor = cutoutimage.GetPixel(x, y);
                        if (cutcolor.A != 0)
                        {
                            float newlaypower = 0;
                            foreach (KeyValuePair<Layer, Bitmap> item in compileLayer)
                            {
                                Layer lay = item.Key;
                                Bitmap img = item.Value;
                                laycolor = img.GetPixel(x, y);
                                if (laycolor.A > 0)
                                {
                                    float gate = lay.setMax - lay.setMin;
                                    colorhex = string.Format("0x{0:X2}{1:X2}{2:X2}", laycolor.R, laycolor.G, laycolor.B);
                                    float colorpower = Convert.ToInt32(colorhex, 16);
                                    float layerpower = lay.ColorMax - lay.ColorMin;
                                    if (colorpower >= lay.setMin && colorpower <= lay.setMax && lay.invert == false)
                                    {
                                        newlaypower += (colorpower - lay.ColorMin) / layerpower;
                                    }
                                    else
                                    {
                                        newlaypower = 0;
                                        break;
                                    }
                                }
                                else
                                {
                                    newlaypower += 1;
                                }
                            }
                            if (newlaypower != 0)
                            {
                                newimg.SetPixel(x, y, Color.FromArgb((int)(255 * (newlaypower / laycount)), 0, 0));
                            }
                            else
                            {
                                newimg.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            }

                        }
                        else
                        {
                            newimg.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));

                        }
                    }
                }


            }
            catch (ArgumentException)
            {

            }
            return newimg;
        }

        public Report[] Handle()
        {
            return Reports.ToArray();
        }
    }
}
