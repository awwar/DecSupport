using Interfaces;
using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace Handlers
{
    class LayerLoader : IHandler
    {
        public Action Redraw { set; get; }
        private readonly Scene Scene;
        private readonly TileCoordinate TileCoordinate;
        private readonly DataLoader Loader;

        private Project cureentProject;

        public LayerLoader(ref Scene scene)
        {
            Scene = scene;
            TileCoordinate = new TileCoordinate(Settings.Settings.TileSize);
            Loader = new DataLoader();
        }

        public void AddLayer(Layer layer)
        {
            layer.Datas = Loader.ReadLayerData(cureentProject.Hash, layer.Hash);
            cureentProject.AddLayer(layer);
        }

        public Frame[] Handle()
        {
            List<Data> retdata = new List<Data>() { };
            if (cureentProject != null)
            {
                foreach (Layer item in cureentProject.Layers)
                {
                    if(item.Datas != null)
                    {
                        foreach (Data data in item.Datas)
                        {
                            data.Draw(Scene.Position);
                            retdata.Add(data);
                        }
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
            cureentProject = Loader.LoadProject(path);
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
            if (cureentProject != null)
            {
                if (path != null)
                {
                    cureentProject.Path = path;
                }
                if (cureentProject.Path == null)
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

        public void CreateLayer(Dictionary<string, Bitmap> img, string Lat, string Lon, string Min, string Max, string Filename)
        {

            int colormin = 255 * 255 * 255;
            int colormax = 0;

            double[] tilelatlon = TileCoordinate.Convert(
                double.Parse(Lat, CultureInfo.InvariantCulture),
                double.Parse(Lon, CultureInfo.InvariantCulture),
                18
            );

            //высчитываем минимальную и максимальную границу цвета для данного слоя
            Color pix;

            foreach (Bitmap item in img.Values)
            {
                for (int i = 0; i < item.Height; i++)
                {
                    for (int j = 0; i < item.Width; i++)
                    {
                        pix = item.GetPixel(i, j);
                        int r = (pix.R == 0) ? 1 : pix.R;
                        int g = (pix.G == 0) ? 1 : pix.G;
                        int b = (pix.B == 0) ? 1 : pix.B;
                        int pixpower = r * g * b;
                        if(pix.A == 0)
                        {
                            continue;
                        }
                        if (pixpower > colormax)
                        {
                            colormax = pixpower;
                        }
                        if (pixpower < colormin)
                        {
                            colormin = pixpower;
                        }
                    }
                }
            }

            Layer layer = new Layer()
            {
                Lat = tilelatlon[0],
                Lon = tilelatlon[1],
                Min = float.Parse(Min),
                Max = float.Parse(Max),
                ColorMin = colormin,
                ColorMax = colormax,
                Name = Path.GetFileNameWithoutExtension(Filename)
            };

            Loader.CreateLayer(img, layer, Filename);
        }

        public Layer[] LoadLayer(string path)
        {
            if (cureentProject != null)
            {
                Layer layer = Loader.AddLayer(path, cureentProject.Hash);

                AddLayer(layer);

                SaveProject();
            }
            return cureentProject.Layers;
        }


        public Layer[] DeleteLayer(Layer layer)
        {
            if (cureentProject != null)
            {
                cureentProject.DeleteLayer(layer);
                Loader.DeleteLayer(layer.Hash, cureentProject.Hash);
                SaveProject();
            }
            return cureentProject.Layers;
        }

    }
}
