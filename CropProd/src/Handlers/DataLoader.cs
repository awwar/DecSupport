using LatLonToTile;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private TileCoordinate tileCoordinate;
        private string basepath = "";

        public DataLoader()
        {
            this.basepath = Path.GetTempPath() + "CropPod/projects/";
            tileCoordinate = new TileCoordinate(256);
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
            SerilizeProject(project);
            File.Delete(project.Path);
            ZipFile.CreateFromDirectory(basepath + project.Name, project.Path);
        }

        public Project LoadProject(string path)
        {
            string newPath = path;
            if (Path.GetExtension(newPath) == ".cpproj")
            {
                newPath = LoadZippenProject(path, newPath);
            }
            string prodfilepath = newPath + "/project.bin";
            Project project = null;
            if (File.Exists(prodfilepath))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(prodfilepath, FileMode.Open, FileAccess.Read);
                project = (Project) formatter.Deserialize(stream);
                project.Path = path;
                stream.Close();
            }
            return project;
        }

        private string LoadZippenProject(string path, string prodname)
        {
            string trueNamePath = basepath + Path.GetFileNameWithoutExtension(prodname);
            if (!Directory.Exists(trueNamePath))
            {
                ZipFile.ExtractToDirectory(path, trueNamePath);
            }
            return trueNamePath;
        }

        private void SerilizeProject(Project project)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(basepath + project.Name + "/project.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, project);
            stream.Close();
        }

        public Dictionary<string , Image> ReadData(string path)
        {
            string prodname = Path.GetFileName(path);
            string prodpath = basepath + prodname;

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
            Dictionary<string, Image> data = new Dictionary<string, Image>();
            foreach (string imageFileName in Directory.GetFiles(prodpath, "*.png"))
            {
                using (FileStream myStream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read))
                {
                    Image img = Image.FromStream(myStream);
                    data.Add(imageFileName, img);
                }
            }
            return data;
        }

        /*private Dictionary<string, Image>[] ReadLayers(string path)
        {
            string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (string imageFileName in Directory.GetFiles(dirs, "*.png"))
            {
                using (FileStream myStream = new FileStream(imageFileName, FileMode.Open, FileAccess.Read))
                {
                    Image img = Image.FromStream(myStream);
                    data.Add(imageFileName, img);
                }
            }
            return data;
        }*/
    }
}
