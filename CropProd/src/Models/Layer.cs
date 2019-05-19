using System;

namespace Models
{
    [Serializable]
    public class Layer : IDisposable
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public float Min { get; set; } = 0;
        public float Max { get; set; } = 1;
        public float ColorMin { get; set; } = 0;
        public float ColorMax { get; set; } = 1;
        public string Hash { get; set; }
        public int Id { get; set; }
        [NonSerialized()] public Data[] Datas = new Data[] { };
        [NonSerialized()] public float setMin = 0;
        [NonSerialized()] public float setMax = 1;
        [NonSerialized()] public bool invert = false;

        public void Dispose()
        {
            if (Datas != null)
            {
                foreach (Data item in Datas)
                {
                    item.Image.Dispose();
                }
            }

        }
    }
}
