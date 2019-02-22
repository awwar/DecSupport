using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers
{
    class LoaderHandler
    {
        List<Tile> data = new List<Tile>();
        int count;
        public bool block = false;
        WebClient client;

        public async void Start()
        {
            client = new WebClient
            {
                Proxy = null
            };
            
            
            while (true)
            {
                if (count > 0 && block == false)
                {
                    Tile[] frms = data.ToArray();
                    data.Clear();
                    count = 0;
                    await Getimg(frms);
                    //Thread.Sleep(200);
                }
            }
        }
        
        public void AddPath(Tile frame)
        {
            try
            {
                data.Add(frame);
                count++;
            }
            catch (Exception)
            {
                Console.WriteLine("22");
            }
        }

        private async Task Getimg(Tile[] frames)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(frames[0].path));
            foreach (Tile frame in frames)
            {
                if (!File.Exists(@frame.path))
                {
                    try
                    {
                        client.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 Safari/537.36");
                        await client.DownloadFileTaskAsync(new Uri(frame.url), @frame.path);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Проблемма с загрузкой тайла");
                        continue;
                    }
                    try
                    {
                        Image img = Image.FromFile(@frame.path);
                        frame.image = img;
                        SceneHandler.AddTile(frame);
                    }
                    catch (Exception)
                    {

                        //AddPath(frame);
                        Console.WriteLine("Проблемма с скачиваннии тайла из папки");
                    }
                }

                //SceneHandler.Refresh();
            }
            Console.WriteLine("Вызвана загрузка {0} тайлов", frames.Length);
            GC.Collect();

            SceneHandler.Refresh();
        }
    }
}
