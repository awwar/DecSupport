using CropProd;
using Models;
using System.Numerics;

namespace Interfaces
{
    internal interface IUserForm
    {
        Vector2 GetDrawableSize();
        void ChangeTitle(string title);
        void DrawLayerItem(int n);
        string ShowOpenFileDialog();
        string ShowSaveFileDialog();
        CreateProjDialogData ShowCreateProjDialog();
        LayerMakerDialogData ShowLayerMakerDialog();
        void ShowBouble(string msg);
    }
}