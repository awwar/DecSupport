using System.Collections.Generic;
using System.Drawing;

namespace Models
{
    public struct LayerMakerDialogData
    {
        public Dictionary<string, Bitmap> Tiles;
        public string Lat;
        public string Lon;
        public string Min;
        public string Max;
        public string FileName;
    }
}
