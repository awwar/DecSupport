﻿using CropProd;
using Models;
using System.Numerics;

namespace Interfaces
{
    public interface IUserForm
    {
        Vector2 GetDrawableSize();
        void ChangeTitle(string title);
        string ShowOpenFileDialog();
        string ShowSaveFileDialog(string filename = null);
        CreateProjDialogData ShowCreateProjDialog();
        LayerMakerDialogData ShowLayerMakerDialog();
        void ShowBouble(string msg);
    }
}