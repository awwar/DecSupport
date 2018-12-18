﻿using Controllers;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CropProd
{
    public partial class Form1 : Form
    {
        SceneHandler ScenHand;
        public Form1()
        {
            InitializeComponent();
            ScenHand = new SceneHandler();
            //Handle draw calls
            Scene.Paint += new PaintEventHandler(ScenHand.Draw);
            Scene.MouseDown += new MouseEventHandler(ScenHand.Scene_MouseDown);
            Scene.MouseUp += new MouseEventHandler(ScenHand.Scene_MouseUp);
            Scene.MouseLeave += new EventHandler(ScenHand.Scene_MouseLeave);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "";

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filename = ofd.FileName;
                    label1.Text = "Открыт файл "+ filename;
                }
            }
            ScenHand.addFrame(new Point(0, 0), Image.FromFile(filename));
            Scene.Refresh();
        }
    }
}
