using System.Drawing;

namespace Events
{
    class ImageLoadArgs
    {
        // Сообщение
        public Image image { get; }
        public string path { get; }

        public ImageLoadArgs(Image image, string path)
        {
            this.image = image;
            this.path = path;
        }
    }
}
