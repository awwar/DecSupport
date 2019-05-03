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
        private string basepath = "";
        private Scene scene;
        private Action Redraw;
        private TileCoordinate tileCoordinate;
        private DataLoader Loader;

        private Project cureentProject;

        public DataHandler(ref Scene scene, Action redraw)
        {
            this.scene = scene;
            this.Redraw = redraw;
            this.basepath = Path.GetTempPath() + "CropPod/projects";
            tileCoordinate = new TileCoordinate(256);
            Loader = new DataLoader();
        }

        public void AddData(Image img , Vector2 pos)
        {
            datas.Add(new Data(pos, new Vector2(img.Width, img.Height), img, ref scene));
        }


        public void DeleteData(Data data)
        {
            datas.Remove(data);
        }

        public Frame[] Handle()
        {

            foreach (var item in datas)
            {
                item.Draw();
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
            Project proj = new Project()
            {
                Name = name,
                Lat = tilelatlon[0],
                Lon = tilelatlon[1]
            };
            Loader.CreateProject(proj);
        }

        public void OpenProject(string path)
        {
            this.cureentProject = Loader.LoadProject(path);
            Console.WriteLine(cureentProject.Lat);
        }

        public void SaveProject()
        {
            if(cureentProject != null)
            {
                cureentProject.Lat = 1.000;
                Loader.SaveProject(cureentProject);
            }
        }

        public void ReadData(string path)
        {
            string prodname = Path.GetFileName(path);
            string prodpath = basepath + "/" + prodname;

            if (!Directory.Exists(prodpath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(prodpath));
                try
                {
                    ZipFile.ExtractToDirectory(path, prodpath);
                }
                catch (Exception)
                {
                    Console.WriteLine("this is not prod file!");
                }
            }
            char[] charSeparators = new char[] { '_' };

            string[] latlon = prodname.Split(charSeparators);

            double[] tilelatlon = tileCoordinate.Convert(
                double.Parse(latlon[0], CultureInfo.InvariantCulture),
                double.Parse(latlon[1], CultureInfo.InvariantCulture),
                18
            );
            scene.Coordinate = new Vector2((float)tilelatlon[0], (float)tilelatlon[1]);

            foreach (string imageFileName in Directory.GetFiles(prodpath, "*.png"))
            {
                using (FileStream myStream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read))
                {
                    Image img = Image.FromStream(myStream);
                    AddImageFromFile(img, imageFileName);
                }
            }
            Redraw();
        }

        private void AddImageFromFile(Image img , string filepath)
        {
            string name = Path.GetFileNameWithoutExtension(filepath);

            char[] charSeparators = new char[] { '_' };

            string[] xy = name.Split(charSeparators);

            Vector2 coords = new Vector2(
                Int32.Parse(xy[0]),
                Int32.Parse(xy[1])
                );

            AddData(img, coords);
        }

    }
}
