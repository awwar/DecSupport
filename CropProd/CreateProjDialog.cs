using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CropProd
{
    public struct CreateProjDialogData
    {
        public string Lat;
        public string Lon;
        public string Name;
    }

    public partial class CreateProjDialog : Form
    {
        public CreateProjDialog()
        {
            InitializeComponent();
        }
    }
}
