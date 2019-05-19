using Models;
using System;
using System.Windows.Forms;

namespace CropProd
{
    public partial class LayerListItem : UserControl
    {
        public Layer Layer;
        public Action<Layer> LayerDelete;
        public float Min;
        public float Max;

        public LayerListItem(Layer lay)
        {
            InitializeComponent();
            Layer = lay;
            DeleteLayer.Click += DeleteLayer_Click;
            isInvert.Click += IsInvert_Click;
            isInvert.Checked = Layer.invert;
        }

        private void IsInvert_Click(object sender, EventArgs e)
        {
            Layer.invert = !Layer.invert;
            isInvert.Checked = Layer.invert;
        }

        private void DeleteLayer_Click(object sender, EventArgs e)
        {
            LayerDelete(Layer);
        }

        private void RightRange_TextChanged(object sender, EventArgs e)
        {
            Max = float.Parse(RightRange.Text);
        }

        private void LeftRange_TextChanged(object sender, EventArgs e)
        {
            Min = float.Parse(LeftRange.Text);
        }
    }
}
