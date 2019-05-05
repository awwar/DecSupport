using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
            project.Hash = GetHashName(project.Name);

            string tempPath = basepath + project.Hash;

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
        }

        public void SaveProject(Project project)
        {
            string tempPath = basepath + project.Hash;

            Serilize(project, tempPath);
            if (File.Exists(project.Path))
            {
                File.Delete(project.Path);
            }
            ZipFile.CreateFromDirectory(tempPath, project.Path);
        }

        public Project LoadProject(string path)
        {
            Project project = ReadFileData<Project>(path);
            string tempPath = basepath + project.Hash;

            if (!Directory.Exists(tempPath))
            {
                ZipFile.ExtractToDirectory(path, tempPath);
            }
            project.Path = path;
            return project;
        }

        public void CreateLayer(Dictionary<string, Bitmap> tiles, Layer layer, string Filename)
        {
            layer.Hash = GetHashName(layer.Name);
            string layerpath = Path.GetTempPath() + "CropPod/layers/" + layer.Hash + "/";

            Directory.CreateDirectory(layerpath);
            foreach (KeyValuePair<string, Bitmap> tile in tiles)
            {
                tile.Value.Save(layerpath + tile.Key + ".png", ImageFormat.Png);
            }
            Serilize(layer, layerpath);

            ZipFile.CreateFromDirectory(layerpath, Filename);
        }

        public Layer AddLayer(string path, string prodname)
        {
            Layer layer = ReadFileData<Layer>(path);
            string prodpath = basepath + prodname + "/" + layer.Hash;

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
            return layer;
        }

        private T ReadFileData <T>(string path)
        {
            T data = default;
            using (var file = File.OpenRead(path))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName == "data.bin")
                    {
                        data = DeSerilize<T>(entry.Open());
                    }
                }
            }
            return data;
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
            Stream stream = new FileStream(path + "/data.bin", FileMode.Open, FileAccess.Read);
            return DeSerilize<T>(stream);
        }

        private T DeSerilize<T>(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            T serializeble = (T)formatter.Deserialize(stream);
            stream.Close();
            return serializeble;
        }

        private string GetHashName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                DateTime now = DateTime.Now;
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(name + now.ToString());
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
