using System;

namespace Models
{
    [Serializable]
    class Project
    {
        public string Name { get; set; } = null;
        public string Path { get; set; } = null;
        public string Hash { get; set; } = null;
        public double Lat { get; set; } = 0;
        public double Lon { get; set; } = 0;
        public Layer[] Layers { get; set; } = null;
    }
}
