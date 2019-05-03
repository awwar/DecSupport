namespace CropProd
{
    partial class MainWindow
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.scene = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.проектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.onOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.onSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.слоиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onLayerCreate = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.scene)).BeginInit();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 2;
            // 
            // scene
            // 
            this.scene.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scene.Cursor = System.Windows.Forms.Cursors.NoMove2D;
            this.scene.Location = new System.Drawing.Point(9, 24);
            this.scene.Margin = new System.Windows.Forms.Padding(0);
            this.scene.Name = "scene";
            this.scene.Size = new System.Drawing.Size(971, 464);
            this.scene.TabIndex = 3;
            this.scene.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(127, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.MainMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MainMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.проектToolStripMenuItem,
            this.слоиToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(989, 24);
            this.MainMenu.TabIndex = 5;
            this.MainMenu.Text = "MainMenu";
            // 
            // проектToolStripMenuItem
            // 
            this.проектToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onNewProject,
            this.onOpenProject,
            this.onSaveProject});
            this.проектToolStripMenuItem.Name = "проектToolStripMenuItem";
            this.проектToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.проектToolStripMenuItem.Text = "Проект";
            // 
            // onNewProject
            // 
            this.onNewProject.Name = "onNewProject";
            this.onNewProject.Size = new System.Drawing.Size(180, 22);
            this.onNewProject.Text = "Создать";
            // 
            // onOpenProject
            // 
            this.onOpenProject.Name = "onOpenProject";
            this.onOpenProject.Size = new System.Drawing.Size(180, 22);
            this.onOpenProject.Text = "Открыть";
            // 
            // onSaveProject
            // 
            this.onSaveProject.Name = "onSaveProject";
            this.onSaveProject.Size = new System.Drawing.Size(180, 22);
            this.onSaveProject.Text = "Сохранить";
            // 
            // слоиToolStripMenuItem
            // 
            this.слоиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onLayerCreate});
            this.слоиToolStripMenuItem.Name = "слоиToolStripMenuItem";
            this.слоиToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.слоиToolStripMenuItem.Text = "Слои";
            // 
            // onLayerCreate
            // 
            this.onLayerCreate.Name = "onLayerCreate";
            this.onLayerCreate.Size = new System.Drawing.Size(180, 22);
            this.onLayerCreate.Text = "Создать";
            // 
            // MainWindow
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(989, 497);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.scene);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.scene)).EndInit();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox scene;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem проектToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onNewProject;
        private System.Windows.Forms.ToolStripMenuItem onOpenProject;
        private System.Windows.Forms.ToolStripMenuItem onSaveProject;
        private System.Windows.Forms.ToolStripMenuItem слоиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onLayerCreate;
    }
}

