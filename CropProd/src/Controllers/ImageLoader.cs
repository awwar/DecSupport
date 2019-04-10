using Events;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Controllers
{
    class ImageLoader
    {
        private WebClient client;

        public delegate void ImageLoadHandler(object sender, ImageLoadArgs e);
        public event ImageLoadHandler onImageLoad;

        private List<Tile> data = new List<Tile>();
        private bool isLoading = false;


        public ImageLoader()
        {
            client = new WebClient
            {
                Proxy = null
            };

            client.DownloadFileCompleted += Client_DownloadFileCompleted;
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
        public void AddFrame(Tile tile)
        {
            data.Add(tile);
            Load();
        }

        /*
         *  Удаления изображение из пула      
         */
        public void DeleteFrame(Tile tile)
        {
            data.Remove(tile);           
        }

        /*
         *  Удаления все изображения из пула      
         */
        public void ClearPool()
        {
            isLoading = false;
            foreach (Tile item in data)
            {
                onImageLoad -= item.ImageLoaded;
                item.image.Dispose();
            }
            data.Clear();
        }

        /*
        * Метод тригеррит ивент у подписчиков
        */
        private void ImageLoaded(Image img, string path)
        {
            onImageLoad(this, new ImageLoadArgs(img, path));
        }

        private Tile DictPop()
        {
            Tile last = data.Last();
            data.RemoveAt(data.Count - 1);
            if (last == null)
            {
                throw new Exception("Path not found");
            }
            return last;
        }

        private async void Load()
        {
            if (isLoading)
            {
                return;
            }
            isLoading = true;
            Tile frame;
            try
            {
                frame = DictPop();
            }
            catch
            {
                return;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(frame.path));

            if (!File.Exists(frame.path))
            {
                try
                {
                    client.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 Safari/537.36");
                    await client.DownloadFileTaskAsync(new Uri(frame.url), @frame.path);
                }
                catch (Exception e)
                {
                    return;
                }
                try
                {
                    Image img = Image.FromFile(@frame.path);
                    ImageLoaded(img, frame.path);
                }
                catch (Exception)
                {
                    Console.WriteLine("Filetaker error!");
                    return;
                }
            }
        }
    }
}
