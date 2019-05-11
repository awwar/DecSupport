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
        public bool Invert = false;

        public LayerListItem()
        {
            InitializeComponent();
            DeleteLayer.Click += DeleteLayer_Click;
            isInvert.Click += IsInvert_Click;
            this.isInvert.Checked = Invert;
        }

        private void IsInvert_Click(object sender, EventArgs e)
        {
            Invert = !Invert;
            this.isInvert.Checked = Invert;
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
