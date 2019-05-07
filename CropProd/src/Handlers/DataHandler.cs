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
        private List<Layer> DataLayer = new List<Layer>();
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

        public void AddLayer(Layer layer)
        {
            layer.Datas = Loader.ReadLayerData(cureentProject.Hash, layer.Hash);
            cureentProject.AddLayer(layer);
        }

        public void DeleteLayer(Layer layer)
        {
            cureentProject.DeleteLayer(layer);
        }

        public Frame[] Handle()
        {
            List <Data> retdata = new List<Data>() { };
            if(cureentProject != null) {
                foreach (Layer item in cureentProject.Layers)
                {
                    foreach (Data data in item.Datas)
                    {
                        data.Draw(Scene.Position);
                        retdata.Add(data);
                    }
                }
            }
            return retdata.ToArray();
        }

        public Project CreateProject(string name, string lat, string lon)
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
            return cureentProject;
        }

        public Project OpenProject(string path)
        {
            this.cureentProject = Loader.LoadProject(path);
            Scene.AppendProject(cureentProject);
            foreach (Layer layer in cureentProject.Layers)
            {
                try
                {
                    AddLayer(layer);
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

        public Layer[] AddLayer(string path)
        {
            if(cureentProject != null)
            {
                Layer layer = Loader.AddLayer(path, cureentProject.Hash);
                cureentProject.AddLayer(layer);

                AddLayer(layer);
            }
            return cureentProject.Layers;
        }


        public Layer[] DeleteLayer(Layer layer)
        {
            if (cureentProject != null)
            {
                cureentProject.DeleteLayer(layer);
                Loader.DeleteLayer(layer.Hash, cureentProject.Hash);
                DeleteLayer(layer);
            }
            return cureentProject.Layers;
        }

    }
}
