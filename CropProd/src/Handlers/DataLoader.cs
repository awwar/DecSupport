using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    class DataLoader
    {
        private string basepath = "";

        public DataLoader()
        {
            this.basepath = Path.GetTempPath() + "CropPod/projects/";
        }

        public void CreateProject(Project project)
        {
            string path = basepath + project.Name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);                
            }
        }

        public void SaveProject(Project project)
        {
            string path = basepath + project.Name + "/";
            Serilize(project, path);
            ZipFile.CreateFromDirectory(path, project.Path);
        }

        public Project LoadProject(string path)
        {
            string newPath = LoadZippenProject(path);
            Project project = DeSerilize<Project>(newPath);
            project.Path = path;
            return project;
        }

        public void CreateLayer(Dictionary<string, Bitmap> tiles, Layer layer, string Filename)
        {
            string layerpath = Path.GetTempPath() + "CropPod/layers/" + Path.GetFileNameWithoutExtension(Filename)+"/";
            Directory.CreateDirectory(layerpath);

            foreach (KeyValuePair<string , Bitmap> tile in tiles)
            {
                tile.Value.Save(layerpath + tile.Key + ".png", ImageFormat.Png);
            }
            Serilize(layer, layerpath);
            ZipFile.CreateFromDirectory(layerpath, Filename);
        }

        public void AddLayer(string path , string prodname)
        {
            string layername = Path.GetFileNameWithoutExtension(path);
            string prodpath = basepath + prodname + "/" + layername;


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
        }

        private string LoadZippenProject(string path)
        {
            string trueNamePath = basepath + Path.GetFileNameWithoutExtension(path);
            if (!Directory.Exists(trueNamePath))
            {
                ZipFile.ExtractToDirectory(path, trueNamePath);
            }
            return trueNamePath;
        }

        private void Serilize<T>(T project, string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path + "/data.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, project);
            stream.Close();
        }

        private T DeSerilize<T>(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path + "/data.bin", FileMode.Open, FileAccess.Read);
            T serializeble = (T)formatter.Deserialize(stream);
            stream.Close();
            return serializeble;
        }
    }
}
