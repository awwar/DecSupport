using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers
{
    class LoaderHandler
    {
        List<Frame> data = new List<Frame>();
        int count;
        public bool block = false;
        WebClient client;

        public async void Start()
        {
            client = new WebClient();
            client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            /*client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;*/
            while (true)
            {
                if (count > 0 && block == false)
                {
                    Frame[] frms = data.ToArray();
                    data.Clear();
                    count = 0;
                    await Getimg(frms);
                }
            }
        }

       /* void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("File Download Compeleted.");
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //lblMessage.Text = e.ProgressPercentage + " % Downloaded.";
        }*/

        public void AddPath(Frame frame)
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

        private async Task Getimg(Frame[] frames)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(frames[0].path));
            foreach (Frame frame in frames)
            {
                byte[] bimg = new byte[0];
                if (!File.Exists(@frame.path))
                {
                    try
                    {
                        await client.DownloadFileTaskAsync(new Uri(frame.url), @frame.path);
                        //bimg = await client.DownloadDataTaskAsync(frame.url);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("44");
                    }
                }

                try
                {
                    Image img = Image.FromFile(@frame.path);
                    /*Stream stream = new MemoryStream(bimg);
                    Image img = Image.FromStream(stream);*/
                    frame.image = img;
                    SceneHandler.AddFrame(frame);
                }
                catch (Exception)
                {

                    AddPath(frame);
                    Console.WriteLine("55");
                }
                SceneHandler.Refresh();
            }
        }
    }
}
