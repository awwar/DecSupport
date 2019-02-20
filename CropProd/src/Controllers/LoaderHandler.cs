using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    class LoaderHandler
    {
        List<Frame> data = new List<Frame>();
        public bool block = false;

        public void Start()
        {
            do
            {
                if(data.Count > 0 && block == false)
                {
                    Frame[] frms = data.ToArray();
                    data.Clear();
                    Getimg(frms);
                }
            } while (true);
        }

        public void AddPath(Frame frame)
        {
            try
            {                    
                data.Add(frame);
            }
            catch (Exception)
            {

            }
        }

        private void Getimg(Frame[] frames)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");

                foreach (Frame frame in frames)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(@frame.path));
                    try
                    {
                        client.DownloadFile(new Uri(frame.url), @frame.path);
                        try
                        {
                            Image img = Image.FromFile(frame.path);
                            frame.image = img;
                            SceneHandler.AddFrame(frame);
                        }
                        catch (Exception)
                        {
                            AddPath(frame);
                        }
                    }
                    catch (Exception)
                    {
                        AddPath(frame);
                    }
                    SceneHandler.Refresh();
                }                
            }
        }
    }
}
