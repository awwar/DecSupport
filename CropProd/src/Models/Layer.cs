using System;

namespace Models
{
    [Serializable]
    public class Layer
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Hash { get; set; }
        public int Id { get; set; }
    }
}
