using Events;
using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;

namespace Handlers
{
    class TileHandler : IHandler
    {
        public Action Redraw { set; get; }
        private string basePath = "";
        private string name = "google";
        private int tilesize = Settings.Settings.TileSize;

        private readonly Random rnd;
        private Scene scene;
        private TileLoader Loader;
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
            basePath = Path.GetTempPath() + "CropPod/tiles";
            ImgLoader = new Thread(() => {
                Loader = new TileLoader();
                Loader.OnImageLoad += ImageLoaded;
            })
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
                tile.Draw(scene.Position);
                UpdateTile(tile);
            }
            return tileA;
        }

        public void Update()
        {
            if (scene.Coordinate.Length() <= 0)
            {
                return;
            }
            Loader.ClearPool();
            ClearTilePool();
            GC.Collect();
            GetScreenAt();
        }

        public void ImageLoaded(object sender, ImageLoadArgs e)
        {
            Tile find = tiles.Find(x => x.Path.Contains(e.Path));
            if (find != null)
            {
                find.Image = e.Image;
            }
            Redraw();
        }

        private void GetScreenAt()
        {
            int width = (int)scene.Size.X / (tilesize * 2) + 2;

            int height = (int)scene.Size.Y / (tilesize * 2) + 2;

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
            double tx = leftop.X + scene.Coordinate.X;
            double ty = leftop.Y + scene.Coordinate.Y;

            string url = string.Format(distrib[name], tx, ty, scene.Zoom, rnd.Next(1, 3));
            string filename = $"{tx.ToString()}_{ty.ToString()}.jpeg";
            string query = string.Format(@"{0}\{1}\{2}\", basePath, name, scene.Zoom);
            string fullpath = query + filename;

            if (!File.Exists(fullpath))
            {
                Tile tile = new Tile(
                    leftop,
                    url,
                    fullpath
                );
                Loader.AddFrame(tile);
                AddTile(tile);
            }
            else
            {
                Image img;
                try
                {
                    FileStream myStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                    img = Image.FromStream(myStream);
                    myStream.Close();
                }
                catch (Exception)
                {
                    img = null;
                }
                AddTile(new Tile(
                    leftop,
                    img
                ));
            }
        }

        private void AddTile(Tile tile)
        {
            if (!tiles.Exists(x => Vector2.Equals(x.Position, tile.Position)))
            {
                tiles.Add(tile);
            }
        }

        private void RemoveTile(Tile tile)
        {
            //DisposeImg(tile);
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
            if (tile.Path != null)
            {
                Loader.DeleteFrame(tile.Path);
            }
            else
            {
                tile.Image.Dispose();
            }
        }

        private void UpdateTile(Tile tile)
        {
            if (tile.Screenposition.Y < -tilesize * 2 || tile.Screenposition.Y > scene.Size.Y + tilesize)
            {
                GetTileAt(
                    new Vector2(
                        tile.Coordinate.X,
                        (tile.Screenposition.Y < -tilesize * 2)
                            ? tile.Coordinate.Y + (int)scene.Size.Y / (tilesize) + 3
                            : tile.Coordinate.Y - (int)scene.Size.Y / (tilesize) - 3
                    )
                );
                RemoveTile(tile);
            }
            else if (tile.Screenposition.X < -tilesize * 2 || tile.Screenposition.X > scene.Size.X + tilesize)
            {
                GetTileAt(
                    new Vector2(
                        (tile.Screenposition.X < -tilesize * 2)
                            ? tile.Coordinate.X + (int)scene.Size.X / (tilesize) + 3
                            : tile.Coordinate.X - (int)scene.Size.X / (tilesize) - 3,
                        tile.Coordinate.Y
                    )
                );
                RemoveTile(tile);
            }
        }
    }
}
