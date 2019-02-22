using Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace Controllers
{
    static class TileHandler
    {
        public static readonly int tileSize = 64;
        static readonly double originShift = 2 * Math.PI * 6378137 / 2.0;
        public static double CurrentLat = 55.763582;
        public static double CurrentLon = 37.663053;
        public static int CurrentZ = 18;
        public static string name = "twogis";
        static Image img;
        static int ct;
        public static LoaderHandler Loader;
        static Dictionary<string, string> distrib = new Dictionary<string, string>
        {
            {"twogis" ,"https://tile1.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.5&r=g&ts=online_sd" },
            {"google" ,"https://khms1.googleapis.com/kh?v=821&x={0}&y={1}&z={2}"},
            {"yandex" ,"https://sat01.maps.yandex.net/tiles?l=sat&v=3.449.0&x={0}&y={1}&z={2}&lang=ru_RU"},
            {"openstrmap" ,"https://a.tile.openstreetmap.org/{2}/{0}/{1}.png "},
            {"PaintMap", "http://c.tile.stamen.com/watercolor/{2}/{0}/{1}.jpg" },
            {"googleS", "http://mt2.google.com/vt/lyrs=s&x={0}&y={1}&z={2}" }

        };

        static public void Initialization()
        {
            Loader = new LoaderHandler();
            Loader.Start();
        }

        static public void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            CurrentLat = e.Position.Location.Latitude;
            CurrentLon = e.Position.Location.Longitude;
            double[] rez = LatLonToMeters(CurrentLat, CurrentLon, CurrentZ);
            SceneHandler.scene.coord = new Vector2((float)rez[0], (float)rez[1]);
        }

        static public void GetTileAt(Vector2 leftop, int zoom = 18)
        {
            double tx = SceneHandler.scene.coord.X + leftop.X;
            double ty = SceneHandler.scene.coord.Y + leftop.Y;
            string url = "";
            Random rnd = new Random();
            url = string.Format(distrib[name], tx, ty, zoom,rnd.Next(1,3));
            string baseurl = Path.GetTempPath() + "CropPod";
            string filename = tx.ToString() + "_" + ty.ToString() + ".jpeg";
            string query = string.Format(@"{0}\{1}\{2}\", baseurl, name, zoom);
            string fullpath = query + filename;
            if (!File.Exists(fullpath))
            {
                Tile frame = new Tile(fullpath, url, leftop);
                Loader.AddPath(frame);
            }
            else
            {
                try
                {
                    img = Image.FromFile(fullpath);
                    SceneHandler.AddTile(leftop, img);
                }
                catch (Exception)
                {
                    Console.WriteLine("11");
                }
            }
            Console.WriteLine("вызвана отрисовка тайлов {0}", ct);
            ct++;
        }

        static public void GetScreenAt(int zoom)
        {
            int width = (int)((SceneHandler.form.Width / tileSize * 1f) / 2) + 4;
            int height = (int)((SceneHandler.form.Height / tileSize * 1f) / 2) + 2;
            Loader.block = true;
            for (int y = -height; y < height; y++)
            {
                for (int x = -width; x < width; x++)
                {
                    GetTileAt(new Vector2(x, y), zoom);
                }
            }
            Loader.block = false;
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
    }
}
