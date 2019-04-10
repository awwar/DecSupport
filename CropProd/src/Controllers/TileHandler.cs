﻿using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Numerics;
using static Controllers.ImageLoader;

namespace Controllers
{
    class TileHandler : IDrawable
    {
        public string name = "google";
        public readonly int tileSize = 256;
        public string basePath = "";
        
        private Random rnd;
        public ImageLoader Loader;
        private Scene scene;
        private double originShift;

        private List<Tile> tiles = new List<Tile>();


        private readonly Dictionary<string, string> distrib = new Dictionary<string, string>
        {
            {"openstrmap"   ,"https://a.tile.openstreetmap.org/{2}/{0}/{1}.png "},
            {"googleS"      ,"http://mt2.google.com/vt/lyrs=s&x={0}&y={1}&z={2}" },
            {"PaintMap"     ,"http://c.tile.stamen.com/watercolor/{2}/{0}/{1}.jpg" },
            {"google"       ,"https://khms1.googleapis.com/kh?v=821&x={0}&y={1}&z={2}"},
            {"twogis"       ,"https://tile1.maps.2gis.com/tiles?x={0}&y={1}&z={2}&v=1.5&r=g&ts=online_sd" },
            {"yandex"       ,"https://sat01.maps.yandex.net/tiles?l=sat&v=3.449.0&x={0}&y={1}&z={2}&lang=ru_RU"}
        };

        public TileHandler(Scene scene)
        {
            this.scene = scene;
            rnd = new Random();
            Loader = new ImageLoader();
            basePath = Path.GetTempPath() + "CropPod";
            originShift = 2 * Math.PI * 6378137 / 2.0;
        }

        public IFrame[] draw()
        {
            Tile[] tileA = tiles.ToArray();
            foreach (Tile tile in tileA)
            {
                tile.draw();
                UpdateTile(tile);
            }
            Console.WriteLine(tileA.Length);
            return tileA;
        }

        public void GetTileAt(Vector2 leftop)
        {
            Image img;
            double tx = leftop.X + scene.coordinate.X;
            double ty = leftop.Y + scene.coordinate.Y;

            string url = string.Format(distrib[name], tx, ty, scene.zoom, rnd.Next(1, 3));
            string filename = tx.ToString() + "_" + ty.ToString() + ".jpeg";
            string query = string.Format(@"{0}\{1}\{2}\", basePath, name, scene.zoom);
            string fullpath = query + filename;

            if (!File.Exists(fullpath))
            {
                Tile tile = new Tile(
                    leftop,
                    url,
                    fullpath,
                    ref scene
                );
                Loader.AddFrame(tile);
                addTile(tile);
                Loader.onImageLoad += new ImageLoadHandler(tile.ImageLoaded);
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
                addTile(new Tile(
                    leftop,
                    img,
                    ref scene
                ));
            }
        }

        public void GetScreenAt()
        {
            clearTilePool();
            int width   = (int) scene.size.X / (256 * 2) + 2;

            int height  = (int) scene.size.Y / (256 * 2) + 2;

            for (int y = -height; y <= height; y++)
            {
                for (int x = -width; x <= width; x++)
                {
                    GetTileAt(new Vector2(x, y));
                }
            }
        }

        public void Zoom()
        {
            Loader.ClearPool();
            GetScreenAt();
        }

        public void addTile(Tile tile)
        {
            tiles.Add(tile);
        }

        public void removeTile(Tile tile)
        {
            //tile.image.Dispose();
            /*if(tile.path != null)
            {
                Loader.DeleteFrame(tile);
                Loader.onImageLoad -= new ImageLoadHandler(tile.ImageLoaded);
            }*/
            tiles.Remove(tile);
        }

        public void clearTilePool()
        {
            foreach (Tile tile in tiles)
            {
                tile.image.Dispose();
                if (tile.path != null)
                {
                    Loader.DeleteFrame(tile);
                    Loader.onImageLoad -= new ImageLoadHandler(tile.ImageLoaded);
                }
            }
            tiles.Clear();
        }

        private void UpdateTile(Tile tile)
        {
            if (tile.screenposition.Y < -256 * 2 || tile.screenposition.Y > scene.size.Y + 256)
            {
                removeTile(tile);
                GetTileAt(
                    new Vector2(
                        tile.coordinate.X,
                        (tile.screenposition.Y < -256 * 2)
                            ? tile.coordinate.Y + (int) scene.size.Y / (256) + 3
                            : tile.coordinate.Y - (int) scene.size.Y / (256) - 3
                    )
                );
            }
            else if (tile.screenposition.X < -256 * 2 || tile.screenposition.X > scene.size.X + 256)
            {
                removeTile(tile);
                GetTileAt(
                    new Vector2(
                        (tile.screenposition.X < -256 * 2)
                            ? tile.coordinate.X + (int) scene.size.X / (256) + 3
                            : tile.coordinate.X - (int) scene.size.X / (256) - 3,
                        tile.coordinate.Y
                    )
                );
            }
        }

        public double[] LatLonToMeters(double lat, double lon, int zoom)
        {
            double mx = lon * originShift / 180.0;
            double my = Math.Log(Math.Tan((90 + lat) * Math.PI / 360.0)) / (Math.PI / 180.0);

            my = my * originShift / 180.0;

            double[] rez = MetersToPixels(mx, my, zoom);

            return rez;
        }

        private double[] MetersToPixels(double mx, double my, int zoom)
        {
            double res = Resolution(zoom);
            double px = (mx + originShift) / res;
            double py = (my + originShift) / res;
            return PixelsToTile(px, py, zoom);
        }

        private double[] PixelsToTile(double px, double py, int zoom)
        {
            double tx = Math.Ceiling(px / tileSize) - 1;
            double ty = Math.Ceiling(py / tileSize) - 1;
            return GoogleTile(tx, ty, zoom);
        }

        private double Resolution(int zoom)
        {
            return (2 * Math.PI * 6378137 / tileSize) / Math.Pow(2, zoom);
        }

        private double[] GoogleTile(double tx, double ty, int zoom)
        {
            return new double[2] { tx, (Math.Pow(2, zoom) - 1) - ty };
        }


    }
}
