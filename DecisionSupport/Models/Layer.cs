using System;
using System.Numerics;

namespace Models
{
    [Serializable]
    public class Layer : IDisposable
    {
        public string Name { get; set; }
        public double TileX { get; set; }
        public double TileY { get; set; }
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 1;
        public float ColorMin { get; set; } = 0;
        public float ColorMax { get; set; } = 1;
        public string ValueType { get; set; } = "";
        public string Hash { get; set; }
        [NonSerialized()] public Data[] DataLayers = new Data[] { };
        [NonSerialized()] public float setMin = 0;
        [NonSerialized()] public float setMax = 1;
        [NonSerialized()] public bool invert = false;

        public void Dispose()
        {
            if (DataLayers != null)
            {
                foreach (Data item in DataLayers)
                {
                    item.Image.Dispose();
                }
            }

        }

        public void Draw(Vector2 position)
        {
            if (DataLayers != null)
            {
                foreach (Data item in DataLayers)
                {
                    item.Draw(position);
                }
            }

        }
    }
}
