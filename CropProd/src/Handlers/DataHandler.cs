using Interfaces;
using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;

namespace Handlers
{
    class DataHandler : IHandler
    {
        private List<Data> datas = new List<Data>();
        private Scene scene;
        private Action Redraw;
        private TileCoordinate tileCoordinate;
        private DataLoader Loader;

        private Project cureentProject;

        public DataHandler(ref Scene scene, Action redraw)
        {
            this.scene = scene;
            this.Redraw = redraw;
            tileCoordinate = new TileCoordinate(Settings.Settings.TileSize);
            Loader = new DataLoader();
        }

        public void AddData(Image img , Vector2 pos)
        {
            datas.Add(new Data(pos, new Vector2(img.Width, img.Height), img));
        }


        public void DeleteData(Data data)
        {
            datas.Remove(data);
        }

        public Frame[] Handle()
        {
            foreach (var item in datas)
            {
                item.Draw(scene.Position);
            }
            return datas.ToArray();
        }

        public void CreateProject(string name, string lat, string lon)
        {
            double[] tilelatlon = tileCoordinate.Convert(
                double.Parse(lat, CultureInfo.InvariantCulture),
                double.Parse(lon, CultureInfo.InvariantCulture),
                18
            );
            cureentProject = new Project()
            {
                Name = name,
                Lat = tilelatlon[0],
                Lon = tilelatlon[1]
            };
            Loader.CreateProject(cureentProject);
            scene.AppendProject(cureentProject);
        }

        public void OpenProject(string path)
        {
            this.cureentProject = Loader.LoadProject(path);
            scene.AppendProject(cureentProject);
        }

        public bool SaveProject(string path = null)
        {
            if(cureentProject != null)
            {
                if(path != null)
                {
                    cureentProject.Path = path;
                }
                if(cureentProject.Path == null)
                {
                    return false;
                }
                else
                {
                    Loader.SaveProject(cureentProject);
                }
            }
            return true;
        }

        public void CreateLayer(Dictionary<string, Bitmap> img, string Lat, string Lon, string Filename)
        {
            double[] tilelatlon = tileCoordinate.Convert(
                double.Parse(Lat, CultureInfo.InvariantCulture),
                double.Parse(Lon, CultureInfo.InvariantCulture),
                18
            );

            Layer layer = new Layer()
            {
                Lat = tilelatlon[0],
                Lon = tilelatlon[1],
                Name = Path.GetFileNameWithoutExtension(Filename)
            };

            Loader.CreateLayer(img, layer, Filename);
        }

        public void AddLayer(string path)
        {
            if(cureentProject != null)
            {
                Loader.AddLayer(path, cureentProject.Hash);
            }
        }

    }
}
