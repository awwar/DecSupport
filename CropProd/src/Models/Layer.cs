using System;

namespace Models
{
    [Serializable]
    class Layer
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
