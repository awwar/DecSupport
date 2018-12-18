using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    class Frame
    {
        public Image image = null;
        public Point origin = new Point(0, 0);
        public Point lefttop = new Point(0, 0);
        public int zorder = 0;
    }
}
