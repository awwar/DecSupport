using DSCore;
using Events;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;

namespace Handlers
{
    class TileLoader
    {
        private readonly WebClient client;
        private string basepath = "";
        public delegate void ImageLoadHandler(object sender, ImageLoadArgs e);
        public event ImageLoadHandler OnImageLoad;

        private readonly Random rnd;
        private Dictionary<string, Tile> data = new Dictionary<string, Tile>();
        private bool isLoading = false;


        public TileLoader()
        {
            rnd = new Random();
            basepath = Settings.TempPath + "CropPod/tiles";

            if (!Directory.Exists(basepath))
            {
                Directory.CreateDirectory(basepath);
            }

            client = new WebClient
            {
                Proxy = null
            };

            client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        /**
         * ищем тайл или в папке или пытаемся его скачать
         */
        public Tile ReserveTile(double X, double Y, double Z, Vector2 leftop)
        {
            Tile tile;
            string url = string.Format(
                Settings.DistributorSrc,
                X,
                Y,
                Z,
                rnd.Next(1, 3));
            string filename = $"{X.ToString()}_{Y.ToString()}.jpeg";
            string query = string.Format(
                @"{0}\{1}\{2}\",
                basepath,
                Settings.DistributorName,
                Z);
            string fullpath = query + filename;

            if (!File.Exists(fullpath))
            {
                tile = new Tile(
                    leftop,
                    url,
                    fullpath
                );
                AddFrame(tile);
            }
            else
            {
                Image img;
                try
                {
                    FileStream myStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                    img = Image.FromStream(myStream);
                    myStream.Close();
                }
                catch (Exception)
                {
                    img = null;
                }
                tile = new Tile(
                    leftop,
                    img
                );
            }
            return tile;
        }

        /*
         *  Когда загрузка завершена (успешно или нет???) пытаемся запустить другие
         *  isLoading - запрещает закачивать что-то если УЖЕ идет загрузка
         */
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            isLoading = false;
            if (data.Count > 0)
            {
                Load();
            }
        }

        /*
         *  Добавляем изображение в пулл    
         */
        public void AddFrame(Tile frame)
        {
            data[frame.Path] = frame;
            Load();
        }

        /*
         *  Удаления изображение из пула      
         */
        public void DeleteFrame(string path)
        {
            data.Remove(path);
        }

        /*
         *  Удаления все изображения из пула      
         */
        public void ClearPool()
        {
            foreach (KeyValuePair<string, Tile> item in data)
            {
                item.Value.Image.Dispose();
            }
            data.Clear();
        }

        private Tile DictPop()
        {
            KeyValuePair<string, Tile> last = data.Last();
            Tile tile = last.Value;
            data.Remove(last.Key);
            return tile;
        }

        private async void Load()
        {
            if (isLoading)
            {
                return;
            }
            isLoading = true;
            Tile frame = DictPop();

            if (frame == null)
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(frame.Path));

            if (!File.Exists(frame.Path))
            {
                try
                {
                    client.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 Safari/537.36");
                    await client.DownloadFileTaskAsync(new Uri(frame.Url), @frame.Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOad {0}", e);
                    File.Delete(@frame.Path);
                    return;
                }
                FileStream myStream = new FileStream(frame.Path, FileMode.Open, FileAccess.Read);
                try
                {
                    Image img = Image.FromStream(myStream);
                    OnImageLoad(this, new ImageLoadArgs(img, frame.Path));
                    myStream.Close();
                }
                catch
                {
                    myStream.Close();
                    Console.WriteLine("Filetaker error!");
                    File.Delete(@frame.Path);
                    return;
                }
                return;
            }
        }
    }
}