using System;
using System.Drawing;

namespace Events
{
    public class FormResizeArgs : EventArgs
    {
        // Сообщение
        public float Width { get; }
        public float Height { get; }

        public FormResizeArgs(float Width, float Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
}