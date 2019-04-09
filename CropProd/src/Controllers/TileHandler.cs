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
        public static int CurrentZ = 18;
        public static string name = "google";
        public static readonly int tileSize = 256;
        public static double CurrentLat = 55.763582;
        public static double CurrentLon = 37.663053;
        public static string basePath = "";

        private static Image img;
        private static Random rnd;
        private static double originShift;
        private static readonly Dictionary<string, string> distrib = new Dictionary<string, string>
        {
            {"openstrmap"   ,"https://a.tile.openstreetmap.org/{2}/{0}/{1}.png "},
            {"googleS"      ,"http://mt2.google.com/vt/lyrs=s&x={0}&y={1}&z={2}" },
            {"PaintMap"     ,"http://c.tile.stamen.com/watercolor/{2}/{0}/{1}.jpg" },
            {"google"       ,"https://khms1.googleapis.com/kh?v=821&x={0}&y={1}&z={2}"},
            {"twogis"       ,"https://tile1.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.5&r=g&ts=online_sd" },
            {"yandex"       ,"https://sat01.maps.yandex.net/tiles?l=sat&v=3.449.0&x={0}&y={1}&z={2}&lang=ru_RU"}
        };

        public static void Initialization()
        {
            rnd = new Random();
            basePath = Path.GetTempPath() + "CropPod";
            originShift = 2 * Math.PI * 6378137 / 2.0;
        }

        public static void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            CurrentLat = e.Position.Location.Latitude;
            CurrentLon = e.Position.Location.Longitude;
            recalculateSceneTilePosition();
        }

        public static void GetTileAt(Vector2 leftop)
        {
            double tx = leftop.X + SceneHandler.scene.coordinate.X;
            double ty = leftop.Y + SceneHandler.scene.coordinate.Y;

            string url = string.Format(distrib[name], tx, ty, CurrentZ, rnd.Next(1, 3));
            string filename = tx.ToString() + "_" + ty.ToString() + ".jpeg";
            string query = string.Format(@"{0}\{1}\{2}\", basePath, name, CurrentZ);
            string fullpath = query + filename;

            if (!File.Exists(fullpath))
            {
                SceneHandler.LoadFrame(new Tile(
                    leftop,
                    url,
                    fullpath,
                    ref SceneHandler.scene
                ));
            }
            else
            {
                try
                {
                    img = Image.FromFile(fullpath);
                }
                catch (Exception)
                {
                    img = null;
                }
                SceneHandler.AddFrame(leftop, img);
            }
        }

        public static void GetScreenAt()
        {
            recalculateSceneTilePosition();

            int width = (int) SceneHandler.scene.size.X / (256 * 2) + 2;

            int height = (int) SceneHandler.scene.size.Y / (256 * 2) + 2;

            for (int y = -height; y <= height; y++)
            {
                for (int x = -width; x <= width; x++)
                {
                    GetTileAt(new Vector2(x, y));
                }
            }
        }

        private static void recalculateSceneTilePosition()
        {
            double[] rez = LatLonToMeters(CurrentLat, CurrentLon, CurrentZ);
            SceneHandler.scene.setTileCenter(
                new Vector2((float) rez[0], (float) rez[1])
            );
        }

        private static double[] LatLonToMeters(double lat, double lon, int zoom)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);

            my = my * originShift / 180.0;

            double[] rez = MetersToPixels(mx, my, zoom);

            return rez;
        }

        private static double[] MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return PixelsToTile(px, py, zoom);
        }

        private static double[] PixelsToTile(double px, double py, int zoom)
        {
            double tx = Math.Ceiling(px / tileSize) - 1;
            double ty = Math.Ceiling(py / tileSize) - 1;
            return GoogleTile(tx, ty, zoom);
        }

        private static double Resolution(int zoom)
        {
            return (2 * Math.PI * 6378137 / tileSize) / Math.Pow(2, zoom);
        }

        private static double[] GoogleTile(double tx, double ty, int zoom)
        {
            return new double[2] { tx, (Math.Pow(2, zoom) - 1) - ty };
        }
    }
}
