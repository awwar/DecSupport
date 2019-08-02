using DSCore;
using Interfaces;
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

        public Report[] Handle()
        {
            return Reports.ToArray();
        }

        public void PrepareReport(Layer[] layers, PointF[] points, Frame[] tiles)
        {
            // Границы установленные пользователем - нативные, числовые а нам нужно перебить их в цветовой эквивалент
            for (int i = 0; i < layers.Length; i++)
            {
                float layerpower = (layers[i].ColorMax - layers[i].ColorMin) / (layers[i].Max - layers[i].Min);
                layers[i].setMax = layers[i].ColorMin + (layers[i].setMax - layers[i].Min) * layerpower;
                layers[i].setMin = layers[i].ColorMin + (layers[i].setMin - layers[i].Min) * layerpower;
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
                compileLayer[layer] = CompileFrames(layer.DataLayers, maxX, minX, maxY, minY);
            }
            Bitmap cutoutimage = CompileCutOut(points, maxX, minX, maxY, minY);
            Bitmap cutouframe = CompileCutOut(points, maxX, minX, maxY, minY, false);
            Bitmap tileimage = CompileFrames(tiles, maxX, minX, maxY, minY);
            Bitmap newimg = Readimg(compileLayer, cutoutimage);
            try
            {
                Reports.Add(Loader.SaveBitmaps(tileimage, newimg, cutouframe));
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
                if (isTileInRect(data.ScreenPosition.X, data.ScreenPosition.Y, maxX, minX, maxY, minY))
                {
                    g.DrawImage(data.Image, (float)Math.Floor(data.ScreenPosition.X - minX), (float)Math.Floor(data.ScreenPosition.Y - minY), 256, 256);
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
            Bitmap newimg = new Bitmap(cutoutimage.Width, cutoutimage.Height);
                int x, y;
                Color cutcolor;
                Color laycolor;
                string colorhex;
                int laycount = 0;
            Settings.RezPaitRulе = RezultRules.Min;
                // Loop through the images pixels to reset color.
                for (x = 0; x < newimg.Width; x++)
                {
                    for (y = 0; y < newimg.Height; y++)
                    {
                        cutcolor = cutoutimage.GetPixel(x, y);
                        if (cutcolor.A != 0)
                        {
                            float layerpower = 0;
                            foreach (KeyValuePair<Layer, Bitmap> item in compileLayer)
                            {
                                Layer lay = item.Key;
                                Bitmap img = item.Value;
                                laycolor = img.GetPixel(x, y);
                                if(laycolor.A == 0 && lay.nonAlpha)
                                {
                                    layerpower = 0;
                                    break;
                                }
                                if (laycolor.A > 0)
                                {
                                    colorhex = string.Format("0x{0:X2}{1:X2}{2:X2}", laycolor.R, laycolor.G, laycolor.B);
                                    int colorpower = Convert.ToInt32(colorhex, 16);
                                    bool value = (colorpower >= lay.setMin && colorpower <= lay.setMax);
                                    if ((lay.invert) ? !value : value)
                                    {
                                        layerpower += GetLayerPower(lay.setMax, lay.setMin, colorpower);                                        
                                        laycount += 1;
                                    }
                                    else
                                    {
                                        layerpower = 0;
                                        break;
                                    }
                                }
                                else
                                {
                                    layerpower += 1;
                                    laycount += 1;
                                }
                            }
                            if (layerpower > 0)
                            {
                                try
                                {
                                    if(layerpower > laycount)
                                    {
                                        layerpower = laycount;
                                    }
                                    newimg.SetPixel(x, y, Color.FromArgb((int)(255 * (layerpower / laycount)), 0, 0));
                                } catch(Exception e)
                                {
                                    Console.WriteLine( e);
                                }
                            }
                            else
                            {
                                newimg.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                            }
                            laycount = 0;

                        }
                        else
                        {
                            newimg.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                        }
                    }
                }
            return newimg;
        }

        private float GetLayerPower(float max, float min, float current)
        {
            float power = 0f;
            float origin = 0f;
            switch (Settings.RezPaitRulе)
            {
                case RezultRules.Middle: // если отклонение от центра
                    origin = (max - min) + 1 / 2;
                    power = 1 - (Math.Abs((current - min) - origin) / origin*2);
                    break;
                case RezultRules.Min: // если отклонение от минимального
                    origin = max - min + 1;
                    power = 1 - ((current - min) / origin);
                    break;
                case RezultRules.Max: // если отклонение от максимального
                    origin = max - min + 1;
                    power = (current - min) / origin;
                    break;
            }
            return power;
        }

    }
}
