﻿using Models;
using System;
using System.Windows.Forms;

namespace CropProd
{
    public partial class LayerListItem : UserControl
    {
        public Layer Layer;
        public Action<Layer> LayerDelete;
        public Action Redaraw;
        public float Min;
        public float Max;
        public bool isHide = false;

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
            try
            {
                Max = float.Parse(RightRange.Text);
            }
            catch (Exception)
            {

            }
        }

        private void LeftRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Min = float.Parse(LeftRange.Text);
            }
            catch (Exception)
            {
            }
        }

        private void ButtonHide_Click(object sender, EventArgs e)
        {
            isHide = !isHide;
            ButtonHide.Text = ((isHide) ? "+" : "_");
            Redaraw();
        }

        private void NonAlpha_Click(object sender, EventArgs e)
        {
            Layer.nonAlpha = !Layer.nonAlpha;
            Strict.Checked = Layer.nonAlpha;
        }

        private void NonAlpha_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
