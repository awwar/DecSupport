using System;
using System.Drawing;

namespace Events
{
    public class ImageLoadArgs : EventArgs
    {
        // Сообщение
        public Image Image { get; }
        public string Path { get; }

        public ImageLoadArgs(Image image, string path)
        {
            this.Image = image;
            this.Path = path;
        }
    }
}
