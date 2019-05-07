using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace CropProd
{
    partial class LayerListItem : UserControl
    {
        public Layer Layer;
        public Action<Layer> LayerDelete;

        public LayerListItem()
        {
            InitializeComponent();
            this.DeleteLayer.Click += DeleteLayer_Click;
        }

        private void DeleteLayer_Click(object sender, EventArgs e)
        {
            LayerDelete(Layer);
        }
    }
}
