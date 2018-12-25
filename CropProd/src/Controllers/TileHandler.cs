﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    static class TileHandler
    {
        static int tileSize = 256;
        static double originShift = 2 * Math.PI * 6378137 / 2.0;
        public static double CurrentLat = 55.763582;
        public static double CurrentLon = 37.663053;
        public static int CurrentZ = 18;


        static public void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            CurrentLat = e.Position.Location.Latitude;
            CurrentLon = e.Position.Location.Longitude;
        }

        static public void GetTileAt(double lat, double lon, int zoom)
        {
            CurrentZ = zoom;
            double[] rez = LatLonToMeters(lat, lon, zoom);
            string path = getimg(rez[0], rez[1], zoom);
            Image img = Image.FromFile(path);
            float camx = (int)SceneHandler.scene.camera.tileCenter.X;
            float camy = (int)SceneHandler.scene.camera.tileCenter.Y;
            SceneHandler.AddFrame(new Vector2(camx, camy), img, rez);
        }

        static public void GetScreenAt(double lat, double lon, int zoom)
        {
            CurrentZ = zoom;
            double[] rez = new double[2];
            rez = LatLonToMeters(lat, lon, zoom);

            int width = (int)((SceneHandler.form.Width / tileSize * 1f) / 2) + 2;
            int height = (int)((SceneHandler.form.Height / tileSize * 1f) / 2) + 1;

            float camx = SceneHandler.scene.camera.tileCenter.X;
            float camy = SceneHandler.scene.camera.tileCenter.Y;

            for (int y = -height; y < height; y++)
            {
                for (int x = -width; x < width; x++)
                {
                    string path = getimg(rez[0] + x, rez[1] + y, zoom);
                    try
                    {
                        Image img = Image.FromFile(path);
                        SceneHandler.AddFrame(new Vector2(camx + x * tileSize, camy + y * tileSize), img, new double[2] { rez[0] + x, rez[1] + y });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        static private double[] LatLonToMeters(double lat, double lon, int zoom)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);

            my = my * originShift / 180.0;

            double[] rez = MetersToPixels(mx, my, zoom);

            return rez;
        }

        static private double[] MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return PixelsToTile(px, py, zoom);
        }

        static private double[] PixelsToTile(double px, double py, int zoom)
        {
            double tx = Math.Ceiling(px / tileSize) - 1;
            double ty = Math.Ceiling(py / tileSize) - 1;
            return GoogleTile(tx, ty, zoom);
        }

        static private double Resolution(int zoom)
        {
            return (2 * Math.PI * 6378137 / tileSize) / Math.Pow(2, zoom);
        }

        static private double[] GoogleTile(double tx, double ty, int zoom)
        {
            return new double[2] { tx, (Math.Pow(2, zoom) - 1) - ty };
        }

        static private string getimg(double tx, double ty, int zoom)
        {
            string url = "";
            string twogis = "https://tile1.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.5&r=g&ts=online_sd";
            string google = "https://khms0.googleapis.com/kh?v=821&hl=ru&x={0}&y={1}&z={2}";
            string yandex = "https://sat04.maps.yandex.net/tiles?l=sat&v=3.449.0&x={0}&y={1}&z={2}&lang=ru_RU";
            string openstrmap = "https://a.tile.openstreetmap.org/{2}/{0}/{1}.png ";
            url = string.Format(google, tx, ty, zoom);
            string baseurl = Path.GetTempPath() + "CropPod\\";
            string filename = tx.ToString() + "_" + ty.ToString() + ".jpeg";
            string query = string.Format(@"{0}{1}\", baseurl, zoom);
            string fullpath = query + filename;

            if (!Directory.Exists(fullpath))
            {
                if (!File.Exists(fullpath))
                {
                    Directory.CreateDirectory(query);

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                        
                        try {
                            client.DownloadFile(new Uri(url), fullpath);
                            client.Dispose();
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(">>>>>>>>>>>" + e.Message);
                        }

                    }
                }
            }

            return fullpath;
        }
    }
}
