using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using static Controllers.ImageLoader;
using LatLonToTile;
using System.Device.Location;

namespace Controllers
{
    class TileHandler : IHandler
    {
        public string basePath = "";
        public string name = "google";
        
        private Random rnd;
        private Scene scene;
        public  ImageLoader Loader;
        public TileCoordinate TileCoordinateConverter;
        private readonly Thread ImgLoader;
        public List<Tile> tiles = new List<Tile>();


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
            TileCoordinateConverter = new TileCoordinate(18, 256);
            basePath = Path.GetTempPath() + "CropPod";
            ImgLoader = new Thread(() => { Loader = new ImageLoader(); })
            {
                IsBackground = false
            };
            ImgLoader.Start();
        }

        public IFrame[] handle()
        {
            Tile[] tileA = tiles.ToArray();
            foreach (Tile tile in tileA)
            {
                tile.draw();
                UpdateTile(tile);
            }
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
                    using (FileStream myStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read))
                    {
                        img = Image.FromStream(myStream);
                    }
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
            recalculateSceneTilePosition();
            Loader.ClearPool();
            clearTilePool();
            GC.Collect();
            GetScreenAt();
        }

        public void addTile(Tile tile)
        {
           tiles.Add(tile);
        }

        public void removeTile(Tile tile)
        {
            if (tile.path != null)
            {
                Loader.DeleteFrame(tile.path);
            }
            /* !! Если пытаться сделать Dispose здесь то память перестанет течь, но будут возникать тормоза! */
            //tile.image.Dispose();
            tiles.Remove(tile);
        }

        public void clearTilePool()
        {
            foreach (Tile tile in tiles)
            {
                if (tile.path != null)
                {
                    Loader.DeleteFrame(tile.path);
                }
                else
                {
                    tile.image.Dispose();
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

        public void GeoWatcherOnStatusChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            scene.Lat = e.Position.Location.Latitude;
            scene.Lon = e.Position.Location.Longitude;
            recalculateSceneTilePosition();
        }

        private void recalculateSceneTilePosition()
        {
            double[] rez = TileCoordinateConverter.Convert(scene.Lat, scene.Lon, scene.zoom);
            scene.setTileCenter(
                new Vector2((float) rez[0], (float) rez[1])
            );
        }
    }
}
