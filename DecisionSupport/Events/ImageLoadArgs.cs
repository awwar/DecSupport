﻿using System.Drawing;

namespace Events
{
    internal class ImageLoadArgs
    {
        // Сообщение
        public Image Image { get; }
        public string Path { get; }

        public ImageLoadArgs(Image image, string path)
        {
            Image = image;
            Path = path;
        }
    }
}
