using Interfaces;
using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using static Controllers.ImageLoader;

namespace Controllers
{
    internal class TileHandler : IHandler
    {
        private string basePath = "";
        private string name = "google";

        private readonly Random rnd;
        private Scene scene;
        private ImageLoader Loader;
        private TileCoordinate TileCoordinateConverter;
        private readonly Thread ImgLoader;
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

        public TileHandler(ref Scene scene)
        {
            this.scene = scene;
            rnd = new Random();
            TileCoordinateConverter = new TileCoordinate(256);
            basePath = Path.GetTempPath() + "CropPod";
            ImgLoader = new Thread(() => { Loader = new ImageLoader(); })
            {
                IsBackground = false
            };
            ImgLoader.Start();
        }

        public Frame[] Handle()
        {
            Tile[] tileA = tiles.ToArray();
            foreach (Tile tile in tileA)
            {
                tile.Draw();
                UpdateTile(tile);
            }
            return tileA;
        }

        public void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            scene.Lat = e.Position.Location.Latitude;
            scene.Lon = e.Position.Location.Longitude;
            //RecalculateSceneTilePosition();
        }

        public void Update()
        {
            RecalculateSceneTilePosition();
            Loader.ClearPool();
            ClearTilePool();
            GC.Collect();
            GetScreenAt();
        }

        private void GetScreenAt()
        {
            int width = (int)scene.size.X / (256 * 2) + 2;

            int height = (int)scene.size.Y / (256 * 2) + 2;

            for (int y = -height; y <= height; y++)
            {
                for (int x = -width; x <= width; x++)
                {
                    GetTileAt(new Vector2(x, y));
                }
            }
        }

        private void GetTileAt(Vector2 leftop)
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
                AddTile(tile);
                Loader.OnImageLoad += new ImageLoadHandler(tile.ImageLoaded);
            }
            else
            {

                try
                {
                    using (FileStream myStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read))
                    {
                        img = Image.FromStream(myStream);
                    }
                }
                catch (Exception)
                {
                    img = null;
                }
                AddTile(new Tile(
                    leftop,
                    img,
                    ref scene
                ));
            }
        }

        private void AddTile(Tile tile)
        {
            if(!tiles.Exists(x => Vector2.Equals(x.Position , tile.Position))) {
                tiles.Add(tile);
            }
        }

        private void RemoveTile(Tile tile)
        {
            DisposeImg(tile);
            tiles.Remove(tile);
        }

        private void ClearTilePool()
        {
            foreach (Tile tile in tiles)
            {
                DisposeImg(tile);
            }
            tiles.Clear();
        }

        private void DisposeImg(Tile tile)
        {
            if (tile.path != null)
            {
                Loader.DeleteFrame(tile.path);
            }
            else
            {
                tile.Image.Dispose();
            }
        }

        private void UpdateTile(Tile tile)
        {
            if (tile.Screenposition.Y < -256 * 2 || tile.Screenposition.Y > scene.size.Y + 256)
            {
                GetTileAt(
                    new Vector2(
                        tile.coordinate.X,
                        (tile.Screenposition.Y < -256 * 2)
                            ? tile.coordinate.Y + (int)scene.size.Y / (256) + 3
                            : tile.coordinate.Y - (int)scene.size.Y / (256) - 3
                    )
                );
                RemoveTile(tile);
            }
            else if (tile.Screenposition.X < -256 * 2 || tile.Screenposition.X > scene.size.X + 256)
            {
                GetTileAt(
                    new Vector2(
                        (tile.Screenposition.X < -256 * 2)
                            ? tile.coordinate.X + (int)scene.size.X / (256) + 3
                            : tile.coordinate.X - (int)scene.size.X / (256) - 3,
                        tile.coordinate.Y
                    )
                );
                RemoveTile(tile);
            }
        }

        private void RecalculateSceneTilePosition()
        {
            double[] rez = TileCoordinateConverter.Convert(scene.Lat, scene.Lon, scene.zoom);
            scene.SetTileCenter(
                new Vector2((float)rez[0], (float)rez[1])
            );
        }
    }
}
