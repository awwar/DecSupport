namespace CropProd
{
    partial class LayerListItem
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LayName = new System.Windows.Forms.Label();
            this.isInvert = new System.Windows.Forms.RadioButton();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.DeleteLayer = new System.Windows.Forms.Button();
            this.LeftRange = new System.Windows.Forms.TextBox();
            this.RightRange = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LayName
            // 
            this.LayName.Location = new System.Drawing.Point(3, 4);
            this.LayName.Name = "LayName";
            this.LayName.Size = new System.Drawing.Size(101, 19);
            this.LayName.TabIndex = 0;
            this.LayName.Text = "label1";
            // 
            // isInvert
            // 
            this.isInvert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.isInvert.AutoSize = true;
            this.isInvert.Location = new System.Drawing.Point(93, 6);
            this.isInvert.Name = "isInvert";
            this.isInvert.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.isInvert.Size = new System.Drawing.Size(63, 17);
            this.isInvert.TabIndex = 1;
            this.isInvert.TabStop = true;
            this.isInvert.Text = "Is Invert";
            this.isInvert.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // DeleteLayer
            // 
            this.DeleteLayer.AllowDrop = true;
            this.DeleteLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteLayer.BackColor = System.Drawing.Color.Red;
            this.DeleteLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.DeleteLayer.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.DeleteLayer.ForeColor = System.Drawing.Color.White;
            this.DeleteLayer.Location = new System.Drawing.Point(176, 3);
            this.DeleteLayer.Margin = new System.Windows.Forms.Padding(0);
            this.DeleteLayer.Name = "DeleteLayer";
            this.DeleteLayer.Size = new System.Drawing.Size(23, 23);
            this.DeleteLayer.TabIndex = 0;
            this.DeleteLayer.Text = "X";
            this.DeleteLayer.UseVisualStyleBackColor = false;
            // 
            // LeftRange
            // 
            this.LeftRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LeftRange.Location = new System.Drawing.Point(6, 46);
            this.LeftRange.Name = "LeftRange";
            this.LeftRange.Size = new System.Drawing.Size(94, 20);
            this.LeftRange.TabIndex = 2;
            this.LeftRange.Text = "0";
            this.LeftRange.TextChanged += new System.EventHandler(this.LeftRange_TextChanged);
            // 
            // RightRange
            // 
            this.RightRange.Location = new System.Drawing.Point(106, 46);
            this.RightRange.Name = "RightRange";
            this.RightRange.Size = new System.Drawing.Size(93, 20);
            this.RightRange.TabIndex = 3;
            this.RightRange.Text = "0";
            this.RightRange.TextChanged += new System.EventHandler(this.RightRange_TextChanged);
            // 
            // LayerListItem
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.RightRange);
            this.Controls.Add(this.LeftRange);
            this.Controls.Add(this.DeleteLayer);
            this.Controls.Add(this.isInvert);
            this.Controls.Add(this.LayName);
            this.Name = "LayerListItem";
            this.Size = new System.Drawing.Size(206, 69);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton isInvert;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.Label LayName;
        public System.Windows.Forms.Button DeleteLayer;
        public System.Windows.Forms.TextBox LeftRange;
        public System.Windows.Forms.TextBox RightRange;
    }
}
