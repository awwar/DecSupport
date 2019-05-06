using Interfaces;
using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace Handlers
{
    class DataHandler : IHandler
    {
        public Action Redraw { set; get; }
        private List<Data> DataLayer = new List<Data>();
        private readonly Scene Scene;
        private readonly TileCoordinate TileCoordinate;
        private readonly DataLoader Loader;

        private Project cureentProject;

        public DataHandler(ref Scene scene)
        {
            this.Scene = scene;
            TileCoordinate = new TileCoordinate(Settings.Settings.TileSize);
            Loader = new DataLoader();
        }

        public void AddData(Image img , Vector2 pos)
        {
            DataLayer.Add(new Data(pos, new Vector2(img.Width, img.Height), img));
        }

        public void AddData(Data[] data)
        {
            foreach (Data item in data)
            {
                DataLayer.Add(item);
            }
        }

        public void DeleteData(Data data)
        {
            DataLayer.Remove(data);
        }

        public Frame[] Handle()
        {
            foreach (var item in DataLayer)
            {
                item.Draw(Scene.Position);
            }
            return DataLayer.ToArray();
        }

        public void CreateProject(string name, string lat, string lon)
        {
            double[] tilelatlon = TileCoordinate.Convert(
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
            Scene.AppendProject(cureentProject);
        }

        public Project OpenProject(string path)
        {
            this.cureentProject = Loader.LoadProject(path);
            Scene.AppendProject(cureentProject);
            foreach (Layer layer in cureentProject.Layers)
            {
                try
                {
                    AddData(Loader.ReadLayerData(cureentProject.Hash, layer.Hash));
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return cureentProject;
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
                    throw new IOException("Path not found");
                }
                else
                {
                    Loader.SaveProject(cureentProject);
                    return true;
                }
            }
            throw new Exception("Project not opened");
        }

        public void CreateLayer(Dictionary<string, Bitmap> img, string Lat, string Lon, string Filename)
        {
            double[] tilelatlon = TileCoordinate.Convert(
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
                Layer layer = Loader.AddLayer(path, cureentProject.Hash);
                cureentProject.AddLayer(layer);
            }
        }

    }
}
