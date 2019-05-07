using System;

namespace Models
{
    [Serializable]
    public class Layer : IDisposable
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Hash { get; set; }
        public int Id { get; set; }
        [NonSerialized()]  public Data[] Datas = new Data[] { };

        public void Dispose()
        {
            foreach (Data item in Datas)
            {
                item.Image.Dispose();
            }
        }
    }
}
