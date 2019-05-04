using Events;
using Interfaces;
using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;

namespace Handlers
{
    internal class TileHandler : IHandler
    {
        private string basePath = "";
        private string name = "google";
        private Action Redraw;

        private readonly Random rnd;
        private Scene scene;
        private TileLoader Loader;
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

        public TileHandler(ref Scene scene, Action redraw)
        {
            this.scene = scene;
            this.Redraw = redraw;
            rnd = new Random();
            TileCoordinateConverter = new TileCoordinate(256);
            basePath = Path.GetTempPath() + "CropPod/tiles";
            ImgLoader = new Thread(() => { Loader = new TileLoader(); Loader.OnImageLoad += ImageLoaded; })
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

        public void Update()
        {
            if(scene.Lat == 0)
            {
                return;
            }
            RecalculateSceneTilePosition();
            Loader.ClearPool();
            ClearTilePool();
            GC.Collect();
            GetScreenAt();
        }

        private void GetScreenAt()
        {
            int width = (int)scene.Size.X / (256 * 2) + 2;

            int height = (int)scene.Size.Y / (256 * 2) + 2;

            for (int y = -height; y <= height; y++)
            {
                for (int x = -width; x <= width; x++)
                {
                    GetTileAt(new Vector2(x, y));
                }
            }
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

        private void GetTileAt(Vector2 leftop)
        {
            double tx = leftop.X + scene.Coordinate.X;
            double ty = leftop.Y + scene.Coordinate.Y;

            string url = string.Format(distrib[name], tx, ty, scene.Zoom, rnd.Next(1, 3));
            string filename = tx.ToString() + "_" + ty.ToString() + ".jpeg";
            string query = string.Format(@"{0}\{1}\{2}\", basePath, name, scene.Zoom);
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
            }
            else
            {
                Image img;
                FileStream myStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                try
                {
                    img = Image.FromStream(myStream);
                }
                catch (Exception)
                {
                    img = null;
                }
                finally
                {
                    myStream.Close();
                    myStream.Dispose();
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
            if (!tiles.Exists(x => Vector2.Equals(x.Position, tile.Position)))
            {
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
            if (tile.Screenposition.Y < -256 * 2 || tile.Screenposition.Y > scene.Size.Y + 256)
            {
                GetTileAt(
                    new Vector2(
                        tile.coordinate.X,
                        (tile.Screenposition.Y < -256 * 2)
                            ? tile.coordinate.Y + (int)scene.Size.Y / (256) + 3
                            : tile.coordinate.Y - (int)scene.Size.Y / (256) - 3
                    )
                );
                RemoveTile(tile);
            }
            else if (tile.Screenposition.X < -256 * 2 || tile.Screenposition.X > scene.Size.X + 256)
            {
                GetTileAt(
                    new Vector2(
                        (tile.Screenposition.X < -256 * 2)
                            ? tile.coordinate.X + (int)scene.Size.X / (256) + 3
                            : tile.coordinate.X - (int)scene.Size.X / (256) - 3,
                        tile.coordinate.Y
                    )
                );
                RemoveTile(tile);
            }
        }

        private void RecalculateSceneTilePosition()
        {
            double[] rez = TileCoordinateConverter.Convert(scene.Lat, scene.Lon, scene.Zoom);
            scene.SetTileCenter(
                new Vector2((float)rez[0], (float)rez[1])
            );
        }
    }
}
