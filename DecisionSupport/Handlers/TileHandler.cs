using Events;
using DSCore;
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

        private int tilesize = Settings.TileSize;

        private Scene scene;
        private TileLoader Loader;
        private readonly Thread ImgLoader;
        private List<Tile> tiles = new List<Tile>();

        public TileHandler(ref Scene scene)
        {
            this.scene = scene;

            ImgLoader = new Thread(() =>
            {
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
            double X = leftop.X + scene.Coordinate.X;
            double Y = leftop.Y + scene.Coordinate.Y;
            Tile tile = Loader.ReserveTile(X, Y, scene.Zoom, leftop);
            AddTile(tile);
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
