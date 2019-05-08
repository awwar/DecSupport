namespace CropProd
{
    public partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.проектToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.onOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.onSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.слоиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onLayerCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.LayerList = new System.Windows.Forms.GroupBox();
            this.scene = new System.Windows.Forms.PictureBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.решениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AcceptDecision = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scene)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.MainMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MainMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.проектToolStripMenuItem,
            this.слоиToolStripMenuItem,
            this.решениеToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(1136, 24);
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
            this.onNewProject.Size = new System.Drawing.Size(132, 22);
            this.onNewProject.Text = "Создать";
            // 
            // onOpenProject
            // 
            this.onOpenProject.Name = "onOpenProject";
            this.onOpenProject.Size = new System.Drawing.Size(132, 22);
            this.onOpenProject.Text = "Открыть";
            // 
            // onSaveProject
            // 
            this.onSaveProject.Name = "onSaveProject";
            this.onSaveProject.Size = new System.Drawing.Size(132, 22);
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
            this.onLayerCreate.Size = new System.Drawing.Size(117, 22);
            this.onLayerCreate.Text = "Создать";
            // 
            // LayerList
            // 
            this.LayerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LayerList.Location = new System.Drawing.Point(923, 28);
            this.LayerList.Name = "LayerList";
            this.LayerList.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.LayerList.Size = new System.Drawing.Size(201, 656);
            this.LayerList.TabIndex = 6;
            this.LayerList.TabStop = false;
            this.LayerList.Text = "Layers";
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
            this.scene.Size = new System.Drawing.Size(911, 660);
            this.scene.TabIndex = 3;
            this.scene.TabStop = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 687);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1136, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(22, 17);
            this.Status.Text = "Ok";
            // 
            // решениеToolStripMenuItem
            // 
            this.решениеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AcceptDecision});
            this.решениеToolStripMenuItem.Name = "решениеToolStripMenuItem";
            this.решениеToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.решениеToolStripMenuItem.Text = "Решение";
            // 
            // AcceptDecision
            // 
            this.AcceptDecision.Name = "AcceptDecision";
            this.AcceptDecision.Size = new System.Drawing.Size(180, 22);
            this.AcceptDecision.Text = "Принять";
            // 
            // MainWindow
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1136, 709);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.LayerList);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.scene);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.TransparencyKey = System.Drawing.Color.Silver;
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scene)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox scene;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem проектToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onNewProject;
        private System.Windows.Forms.ToolStripMenuItem onOpenProject;
        private System.Windows.Forms.ToolStripMenuItem onSaveProject;
        private System.Windows.Forms.ToolStripMenuItem слоиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onLayerCreate;
        public System.Windows.Forms.GroupBox LayerList;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ToolStripMenuItem решениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AcceptDecision;
    }
}

