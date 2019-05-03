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
        public string Name { get; set; }
        public string Path { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
