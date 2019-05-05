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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // LayName
            // 
            this.LayName.AutoSize = true;
            this.LayName.Location = new System.Drawing.Point(3, 4);
            this.LayName.Name = "LayName";
            this.LayName.Size = new System.Drawing.Size(35, 13);
            this.LayName.TabIndex = 0;
            this.LayName.Text = "label1";
            // 
            // isInvert
            // 
            this.isInvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.isInvert.AutoSize = true;
            this.isInvert.Location = new System.Drawing.Point(115, 4);
            this.isInvert.Name = "isInvert";
            this.isInvert.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.isInvert.Size = new System.Drawing.Size(63, 17);
            this.isInvert.TabIndex = 1;
            this.isInvert.TabStop = true;
            this.isInvert.Text = "Is Invert";
            this.isInvert.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(3, 21);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(175, 45);
            this.trackBar1.TabIndex = 2;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // LayerListItem
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.isInvert);
            this.Controls.Add(this.LayName);
            this.Name = "LayerListItem";
            this.Size = new System.Drawing.Size(181, 69);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RadioButton isInvert;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.Label LayName;
    }
}
