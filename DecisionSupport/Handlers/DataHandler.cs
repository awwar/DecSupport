using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Controllers
{
    class DataHandler : IHandler
    {
        private List<Data> datas = new List<Data>();

        private Scene scene;
        private Action Redraw;

        public DataHandler(ref Scene scene, Action redraw)
        {
            this.scene = scene;
            this.Redraw = redraw;
        }

        public void AddData(Bitmap img)
        {
            datas.Add(new Data(new Vector2(0, 0), new Vector2(img.Width, img.Height), img , ref scene));
        }

        public void DeleteData(Data data)
        {
            datas.Remove(data);
        }

        public Frame[] Handle()
        {

            foreach (var item in datas)
            {
                item.Draw();
            }
            return datas.ToArray();
        }
    }
}
