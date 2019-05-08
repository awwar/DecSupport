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
