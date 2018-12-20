using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    class Frame
    {
        public Image image = null;
        public Vector2 origin = new Vector2(0, 0);
        public Vector2 lefttop = new Vector2(0, 0);
        public int zorder = 0;
    }
}
