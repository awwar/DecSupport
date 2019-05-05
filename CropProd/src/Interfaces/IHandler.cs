using Models;
using System;

namespace Interfaces
{
    internal interface IHandler
    {
        Action Redraw { get; set; }
        Frame[] Handle();
    }
}
