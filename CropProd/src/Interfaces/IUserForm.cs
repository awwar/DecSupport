using CropProd;
using Models;
using System.Numerics;

namespace Interfaces
{
    internal interface IUserForm
    {
        Vector2 GetDrawableSize();
        void ChangeTitle(string title);
        void RedrawLayerItem(Layer[] layer);
        string ShowOpenFileDialog();
        string ShowSaveFileDialog(string filename = null);
        CreateProjDialogData ShowCreateProjDialog();
        LayerMakerDialogData ShowLayerMakerDialog();
        void ShowBouble(string msg);
    }
}