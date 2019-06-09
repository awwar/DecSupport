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
            layer.DataLayers = Loader.ReadLayerData(cureentProject.Hash, layer.Hash);
            cureentProject.AddLayer(layer);
        }

        public Layer[] Handle()
        {
            List<Layer> retdata = new List<Layer>() { };
            if (cureentProject != null)
            {
                foreach (Layer item in cureentProject.Layers)
                {
                    if (item.DataLayers != null)
                    {
                        item.Draw(Scene.Position);
                        retdata.Add(item);
                    }

                }
            }
            return retdata.ToArray();
        }

        public Project CreateProject(CreateProjDialogData createProj)
        {
            int[] tilelatlon = TileCoordinate.ConvertWorldToTile(
                double.Parse(createProj.Lat, CultureInfo.InvariantCulture),
                double.Parse(createProj.Lon, CultureInfo.InvariantCulture),
                18
            );
            cureentProject = new Project()
            {
                Name = createProj.Name,
                TileX = tilelatlon[0],
                TileY = tilelatlon[1]
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

        public void CreateLayer(LayerMakerDialogData data)
        {

            int colormin = 256 * 256 * 256;
            int colormax = 0;

            int[] tilelatlon = TileCoordinate.ConvertWorldToTile(
                double.Parse(data.Lat, CultureInfo.InvariantCulture),
                double.Parse(data.Lon, CultureInfo.InvariantCulture),
                18
            );

            //высчитываем минимальную и максимальную границу цвета для данного слоя
            Color pix;
            string colorhex;

            foreach (Bitmap item in data.Tiles.Values)
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
                TileX = tilelatlon[0],
                TileY = tilelatlon[1],
                Min = float.Parse(data.Min),
                Max = float.Parse(data.Max),
                ColorMin = colormin,
                ColorMax = colormax,
                Name = Path.GetFileNameWithoutExtension(data.FilePath),
                ValueType = data.ValueType
            };

            Loader.CreateLayer(data.Tiles, layer, data.FilePath);
        }

        public Layer[] OpenLayer(string path)
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
