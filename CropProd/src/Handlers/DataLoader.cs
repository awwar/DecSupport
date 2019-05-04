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
            project.Hash = GetHashProjName(project);

            string tempPath = basepath + project.Hash;

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);                
            }
        }

        public void SaveProject(Project project)
        {
            string tempPath = basepath + project.Hash;

            project.Hash = GetHashProjName(project);

            Serilize(project, tempPath);
            if (File.Exists(project.Path))
            {
                File.Delete(project.Path);
            }
            ZipFile.CreateFromDirectory(tempPath, project.Path);
        }

        public Project LoadProject(string path)
        {
            Project project = ReadProjectData(path);
            string tempPath = basepath + project.Hash;
            if (!Directory.Exists(tempPath))
            {
                ZipFile.ExtractToDirectory(path, tempPath);
                project.Hash = GetHashProjName(project);
            }
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

        private Project ReadProjectData(string path)
        {
            using (var file = File.OpenRead(path))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName == "data.bin")
                    {
                        return DeSerilize<Project>(entry.Open());
                    }
                }
            }
            return null;
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

        private string GetHashProjName(Project project)
        {
            if (String.IsNullOrEmpty(project.Name))
                return String.Empty;

            if (!String.IsNullOrEmpty(project.Hash))
                return project.Hash;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                DateTime now = DateTime.Now;
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(project.Name + now.ToString());
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
