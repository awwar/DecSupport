using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    class Project
    {
        public string Name { get; set; } = null;
        public string Path { get; set; } = null;
        public double Lat { get; set; } = 0;
        public double Lon { get; set; } = 0;
        public string Hash { get; set; } = null;
    }
}
