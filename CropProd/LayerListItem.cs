using Models;
using System;
using System.Windows.Forms;

namespace CropProd
{
    partial class LayerListItem : UserControl
    {
        public Layer Layer;
        public Action<Layer> LayerDelete;

        public LayerListItem()
        {
            InitializeComponent();
            DeleteLayer.Click += DeleteLayer_Click;
        }

        private void DeleteLayer_Click(object sender, EventArgs e)
        {
            LayerDelete(Layer);
        }
    }
}
