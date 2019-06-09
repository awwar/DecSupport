using System.Collections.Generic;
using System.Drawing;

namespace Models
{
    public struct LayerMakerDialogData
    {
        public Dictionary<string, Bitmap> Tiles { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string ValueType { get; set; }
        public string FilePath { get; set; }
    }
}
