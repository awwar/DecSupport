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
    class LayerLoader : IHandler<Layer>
    {
        public Action Redraw { set; get; }
        private readonly Scene Scene;
        private readonly TileCoordinate TileCoordinate;
        private readonly DataLoader Loader;

        private Project cureentProject;

        public LayerLoader(ref Scene scene)
        {
            Scene = scene;
            TileCoordinate = new TileCoordinate();
            Loader = new DataLoader();
        }

        public void AddLayer(Layer layer)
        {
            layer.Datas = Loader.ReadLayerData(cureentProject.Hash, layer.Hash);
            cureentProject.AddLayer(layer);
        }

        public Layer[] Handle()
        {
            List<Layer> retdata = new List<Layer>() { };
            if (cureentProject != null)
            {
                foreach (Layer item in cureentProject.Layers)
                {
                    if (item.Datas != null)
                    {
                        item.Draw(Scene.Position);
                        retdata.Add(item);
                    }

                }
            }
            return retdata.ToArray();
        }

        public Project CreateProject(string name, string lat, string lon)
        {
            int[] tilelatlon = TileCoordinate.ConvertWorldToTile(
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

            int colormin = 256 * 256 * 256;
            int colormax = 0;

            int[] tilelatlon = TileCoordinate.ConvertWorldToTile(
                double.Parse(Lat, CultureInfo.InvariantCulture),
                double.Parse(Lon, CultureInfo.InvariantCulture),
                18
            );

            //высчитываем минимальную и максимальную границу цвета для данного слоя
            Color pix;
            string colorhex;

            foreach (Bitmap item in img.Values)
            {
                for (int i = 0; i < item.Height; i++)
                {
                    for (int j = 0; i < item.Width; i++)
                    {
                        pix = item.GetPixel(i, j);

                        colorhex = string.Format("0x{0:X2}{1:X2}{2:X2}", pix.R, pix.G, pix.B);
                        int pixpower = Convert.ToInt32(colorhex, 16);

                        if (pixpower > colormax)
                        {
                            colormax = pixpower;
                        }
                        if (pixpower < colormin)
                        {
                            colormin = pixpower;
                        }

                        if (pix.A == 0)
                        {
                            continue;
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
