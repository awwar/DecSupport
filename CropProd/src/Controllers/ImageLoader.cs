   
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
        private readonly WebClient client;

        public delegate void ImageLoadHandler(object sender, ImageLoadArgs e);
        public event ImageLoadHandler onImageLoad;

        private Dictionary<string, Tile> data = new Dictionary<string, Tile>();
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
        public void AddFrame(Tile frame)
        {
            data[frame.path] = frame;
            Load();
        }

        /*
         *  Удаления изображение из пула      
         */
        public void DeleteFrame(string path)
        {
            Tile tile;
            if(data.TryGetValue(path, out tile))
            {
                onImageLoad -= tile.ImageLoaded;
                tile.image.Dispose();
            }  
            data.Remove(path);
        }

        /*
         *  Удаления все изображения из пула      
         */
        public void ClearPool()
        {
            foreach (KeyValuePair<string, Tile> item in data)
            {
                onImageLoad -= item.Value.ImageLoaded;
                item.Value.image.Dispose();
            }
            data.Clear();
        }

        /*
        * Метод тригеррит ивент у подписчиков
        */
        private void ImageLoaded(Image img, string path)
        {
            onImageLoad(this, new ImageLoadArgs(img, path));
            SceneHandler.Refresh();
        }

        private Tile DictPop()
        {
            Tile tile = null;
            KeyValuePair<string, Tile> last = data.Last();
            tile = last.Value;
            data.Remove(last.Key);
            if (tile == null)
            {
                throw new Exception("Path not found");
            }
            return tile;

        }

        private async void Load()
        {
            if (isLoading)
            {
                return;
            }
            isLoading = true;
            Tile frame;
            Image img = null;
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
                    Console.WriteLine("LOad {0}",e);
                    File.Delete(@frame.path);
                    return;
                }
                try
                {
                    using (FileStream myStream = new FileStream(frame.path, FileMode.Open, FileAccess.Read))
                    {
                        img = Image.FromStream(myStream);
                    }
                    ImageLoaded(img, frame.path);
                }
                catch
                {
                    Console.WriteLine("Filetaker error!");
                    File.Delete(@frame.path);
                    return;
                }
                
            }
        }
    }
}